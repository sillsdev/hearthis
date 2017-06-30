using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
	/// when closed (including saving them and all settings permanently).
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

			BackColor = AppPallette.Background;
			pictureBox1.Image = AppPallette.ActorCharacterImage;
			pictureBox2.Image = AppPallette.CharactersImage;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, AppPallette.ScriptFocusTextColor, ButtonBorderStyle.Solid);
		}

		/// <summary>
		/// Because it uses invoke, this should not be called until the control has a handle, typically after it is
		/// added to its container.
		/// </summary>
		public IActorCharacterProvider ActorCharacterProvider
		{
			get { return _actorCharacterProvider; }
			set
			{
				_actorCharacterProvider = value;
				_okButton.Enabled = false;
				Cursor = Cursors.WaitCursor;
				// This is probably overkill...by the time someone brings this up, the full pass will be completed,
				// and updating for a single character is quite fast. But just in case the delay is
				// noticeable in a big project on a slow machine, we'll show something right away.
				_actorCharacterProvider.DoWhenFullyRecordedCharactersAvailable((fullyRecorded) =>
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
				var item = new CheckableItem() { Text = actor, Checked = allRecorded };
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
			var actor = ((CheckableItem)_actorList.SelectedItem).Text;
			var characters = _actorCharacterProvider.GetCharacters((string)actor);
			_characterList.Items.Clear();
			bool gotSelection = false;
			foreach (var character in characters)
			{
				var item = new CheckableItem() {Text = character, Checked = _fullyRecorded.AllRecorded(actor, character)};
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

		private void _okButton_Click(object sender, EventArgs e)
		{
			if (_actorList.SelectedIndex == 0)
			{
				Settings.Default.Actor = null;
				Settings.Default.Character = null;
			}
			else
			{
				Settings.Default.Actor = ((CheckableItem)_actorList.SelectedItem).Text;
				if (_characterList.SelectedItem != null)
					Settings.Default.Character = ((CheckableItem)_characterList.SelectedItem).Text;
				else
					Settings.Default.Character = null; // not sure this can happen, playing safe.
			}
			_actorCharacterProvider.RestrictToCharacter(Settings.Default.Actor, Settings.Default.Character);
			Finish();
		}

		private void Finish()
		{
			Settings.Default.Save();
			Parent.Controls.Remove(this);
			Closed?.Invoke(this, new EventArgs());
		}

		public const string LeadingCheck = "\x2713  "; // check mark followed by two spaces
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
