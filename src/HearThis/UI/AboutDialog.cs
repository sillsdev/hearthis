using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Palaso.Reporting;

namespace HearThis.UI
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Incapsulates the welcome dialog box, in which users may create new projects, or open
	/// existing projects via browsing the file systsem or by choosing a recently used project.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public partial class AboutDialog : Form
	{
		private const int kLogoTextImageTop = 10;

		/// ------------------------------------------------------------------------------------
		public AboutDialog()
		{
			InitializeComponent();

			Font = SystemFonts.MessageBoxFont; //use the default OS UI font
			//_linkProductWebSite.Font = new Font(Font.FontFamily, 9, FontStyle.Bold, GraphicsUnit.Point);
			//_linkSiLWebSite.Font = new Font(Font.FontFamily, 9, FontStyle.Regular, GraphicsUnit.Point);
			//_labelVersionInfo.Font = _linkSiLWebSite.Font;

			_labelVersionInfo.Text = GetVersionInfo(_labelVersionInfo.Text);
		}
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Setups the link label with proper localizations. This method gets called from the
		/// constructor and after strings are localized in the string localizing dialog box.
		/// </summary>
		/// ------------------------------------------------------------------------------------
//		private void LocalizationInitiated()
//		{
//		    _labelVersionInfo.Text = "";// ApplicationContainer.GetVersionInfo(_labelVersionInfo.Text);
//            Palaso.Reporting.
////			LocalizationManager.LocalizeObject(_linkProductWebSite, "AboutDialog.lnkSayMoreWebSite",
////               "Visit the SayMore web site.",
////               locExtender.LocalizationGroup);
////
////			LocalizationManager.LocalizeObject(_linkSiLWebSite, "AboutDialog.lnkSilWebSite",
////			   "SayMore is brought to you by SIL International.",
////			   locExtender.LocalizationGroup);
//
//			var entireWebSiteLink = _linkProductWebSite.Text;
//
//		    var appWebSiteUnderlinedPortion = "HearThis web site";
//
//			_linkProductWebSite.Links.Clear();
//			_linkSiLWebSite.Links.Clear();
//
//			int i = entireWebSiteLink.IndexOf(appWebSiteUnderlinedPortion);
//			if (i >= 0)
//				_linkProductWebSite.Links.Add(i, appWebSiteUnderlinedPortion.Length, "http://hearthis.palaso.org");
//		}

		public static string GetVersionInfo(string fmt)
		{
			var asm = Assembly.GetExecutingAssembly();
			var ver = asm.GetName().Version;
			var file = asm.CodeBase.Replace("file:", string.Empty);
			file = file.TrimStart('/');
			var fi = new FileInfo(file);

			return string.Format(fmt, ver.Major, ver.Minor,
								 ver.Build, fi.CreationTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern));
		}

		/// ------------------------------------------------------------------------------------
		protected override void OnLoad(System.EventArgs e)
		{
			base.OnLoad(e);
			UsageReporter.SendNavigationNotice("AboutBox");
		}

		/// ------------------------------------------------------------------------------------
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			base.OnFormClosing(e);
			//LocalizeItemDlg.StringsLocalized -= LocalizationInitiated;
		}

		/// ------------------------------------------------------------------------------------
		private void HandleWebSiteLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string tgt = e.Link.LinkData as string;

			if (!string.IsNullOrEmpty(tgt))
				System.Diagnostics.Process.Start(tgt);
		}

		/// ------------------------------------------------------------------------------------
		protected override void OnPaintBackground(PaintEventArgs e)
		{
			base.OnPaintBackground(e);

//			// Draw a gradient from top to bottom of window.
//			var rc = new Rectangle(0, 45, ClientSize.Width, ClientSize.Height - 45);
//			using (var br = new LinearGradientBrush(rc, Color.White, AppColors.BarBegin, 90f))
//				e.Graphics.FillRectangle(br, rc);
//
//			// Draw the gradient blue bar.
//			rc = new Rectangle(0, 0, ClientSize.Width, 45);
//			using (var br = new LinearGradientBrush(rc, AppColors.BarBegin, AppColors.BarEnd, 0.0f))
//				e.Graphics.FillRectangle(br, rc);
//
//			// Draw a line at the bottom of the gradient blue bar.
//			using (var pen = new Pen(AppColors.BarBorder))
//				e.Graphics.DrawLine(pen, 0, rc.Bottom, rc.Right, rc.Bottom);
//
//			rc = new Rectangle(new Point(lblSubTitle.Left - 6, 10), Resources.SayMoreText.Size);
//			//rc = new Rectangle(new Point(lblSubTitle.Left - 6, 15), Resources.SayMoreText.Size);
//			//rc.Inflate(-4, -4);
//			e.Graphics.DrawImage(Resources.SayMoreText, rc);

			// Draw the application's logo image.
//			var rc = new Rectangle(new Point(10, 5), Resources.LargeLogo.Size);
//			e.Graphics.DrawImage(Resources.LargeLogo, rc);
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("http://hearthis.palaso.org");
		}

		private void _releaseNotesLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			using(var dlg = new ReleaseNotesWindow())
			{
				dlg.ShowDialog();
			}

		}
	}
}