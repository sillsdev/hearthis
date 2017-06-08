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
using L10NSharp;

namespace HearThis.UI
{
	public static class AppPallette
	{
		public static string ColorSchemeNormal = "Normal";
		public static string ColorSchemeHighContrast = "High Contrast";
		public class ColorSchemeName
		{
			private string _englishName;
			public ColorSchemeName(string englishName)
			{
				_englishName = englishName;
			}

			public string EnglishName
			{
				get { return _englishName; }
			}

			override public string ToString()
			{
				return LocalizationManager.GetString("AppPallette." + _englishName, _englishName);
			}
		}

		public enum ColorSchemeElement
		{
			Background,
			MouseOverButtonBackColor,
			NavigationTextColor,
			ScriptFocusTextColor,
			ScriptContextTextColor,
			EmptyBoxColor,
			HilightColor,
			SecondPartTextColor,
			SkippedLineColor,
			Red,
			Blue,
			Green,
			Titles
		}

		private static Dictionary<string, Dictionary<ColorSchemeElement, Color>> ColorSchemes = new Dictionary<string, Dictionary<ColorSchemeElement, Color>>
		{
			{
				ColorSchemeNormal, new Dictionary<ColorSchemeElement, Color>
				{
					{ColorSchemeElement.Background , Color.FromArgb(65,65,65) },
					{ColorSchemeElement.MouseOverButtonBackColor, Color.FromArgb(78,78,78) },
					{ColorSchemeElement.NavigationTextColor, Color.FromArgb(200,200,200) },
					{ColorSchemeElement.ScriptFocusTextColor, Color.FromArgb(252,202,1) },
					{ColorSchemeElement.ScriptContextTextColor, Color.FromArgb(200,200,200) },
					{ColorSchemeElement.EmptyBoxColor, Color.FromArgb(95,95,95) },
					{ColorSchemeElement.HilightColor, Color.FromArgb(145,58,27) },
					{ColorSchemeElement.SecondPartTextColor, Color.FromArgb(206,83,38) },
					{ColorSchemeElement.SkippedLineColor, Color.FromArgb(166,132,0) },
					{ColorSchemeElement.Red, Color.FromArgb(215,2,0) },
					{ColorSchemeElement.Blue, Color.FromArgb(35,38,83) },
					{ColorSchemeElement.Green, Color.FromArgb(57,165,0) },
					{ColorSchemeElement.Titles, Color.DarkGray }

				}
			},
			{
				ColorSchemeHighContrast, new Dictionary<ColorSchemeElement, Color>
				{
					{ColorSchemeElement.Background, Color.FromArgb(0,0,0) },
					{ColorSchemeElement.MouseOverButtonBackColor, Color.FromArgb(0,0,0) },
					{ColorSchemeElement.NavigationTextColor, Color.FromArgb(255, 255, 255) },
					{ColorSchemeElement.ScriptFocusTextColor, Color.FromArgb(0,255,0) },
					{ColorSchemeElement.ScriptContextTextColor, Color.FromArgb(255,255,255) },
					{ColorSchemeElement.EmptyBoxColor, Color.FromArgb(255,255,255) },
					{ColorSchemeElement.HilightColor, Color.FromArgb(0,255,0) },
					{ColorSchemeElement.SecondPartTextColor, Color.FromArgb(0,255,0) },
					{ColorSchemeElement.SkippedLineColor, Color.FromArgb(0,255,0) },
					{ColorSchemeElement.Red, Color.FromArgb(215,2,0) },
					{ColorSchemeElement.Blue, Color.FromArgb(0,0,255) },
					{ColorSchemeElement.Green, Color.FromArgb(57,165,0) },
					{ColorSchemeElement.Titles, Color.DarkGray }
				}
			}

		};

		public static string CurrentColorScheme
		{
			get
			{
				string setScheme = Settings.Default.UserColorScheme;
				if (ColorSchemes.ContainsKey(setScheme))
				{
					return setScheme;
				}
				else
				{
					return ColorSchemeNormal;
				}
			}
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
			get { return ColorSchemes[CurrentColorScheme][ColorSchemeElement.Background]; }
		}

		public static Color MouseOverButtonBackColor
		{
			get { return ColorSchemes[CurrentColorScheme][ColorSchemeElement.MouseOverButtonBackColor]; }
		}

		public static Color NavigationTextColor
		{
			get { return ColorSchemes[CurrentColorScheme][ColorSchemeElement.NavigationTextColor]; }
		}

		public static Color ScriptFocusTextColor
		{
			get { return ColorSchemes[CurrentColorScheme][ColorSchemeElement.ScriptFocusTextColor]; }
		}

		public static Color ScriptContextTextColor
		{
			get { return ColorSchemes[CurrentColorScheme][ColorSchemeElement.ScriptContextTextColor]; }
		}

		public static Color EmptyBoxColor
		{
			get { return ColorSchemes[CurrentColorScheme][ColorSchemeElement.EmptyBoxColor]; }
		}

		public static Color HilightColor
		{
			get { return ColorSchemes[CurrentColorScheme][ColorSchemeElement.HilightColor]; }
		}

		public static Color SecondPartTextColor
		{
			get { return ColorSchemes[CurrentColorScheme][ColorSchemeElement.SecondPartTextColor]; }
		}

		public static Color SkippedLineColor
		{
			get { return ColorSchemes[CurrentColorScheme][ColorSchemeElement.SkippedLineColor]; }
		}

		public static Color Red
		{
			get { return ColorSchemes[CurrentColorScheme][ColorSchemeElement.Red]; }
		}

		public static Color Blue
		{
			get { return ColorSchemes[CurrentColorScheme][ColorSchemeElement.Blue]; }
		}

		public static Color Green
		{
			get { return ColorSchemes[CurrentColorScheme][ColorSchemeElement.Green]; }
		}

		public static Color TitleColor
		{
			get { return ColorSchemes[CurrentColorScheme][ColorSchemeElement.Titles]; }
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
