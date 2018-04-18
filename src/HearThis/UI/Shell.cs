// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2015, SIL International. All Rights Reserved.
// <copyright from='2011' to='2015' company='SIL International'>
//		Copyright (c) 2015, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using HearThis.Communication;
using HearThis.Properties;
using HearThis.Publishing;
using HearThis.Script;
using L10NSharp;
using NetSparkle;
using SIL.IO;
using SIL.Windows.Forms.Miscellaneous;
using SIL.Windows.Forms.ReleaseNotes;
using Paratext.Data;
using SIL.DblBundle.Text;
using SIL.Reporting;

namespace HearThis.UI
{
	public partial class Shell : Form
	{
		public static Sparkle UpdateChecker;
		public event EventHandler OnProjectChanged;
		private string _projectNameToShow = string.Empty;
		private bool _mouseInMultiVoicePanel;


#if MULTIPLEMODES
		private List<string> allowableModes;

		private const string kAdministrative = "Administrator";
#endif
		private const string kNormalRecording = "NormalRecording";

		public Shell()
		{
			InitializeComponent();
			_toolStrip.BackColor = AppPallette.Background;
			Text = Program.kProduct;

			_settingsProtectionHelper.ManageComponent(_settingsItem);
			_settingsProtectionHelper.ManageComponent(toolStripButtonChooseProject);
			SetupUILanguageMenu();

			_toolStrip.Renderer = new RecordingToolControl.NoBorderToolStripRenderer();

			SetColors();

			InitializeModesCombo();

			// Todo: possibly make this conditional on an a device being connected.
			// If possible notice and show it when a device is later connected.
			// Or: possibly if no device is active it displays instructions.
			_syncWithAndroidItem.Visible = true;
			_toolStrip.Renderer = new ToolStripColorArrowRenderer();
			_multiVoicePanel.MouseLeave += MultiVoicePanelOnMouseTransition;
			_multiVoicePanel.MouseEnter += MultiVoicePanelOnMouseTransition;
			foreach (Control c in _multiVoicePanel.Controls)
			{
				c.MouseEnter += MultiVoicePanelOnMouseTransition;
				c.MouseLeave += MultiVoicePanelOnMouseTransition;
			}
			_multiVoicePanel.Paint += (sender, e) =>
			{
				if (_mouseInMultiVoicePanel && !Controls.OfType<ActorCharacterChooser>().Any())
				{
					var borderRect = _multiVoicePanel.ClientRectangle;
					// The numbers here were determined to line things up with controls below
					borderRect = new Rectangle(borderRect.Left + 16, borderRect.Top, borderRect.Width - 41, borderRect.Height);
					ControlPaint.DrawBorder(e.Graphics, borderRect, AppPallette.FaintScriptFocusTextColor,
						ButtonBorderStyle.Solid);
				}
			};
			_multiVoicePanel.Click += _actorCharacterButton_Click;
		}

		/// <summary>
		/// Unfortunately the mouse 'leaves' the multivoice panel when it enters a child control as well as when it really
		/// leaves the whole panel. So this routine is hooked to happen whenever it leaves or enters any of them.
		/// It figures out whether the mouse is really inside the panel and adjusts the border if this has changed.
		/// </summary>
		/// <param name="sender1"></param>
		/// <param name="eventArgs"></param>
		private void MultiVoicePanelOnMouseTransition(object sender1, EventArgs eventArgs)
		{
			bool isMouseInMVP = _multiVoicePanel.ClientRectangle.Contains(_multiVoicePanel.PointToClient(Control.MousePosition));
			if (isMouseInMVP != _mouseInMultiVoicePanel)
			{
				_mouseInMultiVoicePanel = isMouseInMVP;
				_multiVoicePanel.Invalidate();
			}
		}

		public Project Project { get; private set; }

		private void OnChooseProject(object sender, EventArgs e)
		{
			ChooseProject();
		}

