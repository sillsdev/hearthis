using System;
using NUnit.Framework;
using HearThis;
using System.IO;
using System.Linq;
using static System.Environment;
using static System.IO.Path;

namespace HearThisTests
{
	[TestFixture]
	public class ExternalClipEditorInfoTests
	{
		private ExternalClipEditorInfo _sut;
		private int _numberOfSettingsChangedCalls;

		[SetUp]
		public void Setup()
		{
			_sut = new ExternalClipEditorInfo();
			_numberOfSettingsChangedCalls = 0;
			_sut.SettingsChanged += HandleSUTSettingsChanged;
		}

		private void HandleSUTSettingsChanged(ExternalClipEditorInfo sender)
		{
			Assert.That(sender, Is.EqualTo(_sut));
			_numberOfSettingsChangedCalls++;
		}

		[TestCase("")]
		[TestCase("   ")]
		[TestCase(@"Windows\explorer.exe")] // Not rooted
		[TestCase(@"e:\monkey\soup\not_there.dat")] // Non-existent
		public void ApplicationPath_SetToInvalidValue_ApplicationPathAndNameAreNull(string val)
		{
			_sut.ApplicationPath = val;
			Assert.That(_sut.ApplicationPath, Is.Null);
			// Also, confirm that the ApplicationName was not set:
			Assert.That(_sut.ApplicationName, Is.Null);
			// Also, confirm that this was not treated as a settings change:
			Assert.That(_numberOfSettingsChangedCalls, Is.EqualTo(0));
		}

		[Test]
		public void ApplicationPath_SetToValidValue_ApplicationPathAndNameSet()
		{
			var val = GetSomeApplication();
			_sut.ApplicationPath = val;
			Assert.That(_sut.ApplicationPath, Is.EqualTo(val));
			Assert.That(_sut.ApplicationName, Is.Not.Null);
			Assert.That(_sut.ApplicationPath.ToLowerInvariant(),
				Does.Contain(_sut.ApplicationName.ToLowerInvariant()));
			// Also, confirm that this raised the SettingsChange event:
			Assert.That(_numberOfSettingsChangedCalls, Is.EqualTo(1));
		}

		[Test]
		public void ApplicationPath_ChangeWithDefaultApplicationName_ApplicationNameChanged()
		{
			var val = GetSomeApplication();
			_sut.ApplicationPath = val;
			Assert.That(_sut.ApplicationPath, Is.EqualTo(val));
			val = GetSomeApplication(otherThan:val);
			_sut.ApplicationPath = val;
			Assert.That(_sut.ApplicationPath, Is.EqualTo(val));
			Assert.That(_sut.ApplicationName, Is.Not.Null);
			Assert.That(_sut.ApplicationName, Is.EqualTo(_sut.GetDefaultApplicationNameFromPath()));
		}

		[Test]
		public void ApplicationPath_ChangeWithCustomApplicationName_ApplicationNameNotChanged()
		{
			var val = GetSomeApplication();
			_sut.ApplicationPath = val;
			Assert.That(_sut.ApplicationPath, Is.EqualTo(val));
			_sut.ApplicationName = "My external program";
			val = GetSomeApplication(otherThan:val);
			_sut.ApplicationPath = val;
			Assert.That(_sut.ApplicationPath, Is.EqualTo(val));
			Assert.That(_sut.ApplicationName, Is.EqualTo("My external program"));
		}

		[Test]
		public void ApplicationName_ApplicationPathSet_ApplicationNameUnchanged()
		{
			const string kName = "ThisIsAFriedMonkey";
			_sut.ApplicationName = kName;
			var val = GetSomeApplication();
			_sut.ApplicationPath = val;
			Assert.That(_sut.ApplicationName, Is.EqualTo(kName));
			Assert.That(_numberOfSettingsChangedCalls, Is.EqualTo(2));
		}

		[TestCase("")]
		[TestCase("   ")]
		public void ApplicationName_SetToInvalidValue_ApplicationNameIsNull(string val)
		{
			_sut.ApplicationName = val;
			Assert.That(_sut.ApplicationName, Is.Null);
			// Also, confirm that this was not treated as a settings change:
			Assert.That(_numberOfSettingsChangedCalls, Is.EqualTo(0));
		}

		[TestCase("")]
		[TestCase("   ")]
		public void CommandLineParameters_SetToInvalidValue_CommandLineParametersIsNull(string val)
		{
			_sut.CommandLineParameters = val;
			Assert.That(_sut.CommandLineParameters, Is.Null);
			// Also, confirm that this was not treated as a settings change:
			Assert.That(_numberOfSettingsChangedCalls, Is.EqualTo(0));
		}
		
