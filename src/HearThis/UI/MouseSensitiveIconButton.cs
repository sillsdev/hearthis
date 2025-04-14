// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022-2025, SIL Global.
// <copyright from='2022' to='2025' company='SIL Global'>
//		Copyright (c) 2022-2025, SIL Global.
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
	internal class MouseSensitiveIconButton : Button, ISupportInitialize
	{
		private Image _defaultImage;

		public int RoundedBorderThickness { get; set; }

		public Color RoundedBorderColor { get; set; }

		public Image MouseOverImage { get; set; }

		public Image HighContrastMouseOverImage { get; set; }

		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			ForeColor = AppPalette.HilightColor;
			Image = (AppPalette.CurrentColorScheme == ColorScheme.Normal) ?
				MouseOverImage : HighContrastMouseOverImage;
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			ForeColor = Color.DarkGray;
			Image = _defaultImage;
		}

		protected override void OnPaint(PaintEventArgs pevent)
		{
			base.OnPaint(pevent);

			if (RoundedBorderThickness > 0)
			{
				var rect = ClientRectangle;
				rect.Inflate(-RoundedBorderThickness, -RoundedBorderThickness);

				pevent.Graphics.DrawRoundedRectangle(RoundedBorderColor, rect, 8, RoundedBorderThickness);
			}
		}

		public void BeginInit()
		{
			// No op
		}

		public void EndInit()
		{
			_defaultImage = Image;
			FlatAppearance.MouseDownBackColor =
				FlatAppearance.MouseOverBackColor =
					AppPalette.Background;
		}
	}
	
	internal class RadioButtonHelperButton : MouseSensitiveIconButton
	{
		private RadioButton _correspondingRadioButton;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RadioButton CorrespondingRadioButton
		{
			get => _correspondingRadioButton;
			set
			{
				if (_correspondingRadioButton != null)
				{
					_correspondingRadioButton.MouseEnter -= _correspondingRadioButton_MouseEnter;
					_correspondingRadioButton.MouseLeave -= _correspondingRadioButton_MouseLeave;
				}

				_correspondingRadioButton = value;
				_correspondingRadioButton.MouseEnter += _correspondingRadioButton_MouseEnter;
				_correspondingRadioButton.MouseLeave += _correspondingRadioButton_MouseLeave;
			}
		}

		private void _correspondingRadioButton_MouseEnter(object sender, EventArgs e)
		{
			if (!_correspondingRadioButton.Checked)
				OnMouseEnter(e);
		}

		private void _correspondingRadioButton_MouseLeave(object sender, EventArgs e)
		{
			OnMouseLeave(e);
		}

		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			CorrespondingRadioButton.Checked = true;
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			if (!CorrespondingRadioButton.Checked)
				base.OnMouseEnter(e);
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			if (CorrespondingRadioButton != null)
				CorrespondingRadioButton.Visible = Visible;
		}

		protected override void OnEnabledChanged(EventArgs e)
		{
			base.OnEnabledChanged(e);
			if (CorrespondingRadioButton != null)
				CorrespondingRadioButton.Enabled = Enabled;
		}
	}
}
