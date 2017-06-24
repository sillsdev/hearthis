using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HearThis.UI
{
	public partial class HearThisToolbarButton : PictureBox
	{
		private bool _mouseDown = false;
		public bool _checked = false;
		private bool _isBeingPointedTo;
		const int kmargin = 4;
		private static readonly Pen sThinBoundsPen = new Pen(AppPallette.CommonMuted);
		private static readonly Pen sThickBoundsPen = new Pen(AppPallette.CommonMuted, 2f);
		public event EventHandler CheckedChanged;

		public HearThisToolbarButton()
		{
			InitializeComponent();
			MouseEnter += OnMouseEnter;
			MouseLeave += HearThisButton_MouseLeave;
			this.SizeChanged += HearThisButton_SizeChanged;
			SizeMode = PictureBoxSizeMode.CenterImage;
		}

		private void HearThisButton_SizeChanged(object sender, EventArgs e)
		{
			//Ignore that size change
			SetBounds();
		}

		public new Image Image
		{
			get { return base.Image; }
			set
			{
				base.Image = value;
				SetBounds();
				Invalidate();
			}
		}

		[Description("Act like a checkbox"), Category("Behavior")]
		public bool CheckBox { get; set; }

		public bool Checked
		{
			get { return _checked; }
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
			//base.OnPaint(e);
			e.Graphics.DrawImageUnscaled(base.Image, kmargin, kmargin);

			if (_mouseDown)
			{
				e.Graphics.ScaleTransform(.9f,.9f);
			}

			if(_isBeingPointedTo)
			{
				var r = new Rectangle(0, 0, Width - 2, Height - 2);
				e.Graphics.DrawRectangle(sThickBoundsPen, r);
			}
			else if(CheckBox && Checked)
			{
				var r = new Rectangle(0, 0, Width - 1, Height - 1);
				e.Graphics.DrawRectangle(sThinBoundsPen, r);
			}
		}
	}
}
