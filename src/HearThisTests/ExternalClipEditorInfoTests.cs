using NUnit.Framework;
using HearThis;
using System.IO;
using System.Linq;
using static System.Environment;

namespace HearThisTests
{
	[TestFixture]
	public class ExternalClipEditorInfoTests
	{
		ExternalClipEditorInfo sut;

		[SetUp]
		public void Setup()
		{
			sut = ExternalClipEditorInfo.GetTestInstance();
		}

		[TestCase("")]
		[TestCase("   ")]
		[TestCase(@"Windows\explorer.exe")] // Not rooted
		[TestCase(@"e:\monkey\soup\not_there.dat")] // Non-existent
		public void ApplicationPath_SetToInvalidValue_ApplicationPathAndNameAreNull(string val)
		{
			sut.ApplicationPath = val;
			Assert.That(sut.ApplicationPath, Is.Null);
			// Also, confirm that the ApplicationName was not set
			Assert.That(sut.ApplicationName, Is.Null);
		}

		[Test]
		public void ApplicationPath_SetToValidValue_ApplicationPathAndNameSet()
		{
			var val = GetSomeApplication();
			sut.ApplicationPath = val;
			Assert.That(sut.ApplicationPath, Is.EqualTo(val));
			Assert.That(sut.ApplicationName, Is.Not.Null);
		}

		[Test]
		public void ApplicationName_ApplicationPathSet_ApplicationNameUnchanged()
		{
			const string kName = "ThisIsAFriedMonkey";
			sut.ApplicationName = kName;
			var val = GetSomeApplication();
			sut.ApplicationPath = val;
			Assert.That(sut.ApplicationName, Is.EqualTo(kName));
		}

		[TestCase("")]
		[TestCase("   ")]
		public void ApplicationName_SetToInvalidValue_ApplicationNameIsNull(string val)
		{
			sut.ApplicationName = val;
			Assert.That(sut.ApplicationName, Is.Null);
		}

		[TestCase("")]
		[TestCase("   ")]
		public void CommandLineParameters_SetToInvalidValue_CommandLineParametersIsNull(string val)
		{
			sut.CommandLineParameters = val;
			Assert.That(sut.CommandLineParameters, Is.Null);
		}
		
		[Test]
		public void CommandLineParameters_UsingAssociatedDefaultApp_ThrowsInvalidOperationException()
		{
			sut.UseAssociatedDefaultApplication = true;
			Assert.That(() => { sut.CommandLineParameters = "-i"; }, Throws.InvalidOperationException);
		}

		[TestCase(true)]
		[TestCase(false)]
		public void CommandWithParameters_NotSpecified_ReturnsNull(bool setCommandLineParameters)
		{
			if (setCommandLineParameters)
				sut.CommandLineParameters = "-i";
			Assert.That(sut.IsSpecified, Is.False);
			Assert.That(sut.CommandWithParameters, Is.Null);
		}

		[Test]
		public void CommandWithParameters_CommandLineParametersNotSet_ReturnsApplicationPath()
		{
			var val = GetSomeApplication();
			sut.ApplicationPath = val;
			Assert.That(sut.CommandWithParameters, Is.EqualTo(sut.ApplicationPath));
		}

		[TestCase("-i")]
		[TestCase(" -i")]
		[TestCase("  -i")]
		[TestCase(" -i ")]
		public void CommandWithParameters_CommandLineParametersSet_ReturnsApplicationPathWithParameters(string parameters)
		{
			var val = GetSomeApplication();
			sut.ApplicationPath = val;
			sut.CommandLineParameters = parameters;
			Assert.That(sut.CommandWithParameters, Is.EqualTo(sut.ApplicationPath + " -i"));
		}

		[Test]
		public void GetCommandToOpen_NoCommandLineParameters_ReturnsApplicationPathWithAudioFilePathAsArguments()
		{
			var val = GetSomeApplication();
			sut.ApplicationPath = val;
			Assert.That(sut.GetCommandToOpen(@"c:\frog\soup.wav", out var arguments), Is.EqualTo(sut.ApplicationPath));
			Assert.That(arguments, Is.EqualTo("\"c:\\frog\\soup.wav\""));
		}

		[Test]
		public void GetCommandToOpen_CommandLineParametersBasic_ReturnsApplicationPathWithParametersAndAudioFilePathAsArguments()
		{
			var val = GetSomeApplication();
			sut.ApplicationPath = val;
			sut.CommandLineParameters = "-i";
			Assert.That(sut.GetCommandToOpen(@"c:\frog\soup.wav", out var arguments), Is.EqualTo(sut.ApplicationPath));
			Assert.That(arguments, Is.EqualTo("-i \"c:\\frog\\soup.wav\""));
		}

		[Test]
		public void GetCommandToOpen_CommandLineParametersWithPathParams_ReturnsApplicationPathWithAudioFilePathSubstitutedProperlyAsArguments()
		{
			var val = GetSomeApplication();
			sut.ApplicationPath = val;
			sut.CommandLineParameters = "-i {path} -o{path}";
			Assert.That(sut.GetCommandToOpen(@"c:\frog\soup.wav", out var arguments), Is.EqualTo(sut.ApplicationPath));
			Assert.That(arguments, Is.EqualTo("-i \"c:\\frog\\soup.wav\" -o\"c:\\frog\\soup.wav\""));
		}

		[Test]
		public void GetCommandToOpen_UsingAssociatedDefaultApp_ReturnsAudioFilePath()
		{
			sut.UseAssociatedDefaultApplication = true;
			Assert.That(sut.GetCommandToOpen(@"c:\frog\soup.wav", out var arguments), Is.EqualTo("\"c:\\frog\\soup.wav\""));
			Assert.That(arguments, Is.Null);
		}

		private static string GetSomeApplication()
		{
			var programFilesDir = GetFolderPath(SpecialFolder.ProgramFiles);
			var val = Directory.GetDirectories(programFilesDir)
				.Select(d => Directory.GetFiles(d, "*.exe").FirstOrDefault()).FirstOrDefault();

			if (val == null)
				Assert.Ignore("Setup problem. No executable found in Program Files.");
			return val;
		}
	}
}