		private bool ChooseProject()
		{
			using (var dlg = new ChooseProject())
			{
				if (DialogResult.OK == dlg.ShowDialog())
				{
					// ENHANCE: Someday it might be nice to save/restore these in a project file so they could be remembered on
					// a per-project basis, but the VAST majority of our users are going to be working on a single project, so
					// this might be good enough.
					Settings.Default.Book = -1;
					Settings.Default.Chapter = -1;
					Settings.Default.Block = -1;
					LoadProject(dlg.SelectedProject);
					return true;
				}
				return false;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			bool loaded = false;
			if (!string.IsNullOrEmpty(Settings.Default.Project))
			{
				loaded = LoadProject(Settings.Default.Project);
			}

			if (!loaded) //if never did have a project, or that project couldn't be loaded
			{
				if (!ChooseProject())
				{
					Close();
					return;
				}
			}

			var savedBounds = Settings.Default.RestoreBounds;
			if ((savedBounds.Width >= MinimumSize.Width) && (savedBounds.Height >= MinimumSize.Height) && (IsOnScreen(savedBounds)))
			{
				StartPosition = FormStartPosition.Manual;
				WindowState = FormWindowState.Normal;
				Bounds = savedBounds;
			}
			else
			{
				StartPosition = FormStartPosition.CenterScreen;
				WindowState = FormWindowState.Maximized;
			}

			UpdateChecker = new Sparkle(@"http://build.palaso.org/guestAuth/repository/download/HearThis_HearThisWinDevPublishPt8/.lastSuccessful/appcast.xml",
				Icon);
			// We don't want to do this until the main window is loaded because a) it's very easy for the user to overlook, and b)
			// more importantly, when the toast notifier closes, it can sometimes clobber an error message being displayed for the user.
			UpdateChecker.CheckOnFirstApplicationIdle();
		}

		/// <summary>
		/// Is a significant (100 x 100) portion of the form on-screen?
		/// </summary>
		/// <returns></returns>
		private static bool IsOnScreen(Rectangle rect)
		{
			var screens = Screen.AllScreens;
			var formTopLeft = new Rectangle(rect.Left, rect.Top, 100, 100);

			return screens.Any(screen => screen.WorkingArea.Contains(formTopLeft));
		}

		private void SetColors()
		{
			_moreMenu.ForeColor = AppPallette.NavigationTextColor;
			_multiVoicePanel.BackColor = AppPallette.Background;
			_multiVoiceMarginPanel.BackColor = _multiVoicePanel.BackColor;
			_actorLabel.ForeColor = AppPallette.ScriptFocusTextColor;
			_characterLabel.ForeColor = AppPallette.ScriptFocusTextColor;
			_actorCharacterButton.BackColor = AppPallette.Background;
			_actorCharacterButton.ForeColor = AppPallette.Background;
			_actorCharacterButton.Image = AppPallette.ActorCharacterImage;

		}

		private void SetupUILanguageMenu()
		{
			_uiLanguageMenu.DropDownItems.Clear();
			foreach (var lang in LocalizationManager.GetUILanguages(true))
			{
				var item = _uiLanguageMenu.DropDownItems.Add(lang.NativeName);
				item.Tag = lang;
				item.Click += ((a, b) =>
				{
					LocalizationManager.SetUILanguage(((CultureInfo) item.Tag).IetfLanguageTag, true);
					Settings.Default.UserInterfaceLanguage = ((CultureInfo) item.Tag).IetfLanguageTag;
					item.Select();
					_uiLanguageMenu.Text = ((CultureInfo) item.Tag).NativeName;
				});
				if (((CultureInfo) item.Tag).IetfLanguageTag == Settings.Default.UserInterfaceLanguage)
				{
					_uiLanguageMenu.Text = ((CultureInfo) item.Tag).NativeName;
				}
			}

			_uiLanguageMenu.DropDownItems.Add(new ToolStripSeparator());
			var menu = _uiLanguageMenu.DropDownItems.Add(LocalizationManager.GetString("MainWindow.MoreMenuItem",
				"More...", "Last item in menu of UI languages"));
			menu.Click += ((a, b) =>
			{
				Program.LocalizationManager.ShowLocalizationDialogBox(false);
				SetupUILanguageMenu();
			});
		}

		private void InitializeModesCombo()
		{
			_btnMode.DropDownItems.Clear();
#if MULTIPLEMODES
			allowableModes = new List<string>();
			if (Settings.Default.AllowAdministrativeMode)
			{
				ToolStripItem item = _btnMode.DropDownItems.Add(LocalizationManager.GetString("MainWindow.Modes.Administrator",
					"Administrator"));
				item.Tag = kAdministrative;
				if (Settings.Default.ActiveMode == kAdministrative)
					SetMode(item);
			}
			if (Settings.Default.AllowNormalRecordingMode)
			{
				ToolStripItem item = _btnMode.DropDownItems.Add(LocalizationManager.GetString("MainWindow.Modes.NormalRecording",
					"Normal Recording"));
				item.Tag = kNormalRecording;
				if (Settings.Default.ActiveMode == kNormalRecording)
					SetMode(item);
			}
#endif
			_btnMode.Visible = (_btnMode.DropDownItems.Count > 1);
			//_recordingToolControl1.HidingSkippedBlocks = Settings.Default.ActiveMode == kNormalRecording; obsolete
			_recordingToolControl1.ShowingSkipButton = Settings.Default.ActiveMode != kNormalRecording;

		}

#if MULTIPLEMODES
		private void SetMode(ToolStripItem selectedMode)
		{
			_btnMode.Text = selectedMode.Text;

			switch ((string)selectedMode.Tag)
			{
				case kAdministrative:
					_recordingToolControl1.HidingSkippedBlocks = false;
					break;
				case kNormalRecording:
					_recordingToolControl1.HidingSkippedBlocks = true;
					break;
			}
			SetWindowText();
		}
#endif

		private void OnSaveClick(object sender, EventArgs e)
		{
			MessageBox.Show(
				LocalizationManager.GetString("MainWindow.SaveAutomatically",
					"HearThis automatically saves your work, while you use it. This button is just here to tell you that :-)  To create sound files for playing your recordings, click the Publish button."),
				LocalizationManager.GetString("Common.Save", "Save"));
		}

		private void OnPublishClick(object sender, EventArgs e)
		{
			using (var dlg = new PublishDialog(Project))
			{
				dlg.ShowDialog();
			}
		}

		private void OnSettingsButtonClicked(object sender, EventArgs e)
		{
			var origBreakQuotesIntoBlocksValue = Project.ProjectSettings.BreakQuotesIntoBlocks;
			var origAdditionalBlockBreakChars = Project.ProjectSettings.AdditionalBlockBreakCharacters;
			var origBreakAtParagraphBreaks = Project.ProjectSettings.BreakAtParagraphBreaks;
			var origDisplayNavigationButtonLabels = Settings.Default.DisplayNavigationButtonLabels;
			DialogResult result = _settingsProtectionHelper.LaunchSettingsIfAppropriate(() =>
			{
				using (var dlg = new AdministrativeSettings(Project))
				{
					return dlg.ShowDialog(FindForm());
				}
			});
			if (result == DialogResult.OK)
			{
				if (origBreakQuotesIntoBlocksValue != Project.ProjectSettings.BreakQuotesIntoBlocks ||
					origAdditionalBlockBreakChars != Project.ProjectSettings.AdditionalBlockBreakCharacters ||
					origBreakAtParagraphBreaks != Project.ProjectSettings.BreakAtParagraphBreaks)
				{
					LoadProject(Settings.Default.Project);
				}
				else
				{
					_recordingToolControl1.SetClauseSeparators(Project.ProjectSettings.ClauseBreakCharacters);
#if MULTIPLEMODES
					Invoke(new Action(InitializeModesCombo));
#else
					if (origDisplayNavigationButtonLabels != Settings.Default.DisplayNavigationButtonLabels)
						Invoke(new Action(() =>
						{
							_recordingToolControl1.Invalidate(true);
						}));

					Invoke(new Action(() =>
					{
						//_recordingToolControl1.HidingSkippedBlocks = Settings.Default.ActiveMode == kNormalRecording; obsolete
						_recordingToolControl1.ShowingSkipButton = Settings.Default.ActiveMode != kNormalRecording;
					}));
#endif
				}
			}
		}

		private void OnAboutClick(object sender, EventArgs e)
		{
			using (var dlg = new SILAboutBox(FileLocator.GetFileDistributedWithApplication("aboutbox.htm")))
			{
				dlg.CheckForUpdatesClicked += HandleAboutDialogCheckForUpdatesClick;
				dlg.ReleaseNotesClicked += HandleAboutDialogReleaseNotesClicked;
				dlg.ShowDialog();
			}
		}

		private void HandleAboutDialogReleaseNotesClicked(object sender, EventArgs e)
		{
			var path = FileLocator.GetFileDistributedWithApplication("ReleaseNotes.md");
			using (var dlg = new ShowReleaseNotesDialog(((Form)sender).Icon, path))
				dlg.ShowDialog();
		}

		private void HandleAboutDialogCheckForUpdatesClick(object sender, EventArgs e)
		{
			var updateStatus = UpdateChecker.CheckForUpdatesAtUserRequest();
			if (sender is SILAboutBox aboutBox && updateStatus == Sparkle.UpdateStatus.UpdateNotAvailable)
				aboutBox.NotifyNoUpdatesAvailable();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			Settings.Default.Save();
		}

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			_recordingToolControl1.StartFilteringMessages();
		}

