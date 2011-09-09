using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace HearThis.UI
{
	public static class AppPallette
	{
		public static Color Red = Color.FromArgb(215, 2, 0);
		public static Color Blue = Color.FromArgb(32, 74, 135);
		public static Color Orange = Color.FromArgb(255, 168, 0);
		public static Color DarkGray = Color.FromArgb(115, 115, 115);
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
