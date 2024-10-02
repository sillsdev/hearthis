// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2024, SIL International. All Rights Reserved.
// <copyright from='2011' to='2024' company='SIL International'>
//		Copyright (c) 2024, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using DesktopAnalytics;
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
using SIL.Email;
using SIL.Reporting;
using SIL.Windows.Forms.Extensions;
using static System.String;

namespace HearThis.UI
{
	public partial class Shell : Form, ILocalizable
	{
		private bool _showReleaseNotesOnActivated;
		private bool _bringToFrontWhenShown;
		private static Sparkle UpdateChecker;
		public event EventHandler ProjectLoadInitializationSequenceCompleted;
		public event EventHandler ProjectChanged;
		public delegate void ModeChangedHandler(object sender, Mode newMode);
		public event ModeChangedHandler ModeChanged;
		private string _projectNameToShow = Empty;
		private bool _mouseInMultiVoicePanel;

#if MULTIPLEMODES
		private List<string> allowableModes;

		private const string kAdministrative = "Administrator";
#endif
		private const string kNormalRecording = "NormalRecording";

		public Shell(bool bringToFrontOnFirstActivation = false, bool showReleaseNotesOnStartup = false)
		{
			_bringToFrontWhenShown = bringToFrontOnFirstActivation;
			_showReleaseNotesOnActivated = showReleaseNotesOnStartup;
			InitializeComponent();
			_recordingToolControl1.SetGetUIString(GetUIString);
			_toolStrip.BackColor = AppPalette.Background;
			readAndRecordToolStripMenuItem.Tag = Mode.ReadAndRecord;
			checkForProblemsToolStripMenuItem.Tag = Mode.CheckForProblems;
			Text = Program.kProduct;

			_settingsProtectionHelper.SetSettingsProtection(_settingsItem, true);
			_settingsProtectionHelper.SetSettingsProtection(toolStripButtonChooseProject, true);
			if (!Settings.Default.EnableCheckForProblemsViewInProtectedMode)
			{
				_settingsProtectionHelper.SetSettingsProtection(readAndRecordToolStripMenuItem, true);
				_settingsProtectionHelper.SetSettingsProtection(checkForProblemsToolStripMenuItem, true);
			}

			SetupUILanguageMenu();

			SetColors();

			InitializeModesCombo();

			// TODO: possibly make this conditional on a device being connected.
			// If possible notice and show it when a device is later connected.
			// Or: possibly if no device is active it displays instructions.
			_syncWithAndroidItem.Visible = true;
			_toolStrip.Renderer = new ToolStripColorArrowRenderer { CheckedItemUnderlineColor = AppPalette.Blue };
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
					ControlPaint.DrawBorder(e.Graphics, borderRect, AppPalette.FaintScriptFocusTextColor,
						ButtonBorderStyle.Solid);
				}
			};
			Program.RegisterLocalizable(this);
		}

		/// <summary>
		/// Unfortunately the mouse 'leaves' the multivoice panel when it enters a child control as well as when it really
		/// leaves the whole panel. So this routine is hooked to happen whenever it leaves or enters any of them.
		/// It figures out whether the mouse is really inside the panel and adjusts the border if this has changed.
		/// </summary>
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
			_recordingToolControl1.StopPlaying();

			using (var dlg = new ChooseProject())
			{
				if (DialogResult.OK == dlg.ShowDialog(this))
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
			var loaded = !IsNullOrEmpty(Settings.Default.Project) &&
				LoadProject(Settings.Default.Project);

			ProjectLoadInitializationSequenceCompleted?.Invoke(this, EventArgs.Empty);

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

			UpdateChecker = new Sparkle(@"https://build.palaso.org/guestAuth/repository/download/HearThis_HearThisWinDevPublishPt8/.lastSuccessful/appcast.xml",
				Icon);
			UpdateChecker.DoLaunchAfterUpdate = false; // The HearThis installer already takes care of launching.
			// We don't want to do this until the main window is loaded because a) it's very easy for the user to overlook, and b)
			// more importantly, when the toast notifier closes, it can sometimes clobber an error message being displayed for the user.
			UpdateChecker.CheckOnFirstApplicationIdle();
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			// This is a hack to get HearThis to become the active app when it is launched from the
			// installer.
			if (Visible && _bringToFrontWhenShown)
			{
				_bringToFrontWhenShown = false;
				if (ActiveForm == null)
				{
					// No one could think this is "right" but it seems to be the only reliable way.
					// See https://stackoverflow.com/questions/1463417/what-is-the-right-way-to-bring-a-windows-forms-application-to-the-foreground
					WindowState = FormWindowState.Minimized;
					Show();
					WindowState = FormWindowState.Normal;
				}
			}
		}

		/// <summary>
		/// Gets whether a significant (at least 100x100 pixels) portion of the form is on-screen.
		/// </summary>
		private static bool IsOnScreen(Rectangle rect)
		{
			var screens = Screen.AllScreens;
			var formTopLeft = new Rectangle(rect.Left, rect.Top, 100, 100);

			return screens.Any(screen => screen.WorkingArea.Contains(formTopLeft));
		}

		private void SetColors()
		{
			_moreMenu.ForeColor = AppPalette.NavigationTextColor;
			_multiVoicePanel.BackColor = AppPalette.Background;
			_multiVoiceMarginPanel.BackColor = _multiVoicePanel.BackColor;
			_actorLabel.ForeColor = AppPalette.ScriptFocusTextColor;
			_characterLabel.ForeColor = AppPalette.ScriptFocusTextColor;
			_actorCharacterButton.BackColor = AppPalette.Background;
			_actorCharacterButton.ForeColor = AppPalette.Background;
			_actorCharacterButton.Image = AppPalette.ActorCharacterImage;

		}

		private void SetupUILanguageMenu()
		{
			bool LanguageSelected(string languageId)
			{
				Analytics.Track("UI language chosen",
					new Dictionary<string, string> {
						{ "Previous", Settings.Default.UserInterfaceLanguage },
						{ "New", languageId } });
				Logger.WriteEvent("UI language changed from " +
					$"{Settings.Default.UserInterfaceLanguage} to {languageId}");
				Settings.Default.UserInterfaceLanguage = languageId;
				Program.UpdateUiLanguageForUser(languageId);
				return true;
			}

			bool MoreSelected()
			{
				Analytics.Track("Opened localization dialog box");
				return true;
			}

			_uiLanguageMenu.InitializeWithAvailableUILocales(LanguageSelected,
				Program.PrimaryLocalizationManager, Program.LocIncompleteViewModel,
				MoreSelected);
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
			HandleStringsLocalized();
		}
