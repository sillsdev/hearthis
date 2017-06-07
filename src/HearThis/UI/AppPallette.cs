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
using System.Linq;
using HearThis.Properties;

namespace HearThis.UI
{
	public static class AppPallette
	{

		private static Dictionary<string, Dictionary<string, Color>> ColorSchemes = new Dictionary<string, Dictionary<string, Color>>
		{
			{
				"Normal", new Dictionary<string, Color>
				{
					{"Background", Color.FromArgb(65,65,65) },
					{"MouseOverButtonBackColor", Color.FromArgb(78,78,78) },
					{"NavigationTextColor", Color.FromArgb(200,200,200) },
					{"ScriptFocusTextColor", Color.FromArgb(252,202,1) },
					{"ScriptContextTextColor", Color.FromArgb(200,200,200) },
					{"EmptyBoxColor", Color.FromArgb(95,95,95) },
					{"HilightColor", Color.FromArgb(145,58,27) },
					{"SecondPartTextColor", Color.FromArgb(206,83,38) },
					{"SkippedLineColor", Color.FromArgb(166,132,0) },
					{"Red", Color.FromArgb(215,2,0) },
					{"Blue", Color.FromArgb(35,38,83) },
					{"Green", Color.FromArgb(57,165,0) },
					{"Titles", Color.DarkGray }

				}
			},
			{
				"High Contrast", new Dictionary<string, Color>
				{
					{"Background", Color.FromArgb(0,0,0) },
					{"MouseOverButtonBackColor", Color.FromArgb(0,0,0) },
					{"NavigationTextColor", Color.FromArgb(255, 255, 255) },
					{"ScriptFocusTextColor", Color.FromArgb(0,255,0) },
					{"ScriptContextTextColor", Color.FromArgb(255,255,255) },
					{"EmptyBoxColor", Color.FromArgb(255,255,255) },
					{"HilightColor", Color.FromArgb(0,255,0) },
					{"SecondPartTextColor", Color.FromArgb(0,255,0) },
					{"SkippedLineColor", Color.FromArgb(0,255,0) },
					{"Red", Color.FromArgb(215,2,0) },
					{"Blue", Color.FromArgb(0,0,255) },
					{"Green", Color.FromArgb(57,165,0) },
					{"Titles", Color.DarkGray }
				}
			}

		};

		public static string CurrentColorScheme
		{
			get { return Settings.Default.UserColorScheme;  }
		}

		public static string[] AvailableColorSchemes
		{
			get
			{
				return ColorSchemes.Keys.ToArray();
			}
		}

		public static Color Background
		{
			get { return ColorSchemes[CurrentColorScheme]["Background"]; }
		}

		public static Color MouseOverButtonBackColor
		{
			get { return ColorSchemes[CurrentColorScheme]["MouseOverButtonBackColor"]; }
		}

		public static Color NavigationTextColor
		{
			get { return ColorSchemes[CurrentColorScheme]["NavigationTextColor"]; }
		}

		public static Color ScriptFocusTextColor
		{
			get { return ColorSchemes[CurrentColorScheme]["ScriptFocusTextColor"]; }
		}

		public static Color ScriptContextTextColor
		{
			get { return ColorSchemes[CurrentColorScheme]["ScriptContextTextColor"]; }
		}

		public static Color EmptyBoxColor
		{
			get { return ColorSchemes[CurrentColorScheme]["EmptyBoxColor"]; }
		}

		public static Color HilightColor
		{
			get { return ColorSchemes[CurrentColorScheme]["HilightColor"]; }
		}

		public static Color SecondPartTextColor
		{
			get { return ColorSchemes[CurrentColorScheme]["SecondPartTextColor"]; }
		}

		public static Color SkippedLineColor
		{
			get { return ColorSchemes[CurrentColorScheme]["SkippedLineColor"]; }
		}

		public static Color Red
		{
			get { return ColorSchemes[CurrentColorScheme]["Red"]; }
		}

		public static Color Blue
		{
			get { return ColorSchemes[CurrentColorScheme]["Blue"]; }
		}

		public static Color Green
		{
			get { return ColorSchemes[CurrentColorScheme]["Green"]; }
		}

		public static Color TitleColor
		{
			get { return ColorSchemes[CurrentColorScheme]["Titles"]; }
		}

		public static Brush SkippedSegmentBrush = new SolidBrush(SkippedLineColor);
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

		private static Brush _blueBrush;
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
