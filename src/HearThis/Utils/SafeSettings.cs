using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Threading.Tasks;
using DesktopAnalytics;
using HearThis.Properties;
using HearThis.UI;
using L10NSharp;
using SIL.IO;
using SIL.Reporting;
using static HearThis.FileContentionHelper;

namespace HearThis
{
	internal static class SafeSettings
	{
		public static readonly string UpgradeMutexName = "HearThis_Settings_Lock";
		private static readonly ConcurrentDictionary<string, object> s_criticalCache;

		static SafeSettings()
		{
			s_criticalCache = new ConcurrentDictionary<string, object>();

			try
			{
				s_criticalCache[nameof(Settings.Default.Project)] =
					Settings.Default.Project;
				s_criticalCache[nameof(Settings.Default.UserInterfaceLanguage)] =
					Settings.Default.UserInterfaceLanguage;
				s_criticalCache[nameof(Settings.Default.UserColorScheme)] =
					Settings.Default.UserColorScheme;
				s_criticalCache[nameof(Settings.Default.ExternalClipEditorPath)] =
					Settings.Default.ExternalClipEditorPath;
				s_criticalCache[nameof(Settings.Default.ExternalClipEditorArguments)] =
					Settings.Default.ExternalClipEditorArguments;
			}
			catch (ConfigurationErrorsException e)
			{
				Logger.WriteError(e);
			}
		}

		public static string Project
		{
			get => GetCritical(nameof(Settings.Default.Project), () => Settings.Default.Project);
			set => SetCritical(nameof(Settings.Default.Project), value);
		}

		public static string UserInterfaceLanguage
		{
			get => GetCritical(nameof(Settings.Default.UserInterfaceLanguage),
				() => Settings.Default.UserInterfaceLanguage);
			set => SetCritical(nameof(Settings.Default.UserInterfaceLanguage), value);
		}

		public static ColorScheme UserColorScheme
		{
			get => GetCritical(nameof(Settings.Default.UserColorScheme),
				() => Settings.Default.UserColorScheme);
			set => SetCritical(nameof(Settings.Default.UserColorScheme), value);
		}

		public static string ExternalClipEditorPath
		{
			get => GetCritical(nameof(Settings.Default.ExternalClipEditorPath),
				() => Settings.Default.ExternalClipEditorPath);
			set => SetCritical(nameof(Settings.Default.ExternalClipEditorPath), value);
		}

		public static string ExternalClipEditorArguments
		{
			get => GetCritical(nameof(Settings.Default.ExternalClipEditorArguments),
				() => Settings.Default.ExternalClipEditorArguments);
			set => SetCritical(nameof(Settings.Default.ExternalClipEditorArguments), value);
		}

		private static T GetCritical<T>(string name, Func<T> getter)
		{
			if (s_criticalCache.TryGetValue(name, out var val))
				return (T)val;

			try
			{
				var value = getter();
				s_criticalCache[name] = value;
				return value;
			}
			catch (ConfigurationErrorsException ex)
			{
				Logger.WriteError($"Error reading setting {name}", ex);
				return default;
			}
		}

		private static void SetCritical<T>(string name, T value)
		{
			s_criticalCache[name] = value;
			try
			{
				Settings.Default[name] = value;
			}
			catch (ConfigurationErrorsException ex)
			{
				Logger.WriteError($"Error writing setting {name}", ex);
				// Let Save() recover later
			}
		}

		internal static T Get<T>(Func<T> getter)
		{
			try
			{
				return getter();
			}
			catch (ConfigurationErrorsException ex)
			{
				RecoverFromCorruptConfig("Getting non-critical setting", ex);
				// Now retry. (Will get default value.)
				return getter();
			}
		}
		
		internal static T Set<T>(Func<T> setterThatAlsoReturnsValue, bool save =  false)
		{
			try
			{
				return setterThatAlsoReturnsValue();
			}
			catch (ConfigurationErrorsException ex)
			{
				if (RecoverFromCorruptConfig("Setting non-critical setting", ex))
					return setterThatAlsoReturnsValue();
				return default;
			}
			finally
			{
				if (save)
					Save();
			}
		}

		public static void Save()
		{
			var task = SaveSettingsAsync();

			task.ContinueWith(t =>
			{
				if (t.IsFaulted)
				{
					Logger.WriteError("Error saving settings asynchronously", t.Exception);
					Analytics.ReportException(t.Exception);
				}
			}, TaskContinuationOptions.OnlyOnFaulted);
		}

		public static async Task<bool> SaveSettingsAsync()
		{
			for (int attempt = 1; attempt <= kMaxRetries; attempt++)
			{
				try
				{
					Settings.Default.Save();
					return true; // Success, exit method
				}
				catch (ConfigurationErrorsException e)
				{
					if (attempt < kMaxRetries)
					{
						await Task.Delay(RetryDelay); // Wait before retrying
						continue;
					}

					HandleFinalSaveSettingsFailure(e);
				}
			}

			return false;
		}

		private static bool HandleFinalSaveSettingsFailure(ConfigurationErrorsException e, bool caughtAtTopLevel =  false)
		{
			var processes = GetNumberOfInstancesOfHearThisRunning();
			if (processes > 1)
			{
				// Log problem, but don't bother the user or attempt to recover. If saving
				// continues to fail, the last running instance of HearThis will attempt
				// recovery.
				Logger.WriteError("Error saving HearThis user settings. " +
					$"There were {processes} instances running (see HT-497)", e);
				return caughtAtTopLevel;
			}

			return RecoverFromCorruptConfig("Settings save failed", e);
		}

		private static bool RecoverFromCorruptConfig(string failedActionDesc,
			ConfigurationErrorsException configException)
		{
			Logger.WriteError($"{failedActionDesc}; attempting recovery", configException);

			try
			{
				RobustFile.Delete(configException.Filename);
				Logger.WriteEvent($"Deleted corrupted settings file: {configException.Filename}");

				foreach (var kvp in s_criticalCache)
					Settings.Default[kvp.Key] = kvp.Value;

				Settings.Default.NeedUpgrade = false;

				Settings.Default.Save();
				Logger.WriteEvent("Recovered and rewrote settings from in-memory cache");
				Analytics.Track("RecoveredFromConfigurationErrorsException");
				return true;
			}
			catch (Exception e)
			{
				Logger.WriteError("Failed to recover settings after config file corruption", e);
				Analytics.ReportException(e);
				// Report as a non-fatal error
				ErrorReport.ReportNonFatalExceptionWithMessage(configException,
					LocalizationManager.GetString("FailedToSaveSettings",
					"Failed to save settings."));
				return false;
			}
		}

		/// <summary>
		/// This handles any case where Settings.Default.Save is called directly (after error
		/// handling is set up) without going through the SafeSettings.Save method.
		/// </summary>
		public static void ConditionallyIgnoreConfigurationErrorsException(object sender,
			CancelExceptionHandlingEventArgs e)
		{
			if (!(e.Exception is ConfigurationErrorsException configException))
				return;

			try
			{
				// If this happens during Save and there are any other HearThis processes running,
				// most likely there was contention over the state of the config file. The last
				// instance to shut down should be able to handle saving it correctly. Report in
				// log file, but don't bother the user.
				Logger.WriteError("Error saving HearThis config (probably during shutdown).",
					configException);
				e.Cancel = HandleFinalSaveSettingsFailure(configException, true);
			}
			catch (Exception exception)
			{
				Logger.WriteError(exception);
			}
		}
	}
}
