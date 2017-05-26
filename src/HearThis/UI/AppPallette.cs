// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2014, SIL International. All Rights Reserved.
// <copyright from='2011' to='2014' company='SIL International'>
//		Copyright (c) 2014, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace HearThis.UI
{
	public static class AppPallette
	{

		public static Dictionary<string, Dictionary<string, Color>> ColorSchemes = new Dictionary<string, Dictionary<string, Color>>
		{
			{
				"Dark", new Dictionary<string, Color>
				{
					{"Background", Color.FromArgb(65,65,65) }
				}
			},
			{
				"Light", new Dictionary<string, Color>
				{
					{"Background", Color.FromArgb(255,168,0) }
				}
			},

		};

		public static string CurrentColorScheme
		{
			get { return "Dark";  }
		}
		//public static Color Background = Color.FromArgb(65,65,65);
		public static Color Background
		{
			get { return ColorSchemes[CurrentColorScheme]["Background"]; }
		}
		public static Color MouseOverButtonBackColor = Color.FromArgb(78, 78, 78);
		public static Color NavigationTextColor = Color.FromArgb(200,200,200);
		public static Color ScriptFocusTextColor = Color.FromArgb(252, 202, 1);//242, 242, 242);
		public static Color ScriptContextTextColor = NavigationTextColor;
		public static Color EmptyBoxColor = Color.FromArgb(95,95,95);
		public static Color HilightColor = Color.FromArgb(145, 58, 27);
		public static Color SecondPartTextColor = Color.FromArgb(206, 83, 38); // we had wanted to use HilightColor, but it's not readable. We could change HilightColor to match this
		public static Color SkippedLineColor = Color.FromArgb(166, 132, 0);//242, 242, 242);
		public static Brush SkippedSegmentBrush = new SolidBrush(SkippedLineColor);

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

		public static Pen ButtonMouseOverPen = new Pen(ScriptFocusTextColor, 3);
		public static Pen ButtonSuggestedPen = new Pen(ScriptFocusTextColor, 2);
		public static Brush ButtonRecordingBrush = new SolidBrush(Green);
		public static Brush ButtonWaitingBrush = new SolidBrush(Red);

		public static Brush ObfuscatedTextContextBrush = new SolidBrush(ControlPaint.Light(Background,(float) .3));
		public static Brush ScriptContextTextBrush = new SolidBrush(ScriptContextTextColor);

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
