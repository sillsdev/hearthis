using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HearThis
{
	public partial class ImageButton : Button
	{
		private Image EnabledImage;
		private Image DisabledImage;

		public ImageButton()
		{
			InitializeComponent();
			EnabledImage = DisabledImage = Image;
		}

		public void Initialize(Image enabled, Image disabled)
		{
			EnabledImage = enabled;
			DisabledImage = disabled;
			Image = Enabled ? EnabledImage : DisabledImage;
		}

		private void ImageButton_EnabledChanged(object sender, EventArgs e)
		{
		   if(EnabledImage!=null)
			   Image = Enabled ? EnabledImage : DisabledImage;
		}
	}

	public class SoundLevelButton : ImageButton
	{
		private float _level;
		protected Brush _brush;

		public SoundLevelButton()
			: base()
		{
			//SetStyle(ControlStyles.UserPaint, true);
		}
		public float DetectedLevel
		{
			get { return _level; }
			set
			{
//                if (_brush != null)
//                    _brush.Dispose();
//                //                int x = 215 - (int)(_level * 100);
				//                _brush = new SolidBrush(Color.FromArgb(x, 2, 0));
//                int x = (int)(_level * 100);
//                _brush = new SolidBrush(Color.FromArgb(215, 2 + x, +x));
//                _level = value;

				Invalidate();
			}
		}

//        protected override void OnPaint(PaintEventArgs e)
//        {
//            base.OnPaint(e);
//            //pevent.Graphics.FillEllipse(_brush, pevent.ClipRectangle.Left+5, pevent.ClipRectangle.Top, 32,32);
//
//        }
	}
}