#endif

		private void OnSaveClick(object sender, EventArgs e)
		{
			MessageBox.Show(
				LocalizationManager.GetString("MainWindow.SaveAutomatically",
					"HearThis automatically saves your work, while you use it. This button is just here to tell you that. To create sound files for playing your recordings, on the More menu, click Export Sound Files."),
				LocalizationManager.GetString("MainWindow.Save", "Save"));
		}

		private void OnPublishClick(object sender, EventArgs e)
		{
			_recordingToolControl1.StopPlaying();
			using (var dlg = new PublishDialog(Project, checkForProblemsToolStripMenuItem.Visible && !checkForProblemsToolStripMenuItem.Checked))
			{
				Logger.WriteEvent("Showing export dialog box.");
				dlg.ShowDialog();
				if (dlg.ShowProblems)
					checkForProblemsToolStripMenuItem.Checked = true;
			}
		}

		private void OnSettingsButtonClicked(object sender, EventArgs e)
		{
			var origBreakQuotesIntoBlocksValue = Project.ProjectSettings.BreakQuotesIntoBlocks;
			var origAdditionalBlockBreakChars = Project.ProjectSettings.AdditionalBlockBreakCharacters;
			var origBreakAtParagraphBreaks = Project.ProjectSettings.BreakAtParagraphBreaks;
			var origRangesToBreakByVerse = Project.ProjectSettings.RangesToBreakByVerse?.ScriptureRanges?.ToList();
			var origDisplayNavigationButtonLabels = Settings.Default.DisplayNavigationButtonLabels;
			DialogResult result = _settingsProtectionHelper.LaunchSettingsIfAppropriate(() =>
			{
				using (var dlg = new AdministrativeSettings(Project, GetUIString, ExternalClipEditorInfo.Singleton))
				{
					Logger.WriteEvent("Showing settings dialog box.");
					return dlg.ShowDialog(FindForm());
				}
			});
			if (result == DialogResult.OK)
			{
				if (Settings.Default.EnableCheckForProblemsViewInProtectedMode)
				{
					_settingsProtectionHelper.SetSettingsProtection(readAndRecordToolStripMenuItem, false);
					_settingsProtectionHelper.SetSettingsProtection(checkForProblemsToolStripMenuItem, false);
				}
				else
				{
					// Read & Record will be the only mode available, so we need to select it.
					// Doing it this way not only gets all the downstream controls to update to the
					// correct state, it also ensures that the toolbar menu items will appear
					// correctly when shown (when the user presses Shift+Control).
					readAndRecordToolStripMenuItem.Checked = true;
					_settingsProtectionHelper.SetSettingsProtection(readAndRecordToolStripMenuItem, true);
					_settingsProtectionHelper.SetSettingsProtection(checkForProblemsToolStripMenuItem, true);
				}

				if (origBreakQuotesIntoBlocksValue != Project.ProjectSettings.BreakQuotesIntoBlocks ||
					origAdditionalBlockBreakChars != Project.ProjectSettings.AdditionalBlockBreakCharacters ||
					origBreakAtParagraphBreaks != Project.ProjectSettings.BreakAtParagraphBreaks ||
					((origRangesToBreakByVerse != null && Project.ProjectSettings.RangesToBreakByVerse == null) ||
						(origRangesToBreakByVerse == null && Project.ProjectSettings.RangesToBreakByVerse != null) ||
						(origRangesToBreakByVerse != null &&
							!origRangesToBreakByVerse.SequenceEqual(Project.ProjectSettings.RangesToBreakByVerse.ScriptureRanges))))
				{
					LoadProject(Settings.Default.Project);
				}
				else
				{
					_recordingToolControl1.SetClauseSeparators(Project.ProjectSettings.ClauseBreakCharacterSet);
#if MULTIPLEMODES
					Invoke(new Action(InitializeModesCombo));
#else
					if (origDisplayNavigationButtonLabels != Settings.Default.DisplayNavigationButtonLabels)
						Invoke(new Action(() =>
						{
							_recordingToolControl1.HandleDisplayNavigationButtonLabelsChange();							
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

		private string GetUIString(AdministrativeSettings.UiElement element)
		{
			switch (element)
			{
				case AdministrativeSettings.UiElement.ShiftClipsMenu:
					return _recordingToolControl1.ShiftClipsMenuName;
				case AdministrativeSettings.UiElement.CheckForProblemsView:
					return checkForProblemsToolStripMenuItem.Text;
				default:
					throw new ArgumentOutOfRangeException(nameof(element), element, null);
			}
		}

		private void OnAboutClick(object sender, EventArgs e)
		{
			using (var dlg = new SILAboutBox(FileLocationUtilities.GetFileDistributedWithApplication("aboutbox.htm")))
			{
				dlg.CheckForUpdatesClicked += HandleAboutDialogCheckForUpdatesClick;
				dlg.ReleaseNotesClicked += HandleAboutDialogReleaseNotesClicked;
				Logger.WriteEvent("Showing About dialog box.");
				dlg.ShowDialog(this);
			}
		}

		private static void HandleAboutDialogReleaseNotesClicked(object sender, EventArgs e)
		{
			var path = FileLocationUtilities.GetFileDistributedWithApplication("ReleaseNotes.md");
			using (var dlg = new ShowReleaseNotesDialog(((Form)sender).Icon, path))
				dlg.ShowDialog();
		}

		private static void HandleAboutDialogCheckForUpdatesClick(object sender, EventArgs e)
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
			if (_showReleaseNotesOnActivated)
			{
				_showReleaseNotesOnActivated = false;
				Application.Idle += ShowReleaseNotesWhenActiveAndIdle;
			}

			_recordingToolControl1.MicCheckingEnabled = true;
		}

		private void ShowReleaseNotesWhenActiveAndIdle(object sender, EventArgs e)
		{
			if (ActiveForm == this)
			{
				Application.Idle -= ShowReleaseNotesWhenActiveAndIdle;
				Logger.WriteEvent("Displaying release notes on idle after install.");
				using (var dlg = new ShowReleaseNotesDialog(Icon, FileLocationUtilities.GetFileDistributedWithApplication("releaseNotes.md")))
					dlg.ShowDialog(this);
			}
		}

		protected override void OnDeactivate(EventArgs e)
		{
			base.OnDeactivate(e);
			_recordingToolControl1.MicCheckingEnabled = false;
		}

		private bool LoadProject(string name)
		{
			Logger.WriteEvent("Loading project " + name);
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
				{
					// Changing the color scheme forces a restart, but in that case we don't want to
					// re-initialize the sample project because that would confuse the user.
					scriptProvider = new SampleScriptProvider(Program.RestartedToChangeColorScheme);
				}
				else
				{
					var extension = Path.GetExtension(name);
					var isZip = false;
					switch (extension)
					{
						case MultiVoiceScriptProvider.kMultiVoiceFileExtension:
							scriptProvider = LoadMultivoiceProject(name);
							break;
						case ".zip":
							isZip = true;
							goto case ExistingProjectsList.kProjectFileExtension;
						case ExistingProjectsList.kProjectFileExtension:
						{
							scriptProvider = LoadBundleBasedProject(ref name, isZip);
							if (scriptProvider == null)
								return false;
							break;
						}
						default:
						{
							// In this case the "extension" is really the project ID.
							var id = extension.StartsWith(".") ? extension.Substring(1) : null;
							ScrText paratextProject = null;
							// The following falls back to looking for the project by name if
							// the id is null or looks to be an invalid ID.
							paratextProject = ScrTextCollection.FindById(HexId.FromStrSafe(id), name);
							if (paratextProject == null)
							{
								// We should never get in here coming from the Choose Project
								// dialog, but when restoring the last opened project from settings
								// (which previously only stored the short name), we can in the
								// rare case where there is more than one project with this name
								// (in which case FindById returns null). Look through all projects
								// to find the one (if any) with this name that is stored in the
								// "normal" place with its files in a directory just named using
								// the short name. That should be the one we want because it was
								// the first one. (Any subsequent ones will be stored in
								// _projectsById and will have a Directory of name.ID.)
								try
								{
									paratextProject = ScrTextCollection.ScrTexts(
										IncludeProjects.AccessibleScripture).FirstOrDefault(
										s => Path.GetFileName(s.Directory) == name);
								}
								catch (Exception e)
								{
									Logger.WriteError("Problem trying to find Paratext project.", e);
								}
							}

							// Upgrading from the old world, where we just remembered the project
							// by its short name. From now on, we'll remember the ID, so we can
							// look it up that way.
							if (paratextProject != null && id == null)
								name += "." + paratextProject.Guid;

							if (paratextProject == null)
								return false;
							_projectNameToShow = paratextProject.ToString();
							scriptProvider = new ParatextScriptProvider(new ParatextScripture(paratextProject));
							Logger.WriteEvent("Paratext project loaded: " + name);
							Analytics.Track("LoadedParatextProject");
							break;
						}
					}
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

				ProjectChanged?.Invoke(this, EventArgs.Empty);

				// We normally want to open project in the default (read-and-record) mode, but if
				// we are opening the default project following a restart to change color schemes,
				// we need to re-open in the previous mode so as not to confuse the user.
				var initialModeForProject = Program.RestartedToChangeColorScheme ?
					Settings.Default.CurrentMode : Mode.ReadAndRecord;
				_toolStrip.Items.OfType<ToolStripMenuItem>().Single(i =>
					i.Tag is Mode mode && mode == initialModeForProject).Checked = true;

				HandleStringsLocalized();

				Settings.Default.Project = name;
				Settings.Default.Save();

				if (!IsNullOrEmpty(Project.ProjectSettings.LastDataMigrationReportNag))
				{
					Logger.WriteEvent("Project.ProjectSettings.LastDataMigrationReportNag = " + Project.ProjectSettings.LastDataMigrationReportNag);

					bool clearNag;
					var dataMigrationReportFilename = scriptProvider.GetDataMigrationReportFilename(
						Project.ProjectSettings.LastDataMigrationReportNag);
					try
					{
						if (File.Exists(dataMigrationReportFilename))
							clearNag = false;
						else
						{
							// Also need to check for the old file (used to be XML).
							dataMigrationReportFilename = Path.ChangeExtension(dataMigrationReportFilename, "xml");
							clearNag = !File.Exists(dataMigrationReportFilename);
						}
					}
					catch (Exception e)
					{
						Logger.WriteError(e);
						clearNag = true;
					}
					if (!clearNag)
					{
						using (var dlg = new DataMigrationReportNagDlg(Project.ProjectSettings.LastDataMigrationReportNag, dataMigrationReportFilename,
							ScriptProviderBase.GetUrlForHelpWithDataMigrationProblem(Project.ProjectSettings.LastDataMigrationReportNag)))
						{
							Logger.WriteEvent("Showing Data Migration Report nag dialog box.");
							dlg.ShowDialog(this);
							clearNag = dlg.StopNagging;
							if (dlg.DeleteReportFile)
							{
								try
								{
									RobustFile.Delete(dataMigrationReportFilename);
								}
								catch (Exception e)
								{
									ErrorReport.ReportNonFatalException(e);
								}
							}
						}
					}

					if (clearNag)
					{
						Project.ProjectSettings.LastDataMigrationReportNag = "";
						Project.SaveProjectSettings();
					}
				}

				return true;
			}
			catch (IncompatibleFileVersionException)
			{
				using (var dlg = new UpgradeNeededDialog())
				{
					dlg.Description = Format(LocalizationManager.GetString("MainWindow.IncompatibleVersion.Text",
						"This version of HearThis is not able to load the selected file ({0}). Please upgrade to the latest version."), name);
					dlg.CheckForUpdatesClicked += HandleAboutDialogCheckForUpdatesClick;
					dlg.ShowDialog(this);
				}
			}
			catch (IncompatibleProjectDataVersionException e)
			{
				using (var dlg = new UpgradeNeededDialog())
				{
					dlg.Description = Format(LocalizationManager.GetString(
							"MainWindow.IncompatibleProjectDataVersion",
							"The {0} project was edited with a newer version of {1}. Its data version ({2}) is not compatible with this version of the program. Please upgrade to the latest version of {1} to open this project or contact technical support if you need to downgrade this project.",
							"Param 0: name of project; " +
							"Param 1: \"HearThis\" (product name); " +
							"Param 2: data version of project"),
						e.Project, Program.kProduct, e.ProjectVersion);
					dlg.CheckForUpdatesClicked += HandleAboutDialogCheckForUpdatesClick;
					dlg.ShowDialog(this);
				}
			}
			catch (ProjectOpenCancelledException)
			{
				// There was some kind of file load error that was either automatically deemed
				// fatal (for this project) by the program or that the user (wisely) chose not to
				// ignore. But we don't want to treat it as fatal for HearThis because it might not
				// have been caused by HearThis at all, and the user might want to try to open a
				// different project.
			}
			catch (Exception e)
			{
				ErrorReport.NotifyUserOfProblem(e, "Could not open " + name);
			}
			return false; //didn't load it
		}
		
		private ScriptProviderBase LoadMultivoiceProject(string name)
		{
			var mvScriptProvider = MultiVoiceScriptProvider.Load(name);
			Analytics.Track("LoadedGlyssenScriptProject");
			mvScriptProvider.RestrictToCharacter(Settings.Default.Actor, Settings.Default.Character);
			_multiVoicePanel.Visible = _multiVoiceMarginPanel.Visible =
				Settings.Default.CurrentMode == Mode.ReadAndRecord;
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
			UpdateActorCharacter(mvScriptProvider);
			return mvScriptProvider;
		}

		private ScriptProviderBase LoadBundleBasedProject(ref string name, bool isZip)
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
				return null;
			}

			var metadata = bundle.Metadata;

			var hearThisProjectFolder = Path.Combine(Program.ApplicationDataBaseFolder, metadata.Language.Iso + "_" + metadata.Name);

			if (isZip || Path.GetDirectoryName(name) != hearThisProjectFolder)
			{
				var projectFile = Path.Combine(hearThisProjectFolder, Path.ChangeExtension(Path.GetFileName(name), ExistingProjectsList.kProjectFileExtension));
				if (Directory.Exists(hearThisProjectFolder))
				{
					if (File.Exists(projectFile))
					{
						//TODO: Deal with collision. Offer to open existing project. Overwrite using this bundle?
						return null;
					}
				}
				else
					Directory.CreateDirectory(hearThisProjectFolder);

				RobustFile.Copy(name, projectFile);
				name = projectFile;
				bundle = new TextBundle<DblTextMetadata<DblMetadataLanguage>, DblMetadataLanguage>(name);
			}

			var scriptProvider = new ParatextScriptProvider(new TextBundleScripture(bundle));
			Analytics.Track("LoadedTextReleaseBundleProject");
			_projectNameToShow = metadata.Name;
			readAndRecordToolStripMenuItem.Checked = true;
			return scriptProvider;
		}

		private string MoreLanguagesMenuText => LocalizationManager.GetString("MainWindow.MoreMenuItem",
			"More...", "Last item in menu of UI languages");

		public void HandleStringsLocalized()
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
				Format(
					LocalizationManager.GetString("MainWindow.WindowTitle", "{3} -- {4} {0}.{1}.{2}",
						"{4} is product name: HearThis; {3} is project name, {0}.{1}.{2} are parts of version number."),
						ver.Major, ver.Minor, ver.Build, _projectNameToShow, Program.kProduct);
#endif
			_uiLanguageMenu.DropDownItems[_uiLanguageMenu.DropDownItems.Count - 1].Text = MoreLanguagesMenuText;

			if (_multiVoicePanel.Visible)
			{
				var provider = Project.ActorCharacterProvider;
				if (provider != null)
				{
					_actorLabel.Text = provider.FullyRecordedCharacters.AllRecorded(provider.Actor) ?
						ActorCharacterChooser.LeadingCheck : "" + provider.ActorForUI;
				}
			}
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

		private void _syncWithAndroidItem_Click(object sender, EventArgs e) =>
			AndroidSynchronization.DoAndroidSync(Project, this);

		private void Shell_ResizeEnd(object sender, EventArgs e)
		{
			if (WindowState != FormWindowState.Normal)
				return;

			Settings.Default.RestoreBounds = new Rectangle(Left, Top, Width, Height);
			Settings.Default.Save();
		}

		private void _actorCharacterButton_Click(object sender, EventArgs e)
		{
			var chooser = new ActorCharacterChooser();
			var previousActor = Project.ActorCharacterProvider.Actor;
			var previousCharacter = Project.ActorCharacterProvider.Character;
			chooser.Location = new Point(_actorCharacterButton.Left, _multiVoicePanel.Top);
			chooser.Closed += (o, args) =>
			{
				UpdateActorCharacter(Project.ActorCharacterProvider, previousActor, previousCharacter);
				_recordingToolControl1.UpdateForActorCharacter();
				// Figure out whether the mouse is now in the panel.
				MultiVoicePanelOnMouseTransition(null, null);
				// And may need to redraw even if the transition code thinks it hasn't changed,
				// since something seems to cache the state behind the control.
				_multiVoicePanel.Invalidate();
				_recordingToolControl1.Invalidate(true);
			};
			Controls.Add(chooser);
			chooser.ActorCharacterProvider = Project.ActorCharacterProvider; // not until it has a handle!
			chooser.BringToFront();
			// gives it a chance to notice we are up and turn off the border rectangle.
			_multiVoicePanel.Invalidate();
		}

		private void SetCurrentMode(Mode newMode)
		{
			Settings.Default.CurrentMode = newMode;

			if (Project.ActorCharacterProvider != null)
			{
				switch (newMode)
				{
					case Mode.ReadAndRecord:
						_multiVoicePanel.Show();
						// REVIEW: Should we restore original actor/character?
						break;
					case Mode.CheckForProblems:
						_multiVoicePanel.Hide();
						var previousActor = Project.ActorCharacterProvider.Actor;
						var previousCharacter = Project.ActorCharacterProvider.Character;
						Project.ActorCharacterProvider.RestrictToCharacter(null, null);
						UpdateActorCharacter(Project.ActorCharacterProvider, previousActor, previousCharacter);
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(newMode), newMode, null);
				}
			}

			ModeChanged?.Invoke(this, Settings.Default.CurrentMode);
		}

		private void UpdateActorCharacter(IActorCharacterProvider provider, string previousActor = null, string previousCharacter = null)
		{
			var initializing = previousActor == null && previousCharacter == null;
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
			if (!initializing && previousActor == provider.Actor && previousCharacter == provider.Character)
				return; // nothing changed.
			provider.DoWhenFullyRecordedCharactersAvailable((fullyRecorded) =>
			{
				Invoke((Action) (() =>
				{
					if (IsNullOrEmpty(provider.Actor))
					{
						if (!initializing) // When initializing, leave the original question marks.
							_actorLabel.Text = ActorCharacterChooser.OverviewLabel;
						_characterLabel.Text = "";
					}
					else
					{
						_actorLabel.Text = (fullyRecorded.AllRecorded(provider.Actor) ? ActorCharacterChooser.LeadingCheck : "") + provider.ActorForUI;
						_characterLabel.Text = (fullyRecorded.AllRecorded(provider.Actor, provider.Character) ? ActorCharacterChooser.LeadingCheck : "") +
						                       provider.Character;
					}
				}));
			});
		}

		private void _saveHearThisPackItem_Click(object sender, EventArgs e)
		{
			_recordingToolControl1.StopPlaying();
			bool limitToActor;

			using (var htDlg = new SaveHearThisPackDlg(Project.ActorCharacterProvider))
			{
				Logger.WriteEvent("Showing SaveHearThisPack dialog box");
				if (htDlg.ShowDialog(this) != DialogResult.OK)
					return;
				limitToActor = htDlg.LimitToActor;
			}

			using (var dlg = new SaveFileDialog())
			{
				dlg.Filter = HearThisPackFilter;
				dlg.RestoreDirectory = true;
				if (dlg.ShowDialog() != DialogResult.OK || IsNullOrEmpty(dlg.FileName))
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
				progressDlg.SetLabel(Format(LocalizationManager.GetString(
					"MainWindow.SavingTo", "Saving to {0}", "Keep {0} as a placeholder for the file name"),
					Path.GetFileName(dlg.FileName)));

				// Note: In other places in the UI, we let the localizer decide how/whether to localize the
				// expression "HearThis Pack" even though it is, in some sense, a proper name. I decided to
				// change the English strings to not make "HearThisPack" a parameter, but I am leaving the
				// calls to Format in place (with the parameter changed to be "HearThis Pack") so that it
				// won't break existing localizations.

				progressDlg.Text = Format(LocalizationManager.GetString(
					"MainWindow.SavingHearThisPack", "Saving HearThis Pack"), "HearThis Pack");
				packer.Pack(dlg.FileName, progressDlg.LogBox);

				progressDlg.LogBox.WriteMessage(Format(LocalizationManager.GetString(
					"MainWindow.PackComplete", "Saving HearThis Pack is complete. Click OK to close this window."),
					"HearThis Pack"));
				progressDlg.SetDone();
			}
		}

		private static string HearThisPackFilter => @"HearThisPack files (*" + HearThisPackMaker.HearThisPackExtension + @")|*" +
		                                            HearThisPackMaker.HearThisPackExtension;

		private void _mergeHearThisPackItem_Click(object sender, EventArgs e)
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
					var msg = Format(LocalizationManager.GetString("MainWindow.MergeNoData",
						"This HearThis Pack does not have any data for {0}. It contains data for {1}. If you want to merge it please open that project.",
						"Keep {0} as a placeholder for the current project name, {1} for the project in the file"), Project.Name, reader.ProjectName);
					Logger.WriteEvent(msg);
					MessageBox.Show(this,
						msg,
						LocalizationManager.GetString("MainWindow.MergeWrongProject", "Wrong Project"),
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning);
					return;
				}

				Logger.WriteEvent("Merging HearThis Pack: " + dlg.FileName);
				var packLink = reader.GetLink();
				var ourLink = new WindowsLink(Program.ApplicationDataBaseFolder);
				var merger = new RepoMerger(Project, ourLink, packLink);
				merger.SendData = false; // don't need to send anything to the HearThis pack
				// Don't change this to using...we want the dialog to stay open after this method returns,
				// so the user can read the progress information (which may be quite useful as a record
				// of what was merged). And we can't dispose it until it closes, so just arrange an
				// event to do it then.
				var progressDlg = new MergeProgressDialog();
				progressDlg.Closed += (o, args) => progressDlg.Dispose();
				progressDlg.SetLabel(Path.GetFileName(dlg.FileName));
				progressDlg.Show(this);
				merger.Merge(Project.StylesToSkipByDefault, progressDlg.LogBox);
				Project.ScriptProvider.UpdateSkipInfo();
				_recordingToolControl1.UpdateAfterMerge();
				progressDlg.LogBox.WriteMessage(LocalizationManager.GetString("MergeProgressDialog.MergeComplete", "Merge is complete--click OK to close this window"));
				progressDlg.SetDone();
			}
		}

		private void giveFeedbackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var model = new GiveFeedbackViewModel(this, Project.PathToLastClipRecorded);
			using (var dlg = new GiveFeedbackDlg(model))
			{
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					model.IssueFeedback();

					try
					{
						var emailProvider = EmailProviderFactory.PreferredEmailProvider();
						var emailMessage = emailProvider.CreateMessage();
						emailMessage.To.Add(ErrorReport.EmailAddress);
						emailMessage.Subject = dlg.Title;
						emailMessage.Body = "TODO: Complete this";
						if (emailMessage.Send(emailProvider))
							Close();
					}
					catch (Exception)
					{
						//swallow it and go to the alternate method
					}
				}
			}
		}

		private void MenuDropDownOpening(object sender, EventArgs e)
		{
			var menuItem = sender as ToolStripDropDownButton;
			if (menuItem == null || menuItem.HasDropDownItems == false)
				return; // not a drop down item
			// Current bounds of the current monitor
			var upperRightCornerOfMenuInScreenCoordinates = menuItem.GetCurrentParent().PointToScreen(new Point(menuItem.Bounds.Right, menuItem.Bounds.Top));
			var currentScreen = Screen.FromPoint(upperRightCornerOfMenuInScreenCoordinates);

			// Get width of widest child item (skip separators!)
			var maxWidth = menuItem.DropDownItems.OfType<ToolStripMenuItem>().Select(m => m.Width).Max();

			var farRight = upperRightCornerOfMenuInScreenCoordinates.X + maxWidth;
			var currentMonitorRight = currentScreen.Bounds.Right;

			menuItem.DropDownDirection = farRight > currentMonitorRight ? ToolStripDropDownDirection.Left :
				ToolStripDropDownDirection.Right;
		}

		private void supportToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start($"https://{Program.kSupportUrlSansHttps}");
		}

		private void checkForProblemsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			if (checkForProblemsToolStripMenuItem.Checked)
				UpdateUIForMode(checkForProblemsToolStripMenuItem, readAndRecordToolStripMenuItem);
		}

		private void readAndRecordToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			if (readAndRecordToolStripMenuItem.Checked)
				UpdateUIForMode(readAndRecordToolStripMenuItem, checkForProblemsToolStripMenuItem);
		}

		private void UpdateUIForMode(ToolStripMenuItem selected, ToolStripMenuItem previous)
		{
			(selected.ForeColor, previous.ForeColor) = (previous.ForeColor, selected.ForeColor);
			(selected.Font, previous.Font) = (previous.Font, selected.Font);
			previous.Checked = false;
			previous.CheckOnClick = true;
			selected.CheckOnClick = false;
			SetCurrentMode((Mode)selected.Tag);
		}
	}
}