		[Test]
		public void CommandLineParameters_UsingAssociatedDefaultApp_ThrowsInvalidOperationException()
		{
			_sut.UseAssociatedDefaultApplication();
			Assert.That(() => { _sut.CommandLineParameters = "-i"; }, Throws.InvalidOperationException);
			// Also, confirm that neither of these was treated as a settings change:
			Assert.That(_numberOfSettingsChangedCalls, Is.EqualTo(0));
		}
		
		[Test]
		public void UseAssociatedDefaultApplication_WithNonDefaultAppSpecifiedPreviously_FiresSettingsChangedEvent()
		{
			_sut.ApplicationPath = GetSomeApplication();
			_numberOfSettingsChangedCalls = 0;
			_sut.UseAssociatedDefaultApplication();
			Assert.That(_numberOfSettingsChangedCalls, Is.EqualTo(1));
		}

		[Test]
		public void GetCommandToOpen_NoCommandLineParameters_ReturnsApplicationPathWithAudioFilePathAsArguments()
		{
			var val = GetSomeApplication();
			_sut.ApplicationPath = val;
			Assert.That(_sut.GetCommandToOpen(@"c:\frog\soup.wav", out var arguments), Is.EqualTo(_sut.ApplicationPath));
			Assert.That(arguments, Is.EqualTo("\"c:\\frog\\soup.wav\""));
		}

		[TestCase("-i")]
		[TestCase(" -i")]
		[TestCase("  -i")]
		[TestCase(" -i ")]
		public void GetCommandToOpen_CommandLineParametersBasic_ReturnsApplicationPathWithParametersAndAudioFilePathAsArguments(string parameters)
		{
			var val = GetSomeApplication();
			_sut.ApplicationPath = val;
			_sut.CommandLineParameters = parameters;
			Assert.That(_sut.GetCommandToOpen(@"c:\frog\soup.wav", out var arguments), Is.EqualTo(_sut.ApplicationPath));
			Assert.That(arguments, Is.EqualTo("-i \"c:\\frog\\soup.wav\""));
		}

		[Test]
		public void GetCommandToOpen_CommandLineParametersWithPathParams_ReturnsApplicationPathWithAudioFilePathSubstitutedProperlyAsArguments()
		{
			var val = GetSomeApplication();
			_sut.ApplicationPath = val;
			_sut.CommandLineParameters = "-i {path} -o{path}";
			Assert.That(_sut.GetCommandToOpen(@"c:\frog\soup.wav", out var arguments), Is.EqualTo(_sut.ApplicationPath));
			Assert.That(arguments, Is.EqualTo("-i \"c:\\frog\\soup.wav\" -o\"c:\\frog\\soup.wav\""));
			Assert.That(_numberOfSettingsChangedCalls, Is.EqualTo(2));
		}

		[Test]
		public void GetCommandToOpen_UsingAssociatedDefaultApp_ReturnsAudioFilePath()
		{
			_sut.UseAssociatedDefaultApplication();
			Assert.That(_sut.GetCommandToOpen(@"c:\frog\soup.wav", out var arguments), Is.EqualTo("\"c:\\frog\\soup.wav\""));
			Assert.That(arguments, Is.Null);
		}

		[Test]
		public void UpdateSettings_CalledOnOtherThanPersistedSingleton_ThrowsInvalidOperationException()
		{
			Assert.That(() => _sut.UpdateSettings(new ExternalClipEditorInfo()), Throws.InvalidOperationException);
		}

		private static string GetSomeApplication(string otherThan = null)
		{
			if (otherThan != null)
				otherThan = GetFileNameWithoutExtension(otherThan).ToLowerInvariant();
			try
			{
				var programFilesDir = GetFolderPath(SpecialFolder.ProgramFiles);
				var directories = Directory.GetDirectories(programFilesDir);
				foreach (var dir in directories)
				{
					try
					{
						var files = Directory.GetFiles(dir, "*.exe");
						foreach (var file in files)
						{
							if (otherThan == null ||
							    GetFileNameWithoutExtension(file).ToLowerInvariant() != otherThan)
							{
								return file;
							}
						}
					}
					catch
					{
						// Skip directories that cause access/IO exceptions
					}
				}
			}
			catch (Exception e)
			{
				// Handle the case where we don't have access to the Program Files directory
				Assert.Ignore("Setup problem accessing Program Files directory: " + e.Message);
			}

			Assert.Ignore("Setup problem. No suitable executable found in Program Files.");

			return null; // Unreachable, but IDE/compiler doesn't know that.
		}
	}
}
