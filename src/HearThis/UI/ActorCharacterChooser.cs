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

		public IActorCharacterProvider ActorCharacterProvider
		{
			get { return _actorCharacterProvider; }
			set
			{
				_actorCharacterProvider = value;
				var actors = _actorCharacterProvider.Actors;
				_actorList.Items.Add(LocalizationManager.GetString("ActorCharacterChooser.Overview", "Overview", "Item appears at top of list of actors and means to show everything for all actors"));
				_actorList.Items.AddRange(actors.ToArray());
				var currentActor = _actorCharacterProvider.Actor;
				if (currentActor != null && actors.Contains(currentActor)) // paranoia
					_actorList.SelectedItem = currentActor;
				else
					_actorList.SelectedIndex = 0;
			}
		}

		private void ActorListOnSelectedValueChanged(object sender, EventArgs eventArgs)
		{
			if (_actorList.SelectedIndex == 0)
			{
				_characterList.Hide();
				return;
			}
			var actor = _actorList.SelectedItem;
			var characters = _actorCharacterProvider.GetCharacters((string)actor);
			_characterList.Items.Clear();
			_characterList.Items.AddRange(characters.ToArray());
			var currentCharacter = _actorCharacterProvider.Character;
			if (currentCharacter != null && characters.Contains(currentCharacter))
				_characterList.SelectedItem = currentCharacter;
			else
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
				Settings.Default.Actor = (string) _actorList.SelectedItem;
				Settings.Default.Character = (string) _characterList.SelectedItem;
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
	}
}
