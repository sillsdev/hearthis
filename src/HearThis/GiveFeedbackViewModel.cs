using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Diagnostics;

namespace HearThis
{
	public enum TypeOfFeedback
	{
		Problem = 0,
		Suggestion = 1,
		Appreciation = 2,
		Donate = 3
	}

	public enum ProblemSeverity
	{
		LostData,
		Blocker,
		Major,
		Minor
	}

	public enum Area
	{
		NotApplicable,
		DataSharing,
		Exporting,
		Android,
		Installation,
		Localization,
		Multiple,
		Navigation,
		Other,
		Playback,
		ProjectAdministration,
		ProjectSelection,
		Recording,
		Settings,
		Website,
		Unknown
	}

	public class GiveFeedbackViewModel
	{
		public string Title { get; set; }
		public TypeOfFeedback Type { get; set; }
		public ProblemSeverity Severity { get; set; }
		public string DescriptionAsRTF { get; set; }
		public Area AffectedArea { get; set; }
		public string AffectedProject { get; set; }
		public string WebsiteUrl { get; set; }
		public Form MainForm { get; }
		public string PathToLastClip { get; }
		public string MainFormWindowTitle => MainForm.Text;
		public bool IncludeScreenShot { get; set; }
		public bool IncludeLastClip { get; set; }
		public bool IncludeLogFile { get; set; }

		public GiveFeedbackViewModel(Form mainForm, string pathToLastClip)
		{
			MainForm = mainForm;
			PathToLastClip = pathToLastClip;
		}

		public void GoToCommunitySite()
		{
			Process.Start($"https://{Program.kSupportUrlSansHttps}");
		}

		public void IssueFeedback()
		{
			switch (Type)
			{
				case TypeOfFeedback.Problem:
					if (ReportProblemViaRestApi())
						return;
					break;

				case TypeOfFeedback.Donate:
					Process.Start("https://donate.givedirect.org/?cid=13536&n=289");
					break;
			}
		}

		private byte[] Screenshot()
		{
			return null;
		}

		private bool ReportProblemViaRestApi()
		{
			// https://www.educative.io/answers/how-to-create-a-json-string-in-c-sharp
			// https://docs.atlassian.com/software/jira/docs/api/REST/8.22.6/#issue-getEditIssueMeta
			// https://jira.sil.org/rest/api/latest/issue/createmeta/HT/issuetypes
			return false;
		}
	}

	// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

    public class Component
    {
        public string id { get; set; }
    }

    public class Content
    {
        public string type { get; set; }
        public List<Content> content { get; set; }
        public string text { get; set; }
    }
	
    public class Description
    {
        public string type { get; set; }
        public int version { get; set; }
        public List<Content> content { get; set; }
    }

    public class SystemEnvironment
    {
        public string type { get; set; }
        public int version { get; set; }
        public List<Content> content { get; set; }
    }

    public class Fields
    {
        public string summary { get; set; }
        public Issuetype issuetype { get; set; }
        public List<Component> components { get; set; }
        public JiraProject project { get; set; }
        public Description description { get; set; }
        public Reporter reporter { get; set; }
        public Priority priority { get; set; }
        public List<string> labels { get; set; }
        public Security security { get; set; }
        public SystemEnvironment environment { get; set; }
        public List<HtVersion> versions { get; set; }
    }

    public class Issuetype
    {
		[JsonPropertyName("id")]
        public string Id { get; set; }
    }

    public class Priority
    {
		[JsonPropertyName("id")]
        public string Id { get; set; }
    }

    public class JiraProject
    {
	    [JsonPropertyName("id")]
	    public string Id => "HT";
    }

    public class Reporter
    {
		[JsonPropertyName("id")]
        public string Id { get; set; }
    }

    public class Root
    {
        public Update update { get; set; }
        public Fields fields { get; set; }
    }

    public class Security
    {
		[JsonPropertyName("id")]
        public string Id { get; set; }
    }

    public class Update
    {
    }

    public class HtVersion
    {
		[JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