		protected override void OnDeactivate(EventArgs e)
		{
			base.OnDeactivate(e);
			_recordingToolControl1.StopFilteringMessages();
		}

		private bool LoadProject(string name)
		{
			try
			{
				_projectNameToShow = name;
				if (Settings.Default.Project != name)
				{
					// Forget any actor and character we remembered from another project.
					// (Even if this one isn't multivoice.)
					Settings.Default.Actor = Settings.Default.Character = null;
				}
				ScriptProviderBase scriptProvider;
				if (name == SampleScriptProvider.kProjectUiName)
					scriptProvider = new SampleScriptProvider();
				else if (Path.GetExtension(name) == MultiVoiceScriptProvider.MultiVoiceFileExtension)
				{
					var mvScriptProvider = MultiVoiceScriptProvider.Load(name);
					scriptProvider = mvScriptProvider;
					mvScriptProvider.RestrictToCharacter(Settings.Default.Actor, Settings.Default.Character);
					_multiVoicePanel.Visible = true;
					_multiVoiceMarginPanel.Visible = true;
					// This combination puts the two top-docked controls and the fill-docked _recordingToolControl into the right
					// sequence in the Controls list so that the top two are in the right order and the recording tool occupies
					// the rest of the space.
					// I can't find ANY order I can set in the designer which does this properly, possibly because when layout is
					// first done the multi voice panel is hidden. Another thing that might work is to put them in the right order
					// in the designer and force a layout after making the multi-voice control visible. I haven't tried that.
					// If you experiment with changing this watch out for the top controls being in the wrong order and also
					// for the recording tool being partly hidden behind one or both of them. The latter is easy to miss because
					// there is quite a bit of unused space at the top of the recording control.
					_multiVoicePanel.BringToFront();
					_recordingToolControl1.BringToFront();
					UpdateActorCharacter(mvScriptProvider, true);
				}
				else if (Path.GetExtension(name) == ExistingProjectsList.kProjectFileExtension ||
					Path.GetExtension(name) == ".zip")
				{
					TextBundle<DblTextMetadata<DblMetadataLanguage>, DblMetadataLanguage> bundle;
					try
					{
						bundle = new TextBundle<DblTextMetadata<DblMetadataLanguage>, DblMetadataLanguage>(name);
					}
					catch (Exception e)
					{
						ErrorReport.NotifyUserOfProblem(e,
							LocalizationManager.GetString("MainWindow.ProjectMetadataInvalid", "Project could not be loaded: {0}"), name);
						return false;
					}
					var metadata = bundle.Metadata;

					var hearThisProjectFolder = Path.Combine(Program.ApplicationDataBaseFolder, metadata.Language.Iso + "_" + metadata.Name);

					if (Path.GetExtension(name) == ".zip" || Path.GetDirectoryName(name) != hearThisProjectFolder)
					{
						var projectFile = Path.Combine(hearThisProjectFolder, Path.ChangeExtension(Path.GetFileName(name), ExistingProjectsList.kProjectFileExtension));
						if (Directory.Exists(hearThisProjectFolder))
						{
							if (File.Exists(projectFile))
							{
								//TODO: Deal with collision. Offer to open existing project. Overwrite using this bundle?
								return false;
							}
						}
						else
							Directory.CreateDirectory(hearThisProjectFolder);
						File.Copy(name, projectFile);
						name = projectFile;
						bundle = new TextBundle<DblTextMetadata<DblMetadataLanguage>, DblMetadataLanguage>(name);
					}
					scriptProvider = new ParatextScriptProvider(new TextBundleScripture(bundle));
					_projectNameToShow = metadata.Name;
				}
				else
				{
					ScrText paratextProject = ScrTextCollection.Find(name);
					if (paratextProject == null)
						return false;
					_projectNameToShow = paratextProject.JoinedNameAndFullName;
					scriptProvider = new ParatextScriptProvider(new ParatextScripture(paratextProject));
				}
				if (!(scriptProvider is IActorCharacterProvider))
				{
					// Also can't seem to get this right in designer, with the invisible actor chooser panel confusing things.
					_recordingToolControl1.BringToFront();
				}

				Project = new Project(scriptProvider);
				if (Project.ActorCharacterProvider == null)
				{
					_multiVoicePanel.Hide(); // in case shown by a previously open project.
					_multiVoiceMarginPanel.Hide();
				}
				if (OnProjectChanged != null)
					OnProjectChanged(this, new EventArgs());
				SetWindowText();

				Settings.Default.Project = name;
				Settings.Default.Save();
				return true;
			}
			catch (IncompatibleFileVersionException)
			{
				using (var dlg = new UpgradeNeededDialog())
				{
					dlg.Description = string.Format(LocalizationManager.GetString("MainWindow.IncompatibleVersion.Text", "This version of HearThis is not able to load the selected file ({0}). Please upgrade to the latest version."), name);
					dlg.CheckForUpdatesClicked += HandleAboutDialogCheckForUpdatesClick;
					dlg.ShowDialog(this);
				}
			}
			catch (Exception e)
			{
				ErrorReport.NotifyUserOfProblem(e, "Could not open " + name);
			}
			return false; //didn't load it
		}

