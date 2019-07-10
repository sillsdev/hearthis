using System;
using System.ComponentModel;
using System.Windows.Forms;
using PtxUtils;
using PtxUtils.Progress;
using SIL.Reporting;

namespace HearThis.UI
{
	/// <summary>
	/// Implementation for showing Paratext alert messages (copied from Paratext with minor changes)
	/// </summary>
	public class AlertImpl : Alert
    {
        #region Alert implementation methods
        protected override void ShowLaterInternal(string text, string caption, AlertLevel alertLevel)
        {
            MessageBoxIcon icon = GetIconForAlertLevel(alertLevel);
            ProgressUtils.InvokeLaterOnUIThread(() => MessageBox.Show(text, caption, MessageBoxButtons.OK, icon));
        }

        protected override AlertResult ShowInternal(IComponent owner, string text, string caption,
            AlertButtons alertButtons, AlertLevel alertLevel, AlertDefaultButton defaultButton, bool showInTaskbar)
        {
            Logger.WriteEvent($"Paratext Alert: {text}");

            MessageBoxButtons type = GetButtonsForButtons(alertButtons);
            MessageBoxIcon icon = GetIconForAlertLevel(alertLevel);
            MessageBoxDefaultButton defaultBtn = GetDefaultButtonForDefaultButton(defaultButton);

            DialogResult result = MessageBox.Show((IWin32Window)owner, text, caption, type, icon, defaultBtn,
                showInTaskbar ? MessageBoxOptions.DefaultDesktopOnly : 0);
            switch (result)
            {
                case DialogResult.OK:
                case DialogResult.Yes:
                case DialogResult.Retry:
                    return AlertResult.Positive;
                case DialogResult.No:
                case DialogResult.Abort:
                    return AlertResult.Negative;
                default: // DialogResult.Cancel
                    return AlertResult.Cancel;
            }
        }
        #endregion

        #region Protected helper methods
        protected static MessageBoxButtons GetButtonsForButtons(AlertButtons alertButtons)
        {
            switch (alertButtons)
            {
                case AlertButtons.Ok: return MessageBoxButtons.OK;
                case AlertButtons.OkCancel: return MessageBoxButtons.OKCancel;
                case AlertButtons.YesNo: return MessageBoxButtons.YesNo;
                case AlertButtons.YesNoCancel: return MessageBoxButtons.YesNoCancel;
                case AlertButtons.RetryCancel: return MessageBoxButtons.RetryCancel;
                default: throw new ArgumentException("alertButtons unknown: " + alertButtons);
            }
        }

        protected static MessageBoxIcon GetIconForAlertLevel(AlertLevel alertLevel)
        {
            switch (alertLevel)
            {
                case AlertLevel.Information: return MessageBoxIcon.Information;
                case AlertLevel.Warning: return MessageBoxIcon.Warning;
                case AlertLevel.Error: return MessageBoxIcon.Error;
                case AlertLevel.Question: return MessageBoxIcon.Question;
                default: throw new ArgumentException("alertLevel unknown: " + alertLevel);
            }
        }

        protected static MessageBoxDefaultButton GetDefaultButtonForDefaultButton(AlertDefaultButton alertDefaultButton)
        {
            switch (alertDefaultButton)
            {
                case AlertDefaultButton.Button1: return MessageBoxDefaultButton.Button1;
                case AlertDefaultButton.Button2: return MessageBoxDefaultButton.Button2;
                case AlertDefaultButton.Button3: return MessageBoxDefaultButton.Button3;
                default: throw new ArgumentException("alertDefaultButton unknown: " + alertDefaultButton);
            }
        }
        #endregion
    }
}
