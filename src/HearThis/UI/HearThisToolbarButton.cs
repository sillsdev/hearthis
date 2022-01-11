// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International. All Rights Reserved.
// <copyright from='2017' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HearThis.UI
{
	/// <summary>
	/// Simple image-based buttons that support having a on/off state. They don't yet support an enable/disabled state.
	/// </summary>
	public partial class HearThisToolbarButton : PictureBox
	{
		private bool _mouseDown = false;
		private bool _checked = false;
		private bool _isBeingPointedTo;
		const int kmargin = 4;
		private const float kThickPenWidth = 2f;
		private static readonly Pen sThinBoundsPen = new Pen(AppPalette.CommonMuted);
		private static readonly Pen sThickBoundsPen = new Pen(AppPalette.CommonMuted, kThickPenWidth);
		public event EventHandler CheckedChanged;

		public HearThisToolbarButton()
		{
			InitializeComponent();
			MouseEnter += OnMouseEnter;
			MouseLeave += HearThisButton_MouseLeave;
			SizeChanged += HearThisButton_SizeChanged;
			SizeMode = PictureBoxSizeMode.CenterImage;
		}

		private void HearThisButton_SizeChanged(object sender, EventArgs e)
		{
			//Ignore that size change
			SetBounds();
		}

		public new Image Image
		{
			get => base.Image;
			set
			{
				base.Image = value;
				SetBounds();
				Invalidate();
			}
		}

		[Description("Act like a checkbox"), Category("Behavior")]
		public bool CheckBox { get; set; }

		[Description("When true, uses forecolor for border (instead of CommonMuted)"), Category("Appearance")]
		public bool UseForeColorForBorder { get; set; }

		public bool Checked
		{
			get => _checked;
			set
			{
				_checked = value;
				CheckedChanged?.Invoke(this, null);
				Invalidate();
			}
		}

		private void SetBounds()
		{
			if (Image != null)
			{
				Width = Image.Width + kmargin*2;
				Height = Image.Height + kmargin*2;
			}
		}

		private void HearThisButton_MouseLeave(object sender, EventArgs e)
		{
			_isBeingPointedTo = false;
			Invalidate();
		}

		private void OnMouseEnter(object sender, EventArgs e)
		{
			_isBeingPointedTo = true;
			Invalidate();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left && this.Image != null)
			{
				_mouseDown = true;
				Invalidate();
			}

			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left && this.Image != null)
			{
				_mouseDown = false;
				Invalidate();
			}

			Checked = !Checked;

			base.OnMouseUp(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			//base.OnPaint(e); Doing our own painting because we want to draw the image with a constant margin around, to make room for the selection rectangle
			e.Graphics.DrawImageUnscaled(base.Image, kmargin, kmargin);

			if (_mouseDown)
			{
				e.Graphics.ScaleTransform(.9f,.9f);
			}

			if(_isBeingPointedTo)
			{
				var r = new Rectangle(1, 1, Width - 2, Height - 2);
				if (UseForeColorForBorder)
				{
					using (var pen = new Pen(ForeColor, kThickPenWidth))
						e.Graphics.DrawRectangle(pen, r);
				}
				else
					e.Graphics.DrawRectangle(sThickBoundsPen, r);
			}
			else if(CheckBox && Checked)
			{
				var r = new Rectangle(0, 0, Width - 1, Height - 1);
				if (UseForeColorForBorder)
				{
					using (var pen = new Pen(ForeColor))
						e.Graphics.DrawRectangle(pen, r);
				}
				else
					e.Graphics.DrawRectangle(sThinBoundsPen, r);
			}
		}
	}
}
