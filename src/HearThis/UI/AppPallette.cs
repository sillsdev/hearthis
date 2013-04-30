using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace HearThis.UI
{
	public static class AppPallette
	{
		public static Color Background = Color.FromArgb(65,65,65);
		public static Color NavigationTextColor = Color.FromArgb(200,200,200);
		public static Color EmptyBoxColor = Color.FromArgb(95,95,95);
		public static Color HilightColor = Color.FromArgb(145, 58, 27);

		public static Color Red = Color.FromArgb(215, 2, 0);
		public static Color Blue = Color.FromArgb(35,38,83);//(32, 74, 135);
		//public static Color Orange = Color.FromArgb(255, 168, 0);
		public static Color Green = Color.FromArgb(57,165,0);
		//public static Color DarkGray = Color.FromArgb(175, 175, 175);
		private static Brush _blueBrush;
		public static Pen PartialProgressPen = new Pen(EmptyBoxColor, 3);
		public static Pen CompleteProgressPen =new Pen(HilightColor, 2);
		public static Brush DisabledBrush = new SolidBrush(EmptyBoxColor);
		public static Brush BackgroundBrush = new SolidBrush(Background);

		public static Pen ButtonMouseOverPen = new Pen(HilightColor, 5);
		public static Pen ButtonSuggestedPen = new Pen(HilightColor, 2);
		public static Brush ButtonRecordingBrush = new SolidBrush(Green);
		public static Brush ButtonWaitingBrush = new SolidBrush(Red);


		public static Brush BlueBrush
		{
			get
			{
				if(_blueBrush==null)
				{
					_blueBrush = new SolidBrush(Blue);
				}
				return _blueBrush;
			}
		}


	}
}