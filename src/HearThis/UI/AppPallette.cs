// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International. All Rights Reserved.
// <copyright from='2011' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using HearThis.Properties;
using L10NSharp;

namespace HearThis.UI
{
	public enum ColorScheme
	{
		Normal,
		HighContrast
	}

	public static class ColorSchemeExtensions
	{
		public static string ToLocalizedString(this ColorScheme colorScheme)
		{
			switch (colorScheme)
			{
				case ColorScheme.Normal:
					return LocalizationManager.GetString("ColorScheme.Normal", "Normal");
				case ColorScheme.HighContrast:
					return LocalizationManager.GetString("ColorScheme.HighContrast", "High Contrast");
				default:
					return null;
			}
		}
	}

	public static class AppPallette
	{
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
			Recording,
			Titles,
			ActorCharacterIcon,
			CharactersIcon,
			ScriptContextTextColorDuringRecording,
		}

		// all toolbar button images need to be this color
		public static Color CommonMuted = Color.FromArgb(192,192,192);

		private static Color NormalBackground = Color.FromArgb(65, 65, 65);
		private static Color NormalHighlight = Color.FromArgb(245,212,17);
		private static Color HighContrastHighlight = Color.FromArgb(0,255,0);

		private static readonly Dictionary<ColorScheme, Dictionary<ColorSchemeElement, Color>> ColorSchemes = new Dictionary<ColorScheme, Dictionary<ColorSchemeElement, Color>>
		{
			{
				ColorScheme.Normal, new Dictionary<ColorSchemeElement, Color>
				{
					{ColorSchemeElement.Background, NormalBackground },
					{ColorSchemeElement.MouseOverButtonBackColor, Color.FromArgb(78,78,78) },
					{ColorSchemeElement.NavigationTextColor, CommonMuted },
					{ColorSchemeElement.ScriptFocusTextColor, NormalHighlight },
					{ColorSchemeElement.ScriptContextTextColor, CommonMuted },
					{ColorSchemeElement.EmptyBoxColor, CommonMuted },
					{ColorSchemeElement.HilightColor, NormalHighlight },
					{ColorSchemeElement.SecondPartTextColor, HighContrastHighlight },
					{ColorSchemeElement.SkippedLineColor, Color.FromArgb(166,132,0) }, //review
					{ColorSchemeElement.Red, Color.FromArgb(215,2,0) },
					{ColorSchemeElement.Blue, Color.FromArgb(00,8,118) },
					{ColorSchemeElement.Recording, Color.FromArgb(57,165,0) },
					{ColorSchemeElement.Titles, CommonMuted},
					{ColorSchemeElement.ScriptContextTextColorDuringRecording, ControlPaint.Light(NormalBackground,(float) .15)},
				}
			},
			{
				ColorScheme.HighContrast, new Dictionary<ColorSchemeElement, Color>
				{
					{ColorSchemeElement.Background, Color.FromArgb(0,0,0) },
					{ColorSchemeElement.MouseOverButtonBackColor, Color.FromArgb(0,0,0) },
					{ColorSchemeElement.NavigationTextColor, CommonMuted },
					{ColorSchemeElement.ScriptFocusTextColor, HighContrastHighlight },
					{ColorSchemeElement.ScriptContextTextColor, CommonMuted },
					{ColorSchemeElement.EmptyBoxColor, CommonMuted },
					{ColorSchemeElement.HilightColor, HighContrastHighlight },
					{ColorSchemeElement.SecondPartTextColor, NormalHighlight},
					{ColorSchemeElement.SkippedLineColor, Color.FromArgb(186,186,0) },  //review
					{ColorSchemeElement.Red, Color.FromArgb(255,0,0) },
					{ColorSchemeElement.Blue, Color.FromArgb(0,0,255) },
					{ColorSchemeElement.Recording, Color.FromArgb(0,255,0) },
					{ColorSchemeElement.Titles, CommonMuted },
					{ColorSchemeElement.ScriptContextTextColorDuringRecording, Color.FromArgb(100,100,100)},
				}
			}
		};