		private void SetWindowText()
		{
			var ver = Assembly.GetExecutingAssembly().GetName().Version;
#if MULTIPLEMODES
			Text =
				string.Format(
					LocalizationManager.GetString("MainWindow.WindowTitle", "{3} -- HearThis {0}.{1}.{2} ({4})",
						"{3} is project name, {0}.{1}.{2} are parts of version number. {4} is the active mode (i.e., view)"),
						ver.Major, ver.Minor, ver.Build, _projectNameToShow, _btnMode.Text);
#else
			Text =
				string.Format(
					LocalizationManager.GetString("MainWindow.WindowTitle", "{3} -- {4} {0}.{1}.{2}",
						"{4} is product name: HearThis; {3} is project name, {0}.{1}.{2} are parts of version number."),
						ver.Major, ver.Minor, ver.Build, _projectNameToShow, Program.kProduct);
#endif
		}

		private void ModeDropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
#if MULTIPLEMODES
			if (Settings.Default.ActiveMode != (string) e.ClickedItem.Tag)
			{
				Settings.Default.ActiveMode = (string) e.ClickedItem.Tag;
				SetMode(e.ClickedItem);
			}
#endif
		}

		private void _syncWithAndroidItem_Click(object sender, EventArgs e)
		{
			AndroidSynchronization.DoAndroidSync(Project);
		}
		private void Shell_ResizeEnd(object sender, EventArgs e)
		{
			if (WindowState != FormWindowState.Normal)
				return;

			Settings.Default.RestoreBounds = new Rectangle(Left, Top, Width, Height);
			Settings.Default.Save();
		}

