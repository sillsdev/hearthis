// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2011, SIL International. All Rights Reserved.
// <copyright from='2007' to='2011' company='SIL International'>
//		Copyright (c) 2011, SIL International. All Rights Reserved.
//
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
#endregion
//
// File: SantaFeFocusMessageHandler.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace HearThis.Script
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Handles Santa Fe synchronized scrolling/focus messages, as supported by TE, Paratext, maybe others.
	/// This is a slightly-simplified version of FieldWorks SharedScrControls\SantaFeFocusMessageHandler.cs
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public static class SantaFeFocusMessageHandler
	{
		#region Windows API methods
		/// <summary>The RegisterWindowMessage function defines a new window message that is
		/// guaranteed to be unique throughout the system. The message value can be used when
		/// sending or posting messages.</summary>
		/// <param name="name">unique name of a message</param>
		/// <returns>message identifier in the range 0xC000 through 0xFFFF, or 0 if an error
		/// occurs</returns>
#if !__MonoCS__
		[DllImport("User32.dll")]
		private static extern uint RegisterWindowMessage(string name);
#else
		private static uint RegisterWindowMessage(string name)
		{
			// TODO-Linux: Deal with this somehow see BasicUtils
			return 0;
		}
#endif

		/// <summary></summary>
#if !__MonoCS__
		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool PostMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);
#else
		private static bool PostMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam)
		{
			// TODO-Linux: Deal with this somehow see BasicUtils
			return false;
		}
#endif
		#endregion

		/// ----------------------------------------------------------------------------------------
		/// <summary>
		/// Types of "focus sharing" supported by HearThis (must match SanatFe spec)
		/// </summary>
		/// ----------------------------------------------------------------------------------------
		private enum FocusTypes
		{
			/// <summary></summary>
			ScriptureReferenceFocus = 1,
		}

		/// <summary>
		/// The registry key for synchronizing apps to a Scripture reference (must match SanatFe spec)
		/// </summary>
		private static readonly RegistryKey s_SantaFeRefKey =
			Registry.CurrentUser.CreateSubKey(@"Software\SantaFe\Focus\ScriptureReference");

		/// <summary>
		/// The Windows message used for synchronized scrolling (must match SanatFe spec)
		/// </summary>
		private static int s_FocusMsg = (int)RegisterWindowMessage("SantaFeFocus");

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Notify all Santa Fe windows that a Scripture Reference focus change has occured.
		/// </summary>
		/// <param name="sRef">The string representation of the reference (e.g. MAT 1:1)</param>
		/// ------------------------------------------------------------------------------------
		public static void SendFocusMessage(string sRef)
		{
			//BCVRef bcvRef = new BCVRef(sRef);
			//if (!bcvRef.Valid)
			//    return;
			s_SantaFeRefKey.SetValue(null, sRef);
			PostMessage(new IntPtr(-1), s_FocusMsg, (uint)FocusTypes.ScriptureReferenceFocus, 0);
		}
	}
}
