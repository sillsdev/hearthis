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
		}

		public IActorCharacterProvider ActorCharacterProvider
		{
			get { return _actorCharacterProvider; }
			set
			{
				_actorCharacterProvider = value;
				var actors = _actorCharacterProvider.Actors;
				_actorList.Items.AddRange(actors.ToArray());
				if (Settings.Default.Actor != null && actors.Contains(Settings.Default.Actor)) // might have been saved from another project
					_actorList.SelectedItem = Settings.Default.Actor;
				else
					_actorList.SelectedIndex = 0;
			}
		}

		private void ActorListOnSelectedValueChanged(object sender, EventArgs eventArgs)
		{
			var actor = _actorList.SelectedItem;
			var characters = _actorCharacterProvider.GetCharacters((string)actor);
			_characterList.Items.Clear();
			_characterList.Items.AddRange(characters.ToArray());
			if (Settings.Default.Character != null && characters.Contains(Settings.Default.Character))
				_characterList.SelectedItem = Settings.Default.Character;
			else
				_characterList.SelectedIndex = 0;
		}

		private void _okButton_Click(object sender, EventArgs e)
		{
			Settings.Default.Actor = (string) _actorList.SelectedItem;
			Settings.Default.Character = (string) _characterList.SelectedItem;
			Settings.Default.Save();
			Parent.Controls.Remove(this);
			Closed?.Invoke(this, new EventArgs());
		}
	}
}