		private string _previousActor;
		private string _previousCharacter;

		private void _actorCharacterButton_Click(object sender, EventArgs e)
		{
			var chooser = new ActorCharacterChooser();
			_previousActor = Project.ActorCharacterProvider.Actor;
			_previousCharacter = Project.ActorCharacterProvider.Character;
			chooser.Location = new Point(_actorCharacterButton.Left, _multiVoicePanel.Top);
			chooser.Closed += (o, args) =>
			{
				UpdateActorCharacter(Project.ActorCharacterProvider, false);
				// Figure out whether the mouse is now in the panel.
				MultiVoicePanelOnMouseTransition(null, null);
				// And may need to redraw even if the transition code thinks it hasn't changed,
				// since something seems to cache the state behind the control.
				_multiVoicePanel.Invalidate();
			};
			this.Controls.Add(chooser);
			chooser.ActorCharacterProvider = Project.ActorCharacterProvider; // not until it has a handle!
			chooser.BringToFront();
			// gives it a chance to notice we are up and turn off the border rectangle.
			_multiVoicePanel.Invalidate();
		}

		private string _originalCurrentActorItemText;

		private void UpdateActorCharacter(IActorCharacterProvider provider, bool initializing)
		{
			if (!initializing && provider.Actor == null)
			{
				// A special case for when the user brings up the dialog in the ???? state and 'changes' it to Overview.
				// This would otherwise be considered 'no change' and not update the label.
				// We don't do this at startup because then we WANT the ???? label.
				_actorLabel.Text = ActorCharacterChooser.OverviewLabel;
			}
			if (initializing)
			{
				// So the designer text is not visible while waiting for the fullyRecorded data.
				_characterLabel.Text = "";
			}
			if (!initializing && _previousActor == provider.Actor && _previousCharacter == provider.Character)
				return; // nothing changed.
			provider.DoWhenFullyRecordedCharactersAvailable((fullyRecorded) =>
			{
				this.Invoke((Action) (() =>
				{
					if (string.IsNullOrEmpty(provider.Actor))
					{
						if (!initializing) // When initializing, leave the original question marks.
							_actorLabel.Text = ActorCharacterChooser.OverviewLabel;
						_characterLabel.Text = "";
					}
					else
					{
						_actorLabel.Text = (fullyRecorded.AllRecorded(provider.Actor) ? ActorCharacterChooser.LeadingCheck : "") + provider.Actor;
						_characterLabel.Text = (fullyRecorded.AllRecorded(provider.Actor, provider.Character) ? ActorCharacterChooser.LeadingCheck : "") +
						                       provider.Character;
					}
				}));
			});
			// When initializing, we want any saved current position to win. Also, we don't yet have
			// things initialized enough to call this method.
			if (!initializing)
				_recordingToolControl1.UpdateForActorCharacter();
		}

