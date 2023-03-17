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

namespace HearThis.UI
{
	/// <summary>
	/// This simple dialog provides a brief explanation of what HearThisPacks are for
	/// and a home for the control that allows choosing to limit the pack to the current
	/// actor.
	/// </summary>
	public partial class SaveHearThisPackDlg : Form, ILocalizable
	{
		private readonly IActorCharacterProvider _multiVoiceProvider;

		public SaveHearThisPackDlg(IActorCharacterProvider multiVoiceProvider)
		{
			_multiVoiceProvider = multiVoiceProvider;
			InitializeComponent();
			_lblAboutRestrictToCharacter.Visible = multiVoiceProvider != null;
			_limitToCurrentActor.Visible = !string.IsNullOrEmpty(multiVoiceProvider?.Actor);

			Program.RegisterLocalizable(this);
			HandleStringsLocalized();
		}

		public void HandleStringsLocalized()
		{
			if (_limitToCurrentActor.Visible)
			{
				if (_limitToCurrentActor.Text.Contains("{0}"))
				{
					_limitToCurrentActor.Tag = _limitToCurrentActor.Text;
					_limitToCurrentActor.Text = string.Format(_limitToCurrentActor.Text,
						_multiVoiceProvider.ActorForUI);
				}
				else if (_limitToCurrentActor.Tag is string fmt)
				{
					_limitToCurrentActor.Text = string.Format(fmt, _multiVoiceProvider.ActorForUI);
				}
			}

			_lblAboutHearThisPack.Text = string.Format(_lblAboutHearThisPack.Text, Program.kProduct);
		}

		public bool LimitToActor => _limitToCurrentActor.Checked;
	}
}
