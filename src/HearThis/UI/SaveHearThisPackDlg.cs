// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2023, SIL International. All Rights Reserved.
// <copyright from='2017' to='2023' company='SIL International'>
//		Copyright (c) 2023, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.Windows.Forms;
using HearThis.Script;
using L10NSharp;

namespace HearThis.UI
{
	/// <summary>
	/// This simple dialog provides a brief explanation of what HearThisPacks are for
	/// and a home for the control that allows choosing to limit the pack to the current
	/// actor.
	/// </summary>
	public partial class SaveHearThisPackDlg : Form, ILocalizable
	{
		private readonly string _actor;

		public SaveHearThisPackDlg(bool isMultiVoiceProject, string actor)
		{
			_actor = actor;

			InitializeComponent();
			_lblAboutRestrictToCharacter.Visible = isMultiVoiceProject;
			_limitToCurrentActor.Visible = !string.IsNullOrEmpty(_actor);

			Program.RegisterLocalizable(this);
			HandleStringsLocalized();
		}

		public void HandleStringsLocalized()
		{
			_limitToCurrentActor.Text = string.Format(_limitToCurrentActor.Text,
				MultiVoiceScriptProvider.GetActorNameForUI(_actor));
			_lblAboutHearThisPack.Text = string.Format(_lblAboutHearThisPack.Text, Program.kProduct);
		}

		public bool LimitToActor => _limitToCurrentActor.Checked;
	}
}