		private void _actorLabel_Click(object sender, EventArgs e)
		{
			_actorCharacterButton_Click(sender, e);
		}

		private void _characterLabel_Click(object sender, EventArgs e)
		{
			_actorCharacterButton_Click(sender, e);
		}

		private void _saveHearthisPackItem_Click(object sender, EventArgs e)
		{
			bool limitToActor = false;
			using (var htDlg = new SaveHearThisPackDlg())
			{
				htDlg.Actor = Project.ActorCharacterProvider?.Actor;
				if (htDlg.ShowDialog(this) != DialogResult.OK)
					return;
				limitToActor = htDlg.LimitToActor;
			}
			var dlg = new SaveFileDialog();
			dlg.Filter = HearThisPackFilter;
			dlg.RestoreDirectory = true;
			if (dlg.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(dlg.FileName))
				return;
			var packer = new HearThisPackMaker(Project.ProjectFolder);
			if (limitToActor && Project.ActorCharacterProvider != null)
				packer.Actor = Project.ActorCharacterProvider.Actor;
			var progressDlg = new MergeProgressDialog();
			// See comment in merge...dialog will close when user clicks OK AFTER this method returns.
			progressDlg.Closed += (o, args) => progressDlg.Dispose();
			progressDlg.SetSource(Path.GetFileName(dlg.FileName));
			progressDlg.Show(this);
			// Enhance: is it worth having the message indicate whether we are restricting to actor?
			// If it didn't mean yet another message to localize I would.
			progressDlg.SetLabel(string.Format(LocalizationManager.GetString("MainWindow.SavingTo", "Saving to {0}", "Keep {0} as a placeholder for the file name")
				, Path.GetFileName(dlg.FileName)));
			progressDlg.Text = string.Format(LocalizationManager.GetString("MainWindow.SavingHearThisPack", "Saving {0}", "{0} will be the file extension, HearThisPack"), "HearThisPack");
			packer.Pack(dlg.FileName, progressDlg.LogBox);
			progressDlg.LogBox.WriteMessage(string.Format(LocalizationManager.GetString("MainWindow.PackComplete", "{0} is complete--click OK to close this window"), "HearThisPack"));
			progressDlg.SetDone();
		}

