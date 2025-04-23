// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2017-2025, SIL Global.
// <copyright from='2017' to='2025' company='SIL Global'>
//		Copyright (c) 2017-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HearThis.Script;
using HearThis.Properties;
using L10NSharp;

namespace HearThis.UI
{
	/// <summary>
	/// This control allows choosing which actor and character HearThis is currently supposed to be recording.
	/// It is designed to be used just once: create one and add it to the appropriate parent. When OK is clicked,
	/// it will raise the Closed event and remove itself from its parent's controls.
	/// It initializes itself from Settings.Default.Actor and Settings.Default.Character, and updates those
	/// when closed (including saving them and all settings permanently). TODO: We should migrate actor and
	/// character to project settings since they are really project-specific. But the code handles the case
	/// where the actor or character isn't found and probably very few users will be switching between different
	/// multi-voice projects, so it's not super critical.
	/// </summary>
	public partial class ActorCharacterChooser : UserControl
	{
		private IActorCharacterProvider _actorCharacterProvider;
		private FullyRecordedStatus _fullyRecorded;

		public event EventHandler<EventArgs> Closed;

		public ActorCharacterChooser()
		{
			InitializeComponent();
			_actorList.SelectedValueChanged += ActorListOnSelectedValueChanged;

			BackColor = AppPalette.Background;
			pictureBox1.Image = AppPalette.ActorCharacterImage;
			pictureBox2.Image = AppPalette.CharactersImage;
			_characterList.BackColor = BackColor;
			_actorList.BackColor = BackColor;
			_actorList.DrawMode = DrawMode.OwnerDrawFixed;
			_characterList.DrawMode = DrawMode.OwnerDrawFixed;
			_actorList.DrawItem += DrawListItem;
			_characterList.DrawItem += DrawListItem;
		}

		// Thanks to https://stackoverflow.com/questions/13675006/change-the-selected-color-of-a-listbox-in-winforms
		private void DrawListItem(object sender, DrawItemEventArgs e)
		{
			e.DrawBackground();
			Graphics g = e.Graphics;
			var selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
			Brush brush = selected ?
				AppPalette.HighlightBrush : AppPalette.BackgroundBrush;
			g.FillRectangle(brush, e.Bounds);
			e.Graphics.DrawString(((ListBox)sender).Items[e.Index].ToString(), e.Font,
				selected ? Brushes.Black : Brushes.White,
				e.Bounds, StringFormat.GenericDefault);
			e.DrawFocusRectangle();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			ControlPaint.DrawBorder(e.Graphics, ClientRectangle, AppPalette.ScriptFocusTextColor, ButtonBorderStyle.Solid);
		}

		/// <summary>
		/// Because it uses invoke, this should not be called until the control has a handle, typically after it is
		/// added to its container.
		/// </summary>
		public IActorCharacterProvider ActorCharacterProvider
		{
			get => _actorCharacterProvider;
			set
			{
				_actorCharacterProvider = value;
				_okButton.Enabled = false;
				Cursor = Cursors.WaitCursor;
				// This is probably overkill...by the time someone brings this up, the full pass will be completed,
				// and updating for a single character is quite fast. But just in case the delay is
				// noticeable in a big project on a slow machine, we'll show something right away.
				_actorCharacterProvider.DoWhenFullyRecordedCharactersAvailable(fullyRecorded =>
				{
					_fullyRecorded = fullyRecorded;
					Invoke((Action) (() =>
					{
						InitializeLists();
						_okButton.Enabled = true;
						Cursor = Cursors.Default;
					}));
				});
			}
		}

		void InitializeLists()
		{
			var actors = _actorCharacterProvider.Actors.ToArray();
			_actorList.Items.Add(OverviewLabel);
			var currentActor = _actorCharacterProvider.Actor;
			bool gotSelection = false;
			foreach (string actor in actors)
			{
				var allRecorded = _fullyRecorded.AllRecorded(actor);
				var item = new CheckableItem
				{
					Text = MultiVoiceScriptProvider.GetActorNameForUI(actor),
					Checked = allRecorded
				};
				_actorList.Items.Add(item);
				if (actor == currentActor)
				{
					_actorList.SelectedItem = item;
					gotSelection = true;
				}
			}
			if (!gotSelection)
				_actorList.SelectedIndex = 0;
		}

		public static string OverviewLabel { get; } = LocalizationManager.GetString("ActorCharacterChooser.Overview", "Overview", "Item appears at top of list of actors and means to show everything for all actors");

		private void ActorListOnSelectedValueChanged(object sender, EventArgs eventArgs)
		{
			if (_actorList.SelectedIndex == 0)
			{
				_characterList.Hide();
				return;
			}

			var actor = GetSelectedActor();
			var characters = _actorCharacterProvider.GetCharacters(actor);
			_characterList.Items.Clear();
			bool gotSelection = false;
			foreach (var character in characters)
			{
				var item = new CheckableItem {Text = character, Checked = _fullyRecorded.AllRecorded(actor, character)};
				_characterList.Items.Add(item);
				if (character == _actorCharacterProvider.Character)
				{
					_characterList.SelectedItem = item;
					gotSelection = true;
				}
			}
			if (!gotSelection)
				_characterList.SelectedIndex = 0;
			_characterList.Show();
		}

		/// <summary>
		/// Gets the name of the selected actor. This is name in the data, NOT the localized
		/// version (in the case of "unassigned").
		/// </summary>
		/// <returns></returns>
		private string GetSelectedActor()
		{
			var actor = ((CheckableItem)_actorList.SelectedItem).Text;
			if (actor == MultiVoiceScriptProvider.GetActorNameForUI(MultiVoiceScriptProvider.kUnassignedActorName))
				actor = MultiVoiceScriptProvider.kUnassignedActorName;
			return actor;
		}

		private void _okButton_Click(object sender, EventArgs e)
		{
			if (_actorList.SelectedIndex == 0)
			{
				Settings.Default.Actor = null;
				Settings.Default.Character = null;
			}
			else
			{
				Settings.Default.Actor = GetSelectedActor();
				// Not sure if the selected item can ever be null, but playing it safe.
				Settings.Default.Character = (_characterList.SelectedItem as CheckableItem)?.Text;
			}
			_actorCharacterProvider.RestrictToCharacter(Settings.Default.Actor, Settings.Default.Character);
			Finish();
		}

		private void Finish()
		{
			FileContentionHelper.SaveSettings();
			Parent.Controls.Remove(this);
			Closed?.Invoke(this, new EventArgs());
		}

		public const string LeadingCheck = "\x2714  "; // check mark followed by two spaces
	}

	class CheckableItem
	{
		public string Text;
		public bool Checked;
		public override string ToString()
		{
			return (Checked ?  ActorCharacterChooser.LeadingCheck : "") + Text;
		}
	}
}