		public static readonly Dictionary<ColorScheme, Dictionary<ColorSchemeElement, Image>> ColorSchemeIcons = new Dictionary<ColorScheme, Dictionary<ColorSchemeElement, Image>>
		{
			{
				ColorScheme.Normal, new Dictionary<ColorSchemeElement, Image>
				{
					{ColorSchemeElement.ActorCharacterIcon, Resources.speakIntoMike75x50 },
					{ColorSchemeElement.CharactersIcon, Resources.characters }

				}
			},
			{
				ColorScheme.HighContrast, new Dictionary<ColorSchemeElement, Image>
				{
					{ColorSchemeElement.ActorCharacterIcon, Resources.speakIntoMike75x50HC },
					{ColorSchemeElement.CharactersIcon, Resources.charactersHC }
				}
			}
		};

		public static ColorScheme CurrentColorScheme
		{
			get
			{
				var setScheme = Settings.Default.UserColorScheme;
				if (ColorSchemes.ContainsKey(setScheme))
				{
					return setScheme;
				}
				return ColorScheme.Normal;
			}
		}

		public static IEnumerable<KeyValuePair<ColorScheme, string>> AvailableColorSchemes
		{
			get
			{
				foreach (var colorScheme in Enum.GetValues(typeof(ColorScheme)).Cast<ColorScheme>())
				{
					yield return new KeyValuePair<ColorScheme, string>(colorScheme, colorScheme.ToLocalizedString());
				}
			}
		}

		public static Image CharactersImage
		{
			get { return ColorSchemeIcons[CurrentColorScheme][ColorSchemeElement.CharactersIcon]; }
		}

		public static Image ActorCharacterImage
		{
			get { return ColorSchemeIcons[CurrentColorScheme][ColorSchemeElement.ActorCharacterIcon]; }
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

		public static Color FaintScriptFocusTextColor
		{
			get
			{
				var focusColor = ColorSchemes[CurrentColorScheme][ColorSchemeElement.ScriptFocusTextColor];
				return Color.FromArgb(128, focusColor.R, focusColor.G, focusColor.B);
			}
		}

		public static Color ScriptContextTextColor
		{
			get { return ColorSchemes[CurrentColorScheme][ColorSchemeElement.ScriptContextTextColor]; }
		}

		public static Color ScriptContextTextColorDuringRecording
		{
			get { return ColorSchemes[CurrentColorScheme][ColorSchemeElement.ScriptContextTextColorDuringRecording]; }
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

		public static Color Recording
		{
			get { return ColorSchemes[CurrentColorScheme][ColorSchemeElement.Recording]; }
		}

		public static Color TitleColor
		{
			get { return ColorSchemes[CurrentColorScheme][ColorSchemeElement.Titles]; }
		}

		public static Pen CompleteProgressPen =new Pen(HilightColor, 2);
		public static Brush DisabledBrush = new SolidBrush(EmptyBoxColor);
		public static Brush BackgroundBrush = new SolidBrush(Background);

		public static Pen ButtonMouseOverPen = new Pen(ScriptFocusTextColor, 3);
		public static Pen ButtonSuggestedPen = new Pen(ScriptFocusTextColor, 2);
		public static Brush ButtonRecordingBrush = new SolidBrush(Recording);
		public static Brush ButtonWaitingBrush = new SolidBrush(Red);

		private static Brush _blueBrush;
		public static Brush BlueBrush => _blueBrush ?? (_blueBrush = new SolidBrush(Blue));

		private static Brush _emptyBoxBrush;
		public static Brush EmptyBoxBrush => _emptyBoxBrush ?? (_emptyBoxBrush = new SolidBrush(EmptyBoxColor));

		static Brush _highlightBrush;
		public static Brush HighlightBrush => _highlightBrush ?? (_highlightBrush = new SolidBrush(HilightColor));
	}
}
