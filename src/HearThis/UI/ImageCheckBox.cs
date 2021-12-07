using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace HearThis.UI
{
	class ImageCheckBox : CheckBox
	{
		#region private member variables
		private Image _imageCheckedFocused = null;
		private Image _imageCheckedMouseOver = null;
		private Image _imageCheckedInactive = null;

		private Color _boxBackColor = Color.Empty;
		private Color _innerBorderColorFocused = Color.Transparent;
		private Color _innerBorderColorMouseOver = Color.Transparent;

		private bool _mouseIsOverControl = false;

		private Rectangle _checkBoxRect;
		#endregion

		public ImageCheckBox()
		{
			SetStyle(ControlStyles.DoubleBuffer
				| ControlStyles.UserPaint
				| ControlStyles.SupportsTransparentBackColor, true);

			SetStyle(ControlStyles.Opaque, false);

			UpdateCheckBoxRect();
		}

		private void UpdateCheckBoxRect()
		{
			var checkBoxSize = Math.Min(ClientSize.Height - Padding.Vertical, ClientSize.Width - Padding.Horizontal);
			_checkBoxRect = new Rectangle(Padding.Left, Padding.Top, checkBoxSize, checkBoxSize);
		}

		#region Designer Properties
		[PtxUtils.StableAPI,
		Browsable(true),
		Category("Appearance"),
		Description("Image to display when checked."),
		RefreshProperties(RefreshProperties.Repaint)]
		public Image ImageCheckedNormal { get; set; }

		[PtxUtils.StableAPI,
		Browsable(true),
		Category("Appearance"),
		Description("Image to display when checked and focused."),
		RefreshProperties(RefreshProperties.Repaint)]
		public Image ImageCheckedFocused
		{
			get => _imageCheckedFocused ?? ImageCheckedNormal;
			set => _imageCheckedFocused = value;
		}

		[PtxUtils.StableAPI,
		Browsable(true),
		Category("Appearance"),
		Description("Image to display when checked and mouse is over control."),
		RefreshProperties(RefreshProperties.Repaint)]
		public Image ImageCheckedMouseOver
		{
			get => _imageCheckedMouseOver ?? ImageCheckedNormal;
			set => _imageCheckedMouseOver = value;
		}

		[PtxUtils.StableAPI,
		Browsable(true),
		Category("Appearance"),
		Description("Image to display when checked but disabled."),
		RefreshProperties(RefreshProperties.Repaint)]
		public Image ImageCheckedInactive
		{
			get => _imageCheckedInactive ?? ImageCheckedNormal;
			set => _imageCheckedInactive = value;
		}

		[PtxUtils.StableAPI,
		Browsable(true),
		Category("Appearance"),
		Description("Padding around Image (inside check box)."),
		RefreshProperties(RefreshProperties.Repaint)]
		public Padding ImagePadding { get; set; } = new Padding(2, 2, 2, 2);

		[PtxUtils.StableAPI,
		Browsable(true),
		Category("Appearance"),
		Description("Color to paint the inside of the box when not checked."),
		RefreshProperties(RefreshProperties.Repaint)]
		public Color BoxBackColor
		{
			get => _boxBackColor == Color.Empty ? BackColor : _boxBackColor;
			set => _boxBackColor = value;
		}

		[PtxUtils.StableAPI,
		Browsable(true),
		Category("Appearance"),
		Description("Color to create the popup effect when FlatStyle is Popup."),
		RefreshProperties(RefreshProperties.Repaint)]
		public Color InnerBorderColor { get; set; } = Color.Transparent;

		[PtxUtils.StableAPI,
		Browsable(true),
		Category("Appearance"),
		Description("Color to create the popup effect when focused and FlatStyle is Popup."),
		RefreshProperties(RefreshProperties.Repaint)]
		public Color InnerBorderColorFocused
		{
			get => _innerBorderColorFocused == Color.Empty ? InnerBorderColor : _innerBorderColorFocused;
			set => _innerBorderColorFocused = value;
		}

		[PtxUtils.StableAPI,
		Browsable(true),
		Category("Appearance"),
		Description("Color to create the popup effect when the mouse is over it and FlatStyle is not Flat."),
		RefreshProperties(RefreshProperties.Repaint)]
		public Color InnerBorderColorMouseOver
		{
			get => _innerBorderColorMouseOver == Color.Empty ? InnerBorderColor : _innerBorderColorMouseOver;
			set => _innerBorderColorMouseOver = value;
		}

		[PtxUtils.StableAPI,
		Browsable(true),
		Category("Appearance"),
		Description("Number of pixels between trialing edge of check box and start of the area where the text can be drawn."),
		RefreshProperties(RefreshProperties.Repaint),
		DefaultValue(3)]
		public int GapBetweenBoxAndText { get; set; } = 3;
		#endregion

		protected override void OnPaint(PaintEventArgs pevent)
		{
			if (FlatStyle == FlatStyle.System)
			{
				// No point in using this control.
				base.OnPaint(pevent);
				return;
			}

			DrawText(pevent);

			var backColor = Capture && _mouseIsOverControl ? FlatAppearance.MouseDownBackColor :
				(_mouseIsOverControl ? FlatAppearance.MouseOverBackColor : (Checked ? FlatAppearance.CheckedBackColor : BoxBackColor));

			if (backColor.A > 0)
			{
				using (var backBrush = new SolidBrush(backColor))
					pevent.Graphics.FillRectangle(backBrush, _checkBoxRect);
			}

			var adjCheckBoxRect = _checkBoxRect;
			adjCheckBoxRect.Width -= FlatAppearance.BorderSize;
			adjCheckBoxRect.Height -= FlatAppearance.BorderSize;

			if (FlatStyle == FlatStyle.Flat || FlatStyle == FlatStyle.Standard)
			{
				using (var boxPen = new Pen(Capture || _mouseIsOverControl ? SystemColors.HotTrack : FlatAppearance.BorderColor, FlatAppearance.BorderSize))
					pevent.Graphics.DrawRectangle(boxPen, adjCheckBoxRect);
			}

			if (FlatStyle == FlatStyle.Popup && _mouseIsOverControl || FlatStyle == FlatStyle.Standard)
			{
				// TODO: Draw popup box
			}

			adjCheckBoxRect.X += ImagePadding.Left;
			adjCheckBoxRect.Y += ImagePadding.Top;
			adjCheckBoxRect.Width -= ImagePadding.Horizontal;
			adjCheckBoxRect.Height -= ImagePadding.Vertical;

			Debug.WriteLine("In OnPaint: Checked = " + Checked);

			var img = Image;
			if (Checked)
			{
				img = Enabled ? ImageCheckedNormal : ImageCheckedInactive;
				if (_mouseIsOverControl)
					img = ImageCheckedMouseOver;
				else if (Focused)
					img = ImageCheckedFocused;
				pevent.Graphics.DrawImage(img, adjCheckBoxRect);
			}
			if (img != null)
				pevent.Graphics.DrawImage(img, adjCheckBoxRect);
		}

		private void DrawText(PaintEventArgs pevent)
		{
			if (!string.IsNullOrEmpty(Text))
			{
				var format = new StringFormat(StringFormatFlags.NoWrap);

				switch (TextAlign)
				{
					case ContentAlignment.TopLeft:
						format.LineAlignment = StringAlignment.Near;
						format.Alignment = StringAlignment.Near;
						break;
					case ContentAlignment.TopCenter:
						format.LineAlignment = StringAlignment.Near;
						format.Alignment = StringAlignment.Center;
						break;
					case ContentAlignment.TopRight:
						format.LineAlignment = StringAlignment.Near;
						format.Alignment = StringAlignment.Far;
						break;
					case ContentAlignment.MiddleLeft:
						format.LineAlignment = StringAlignment.Center;
						format.Alignment = StringAlignment.Near;
						break;
					case ContentAlignment.MiddleCenter:
						format.LineAlignment = StringAlignment.Center;
						format.Alignment = StringAlignment.Center;
						break;
					case ContentAlignment.MiddleRight:
						format.LineAlignment = StringAlignment.Center;
						format.Alignment = StringAlignment.Far;
						break;
					case ContentAlignment.BottomLeft:
						format.LineAlignment = StringAlignment.Far;
						format.Alignment = StringAlignment.Near;
						break;
					case ContentAlignment.BottomCenter:
						format.LineAlignment = StringAlignment.Far;
						format.Alignment = StringAlignment.Center;
						break;
					case ContentAlignment.BottomRight:
						format.LineAlignment = StringAlignment.Far;
						format.Alignment = StringAlignment.Far;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				using (SolidBrush brush = new SolidBrush(ForeColor))
					pevent.Graphics.DrawString(Text, Font, brush, new RectangleF(_checkBoxRect.Right + GapBetweenBoxAndText,
						ClientRectangle.Y + Padding.Top,
						ClientSize.Width - Padding.Horizontal - _checkBoxRect.Width - GapBetweenBoxAndText,
						ClientSize.Height - Padding.Vertical), format);
			}
		}

		#region Event Handlers
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			UpdateCheckBoxRect();
		}

		protected override void OnPaddingChanged(EventArgs e)
		{
			base.OnPaddingChanged(e);
			UpdateCheckBoxRect();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			Capture = true;
			base.OnMouseDown(e);
			Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			Capture = false;
			Invalidate();
		}

		protected override void OnCheckedChanged(EventArgs e)
		{
			base.OnCheckedChanged(e);
			Debug.WriteLine("Checked: " + Checked);
		}

		protected override void OnMouseEnter(EventArgs eventargs)
		{
			_mouseIsOverControl = true;
			base.OnMouseEnter(eventargs);
		}

		protected override void OnMouseLeave(EventArgs eventargs)
		{
			_mouseIsOverControl = false;
			base.OnMouseEnter(eventargs);
		}
		#endregion
	}
}
