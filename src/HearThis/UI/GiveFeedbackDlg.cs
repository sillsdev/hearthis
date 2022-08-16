// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International. All Rights Reserved.
// <copyright from='2021' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using L10NSharp;

namespace HearThis.UI
{
	/// <summary>
	/// This allows users a way to report a bug or give other feedback regarding HearThis.
	/// </summary>
	public partial class GiveFeedbackDlg : Form, ILocalizable
	{
		private readonly GiveFeedbackViewModel _model;

		public enum PriorityOrSeverity
		{
			LostData = 0,
			NotAbleToCompleteTask = 1,
			CompletedTaskWithDifficulty = 2,
			Minor = 3,
			NotApplicable,
		}

		public enum AffectedArea
		{
			NotApplicable, // Not available for some types of Feedback
			Exporting,
			Installation,
			Localization,
			MultipleAreas,
			Navigation,
			Other,
			Playback,
			ProjectAdministration,
			ProjectSelection,
			Recording,
			Settings,
			Website,
			Unknown,
		}

		public GiveFeedbackDlg(GiveFeedbackViewModel model)
		{
			_model = model;
			InitializeComponent();
			_linkCommunityHelp.Links[0].LinkData = new Action(() => _model.GoToCommunitySite());
			_linkDonate.Links[0].LinkData = new Action(OpenDonationPage);

			Program.RegisterLocalizable(this);
			HandleStringsLocalized();
		}

		#region Properties
		public string Title => _txtTitle.Text;

		public TypeOfFeedback FeedbackType => _model.Type;

		public PriorityOrSeverity Priority => PriorityIsRelevant ?
			(PriorityOrSeverity)_cboPriority.SelectedIndex : PriorityOrSeverity.NotApplicable;

		public string Description => _richTextBoxDescription.Text;

		public AffectedArea AffectedPartOfProgram => (AffectedArea)
			(_cboAffects.SelectedIndex + (FeedbackType == TypeOfFeedback.Appreciation ? 0 : 1));

		private bool PriorityIsRelevant
		{
			get => _cboPriority.Visible;
			set => _cboPriority.Visible = _lblPriorityOrSeverity.Visible = value;
		}
		#endregion

		#region Evemnt handlers
		public void HandleStringsLocalized()
		{
			_lblPriorityOrSeverity.Tag = _lblPriorityOrSeverity.Text;
			_cboPriority.Tag = new List<string>(from object item in _cboPriority.Items select item.ToString());
			_cboAffects.Tag = _cboAffects.Items[0].ToString();
			_linkCommunityHelp.SetLinkRegions();
			_linkDonate.SetLinkRegions();
		}

		private void LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			((Action)e.Link.LinkData).Invoke();
		}

		private void UpdateOkButtonState(object sender, EventArgs e)
		{

		}

		private void _cboTypeOfFeedback_SelectedIndexChanged(object sender, EventArgs e)
		{
			_model.Type = (TypeOfFeedback)_cboTypeOfFeedback.SelectedIndex;
			_cboPriority.Items.Clear();
			if (!(_cboPriority.Tag is List<string> severityList) || severityList.Count != 4)
				throw new ApplicationException("_cboPriority.Tag not set properly in HandleStringsLocalized");
			if (!(_cboAffects.Tag is string notApplicableItem))
				throw new ApplicationException("_cboAffects.Tag not set properly in HandleStringsLocalized");

			switch (FeedbackType)
			{
				case TypeOfFeedback.Problem:
					_lblPriorityOrSeverity.Text = _lblPriorityOrSeverity.Tag as string;
					PriorityIsRelevant = true;
					RemoveNotApplicableItemFromAffectsCombo();
					foreach (var item in severityList)
						_cboPriority.Items.Add(item);
					break;

				case TypeOfFeedback.Suggestion:
					_lblPriorityOrSeverity.Text = LocalizationManager.GetString("GiveFeedbackDlg._lblPriorityOrSeverity.SuggestionText",
						"Priority");
					PriorityIsRelevant = true;
					RemoveNotApplicableItemFromAffectsCombo();
					_cboPriority.Items.Add(severityList[(int)PriorityOrSeverity.NotAbleToCompleteTask]);
					_cboPriority.Items.Add(severityList[(int)PriorityOrSeverity.CompletedTaskWithDifficulty]);
					_cboPriority.Items.Add(LocalizationManager.GetString("GiveFeedbackDlg._cboPriority.MinorEnhancement",
						"Idea for a minor enhancement"));
					break;

				case TypeOfFeedback.Appreciation:
					if (_cboAffects.Items.Count == (int)AffectedArea.Unknown)
						_cboAffects.Items.Insert(0, notApplicableItem);
					PriorityIsRelevant = false;
					break;

				case TypeOfFeedback.Donate:
					PriorityIsRelevant = false;
					_lblDescription.Visible = false;
					_richTextBoxDescription.Visible = false;
					_lblAffects.Visible = false;
					_cboAffects.Visible = false;
					_lblProject.Visible = false;
					_cboProjectOrWebsite.Visible = false;
					_linkDonate.Visible = true;
					return;
				default:
					throw new IndexOutOfRangeException("Unexpected type of feedback.");
			}
		}

		private void _btnOk_Click(object sender, EventArgs e)
		{
			if (FeedbackType == TypeOfFeedback.Donate)
			{
				var msg = LocalizationManager.GetString("GiveFeedbackDlg.DonationConfirmation",
					"Visit donation page now?");
				if (MessageBox.Show(msg, ProductName, MessageBoxButtons.YesNo) == DialogResult.No)
					DialogResult = DialogResult.Cancel;
			}
		}
		#endregion

		#region private helper methods
		private void RemoveNotApplicableItemFromAffectsCombo()
		{
			if (_cboAffects.Items.Count > (int)AffectedArea.Unknown)
				_cboAffects.Items.RemoveAt(0);
		}
		
		private void OpenDonationPage()
		{
			DialogResult = DialogResult.OK;
			Close();
		}
		#endregion
	}
}
