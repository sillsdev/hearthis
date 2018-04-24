using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HearThis.UI
{
	/// <summary>
	/// This simple dialog provides a brief explanation of what HearThisPacks are for
	/// and a home for the control that allows choosing to limit the pack to the current
	/// actor.
	/// </summary>
	public partial class SaveHearThisPackDlg : Form
	{
		private string _actor;
		private string _originalLabelText;

		public SaveHearThisPackDlg()
		{
			InitializeComponent();
			_originalLabelText = _limitToCurrentActor.Text;
		}

		public string Actor
		{
			get { return _actor; }
			set
			{
				_actor = value;
				_limitToCurrentActor.Text = string.Format(_originalLabelText, _actor);
				_limitToCurrentActor.Visible = !string.IsNullOrEmpty(_actor);
			}
		}

		public bool LimitToActor => _limitToCurrentActor.Checked;
	}
}