		private static string HearThisPackFilter => @"HearThisPack files (*" + HearThisPackMaker.HearThisPackExtension + @")|*" +
		                                            HearThisPackMaker.HearThisPackExtension;

		private void _mergeHearthisPackItem_Click(object sender, EventArgs e)
		{
			var dlg = new OpenFileDialog();
			dlg.Filter = HearThisPackFilter;
			dlg.RestoreDirectory = true;
			dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if (dlg.ShowDialog() != DialogResult.OK)
				return;
			using (var reader = new HearThisPackReader(dlg.FileName))
			{
				if (reader.ProjectName.ToLowerInvariant() != Project.Name.ToLowerInvariant())
				{
					var msg = LocalizationManager.GetString("MainWindow.MergeNoData",
						"This HearThis pack does not have any data for {0}. It contains data for {1}. If you want to merge it please open that project.",
						"Keep {0} as a placeholder for the current project name, {1} for the project in the file");
					MessageBox.Show(this,
						string.Format(msg, Project.Name, reader.ProjectName),
						LocalizationManager.GetString("MainWindow.MergeWrongProject", "Wrong Project"),
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning);
					return;
				}
				var packLink = reader.GetLink();
				var ourLink = new WindowsLink(Program.ApplicationDataBaseFolder);
				var merger = new RepoMerger(Project, ourLink, packLink);
				merger.SendData = false; // don't need to send anything to the hear this pack
				// Don't change this to using...we want the dialog to stay open after this method returns,
				// so the user can read the progress information (which may be quite useful as a record
				// of what was merged). And we can't dispose it until it closes, so just arrange an
				// event to do it then.
				var progressDlg = new MergeProgressDialog();
				progressDlg.Closed += (o, args) => progressDlg.Dispose();
				progressDlg.SetLabel(Path.GetFileName(dlg.FileName));
				progressDlg.Show(this);
				merger.Merge(progressDlg.LogBox);
				_recordingToolControl1.UpdateAfterMerge();
				progressDlg.LogBox.WriteMessage(LocalizationManager.GetString("MergeProgressDialog.MergeComplete", "Merge is complete--click OK to close this window"));
				progressDlg.SetDone();
			}
		}

		private void supportToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start(@"https://community.scripture.software.sil.org/");
		}
	}
}