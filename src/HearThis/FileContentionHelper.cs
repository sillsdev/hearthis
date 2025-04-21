using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using L10NSharp;
using SIL.Reporting;
using SIL.Xml;
using static System.Diagnostics.Process;
using static System.IO.Path;

namespace HearThis
{
	/// <summary>
	/// This class contains helper methods for minimizing problems when there is contention over
	/// user settings cause by running multiple instances of HearThis. See HT-497. Note that
	/// probably some or all of these problems could be solved (maybe better) by making rigorous
	/// use of the settings lock mutex. I think this would require obtaining the mutex any time
	/// we wanted to read a setting that we were potentially about to change and holding it until
	/// we either changed it or decided not too. In each case, we'd need to handle blocking in
	/// an appropriate way. It might be worth attempting, but I'm hoping this will be adequate to
	/// avoid crashes.
	/// </summary>
	internal static class FileContentionHelper
	{
		public static readonly string UpgradeMutexName = "HearThis_Settings_Lock";

		private const int MaxRetries = 3;
		private static readonly TimeSpan RetryDelay = TimeSpan.FromMilliseconds(500);

		/// <summary>
		/// This handles any case where Settings.Default.Save is called directly (after error
		/// handling is set up) without going through the SaveSettings method.
		/// </summary>
		public static void ConditionallyIgnoreConfigurationErrorsException(object sender,
			CancelExceptionHandlingEventArgs e)
		{
			if (!(e.Exception is ConfigurationErrorsException))
				return;

			try
			{
				// If this happens during Save and there are any other HearThis processes running,
				// most likely there was contention over the state of the config file. The last
				// instance to shut down should be able to handle saving it correctly. Report in
				// log file, but don't bother the user.
				var processes = GetNumberOfInstancesOfHearThisRunning(false);
				if (processes > 1)
				{
					// If there are multiple instances running, log the error but don't bother the user
					Logger.WriteError("Error saving HearThis config (probably during shutdown). " +
					                  $"There were {processes} instances running (see HT-497)",
						e.Exception);
					e.Cancel = true;
				}
			}
			catch (Exception exception)
			{
				Logger.WriteError(exception);
			}
		}

		public static void SaveSettings()
		{
			Task.Run(async () =>
			{
				try
				{
					await SaveSettingsAsync();
				}
				catch (TaskCanceledException ex)
				{
					Logger.WriteError("Error saving settings asynchronously", ex);
				}
			});
		}

		public static async Task<bool> SaveSettingsAsync()
		{
			for (int attempt = 1; attempt <= MaxRetries; attempt++)
			{
				try
				{
					Properties.Settings.Default.Save();
					return true; // Success, exit method
				}
				catch (ConfigurationErrorsException e)
				{
					if (attempt < MaxRetries)
					{
						await Task.Delay(RetryDelay); // Wait before retrying
						continue;
					}

					HandleFinalSaveSettingsFailure(e);
				}
			}

			return false;
		}

		private static void HandleFinalSaveSettingsFailure(ConfigurationErrorsException e)
		{
			var processes = GetNumberOfInstancesOfHearThisRunning();
			if (processes > 1)
			{
				Logger.WriteError($"Error saving HearThis user settings. There were {processes} instances running (see HT-497)", e);
				return; // Don't bother the user
			}

			// Report as a non-fatal error
			ErrorReport.ReportNonFatalExceptionWithMessage(e, LocalizationManager.GetString("FailedToSaveSettings",
				"Failed to save settings."));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Deserializes XML from the specified file to an object of the specified type.
		/// </summary>
		/// <typeparam name="T">The object type</typeparam>
		/// <param name="filename">The filename from which to load</param>
		/// ------------------------------------------------------------------------------------
		public static T DeserializeFromFile<T>(string filename)
		{
			var result = DeserializeFromFile<T>(filename, out var e);
			if (result != null)
				return result;
			throw e;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Deserializes XML from the specified file to an object of the specified type.
		/// </summary>
		/// <typeparam name="T">The object type</typeparam>
		/// <param name="filename">The filename from which to load</param>
		/// <param name="e">The exception generated during the deserialization.</param>
		/// ------------------------------------------------------------------------------------
		public static T DeserializeFromFile<T>(string filename, out Exception e)
		{
			e = null;
			for (int attempt = 1; attempt <= MaxRetries; attempt++)
			{
				T result = XmlSerializationHelper.DeserializeFromFile<T>(filename, out e);
				if (result != null)
					return result;

				if (!(e is IOException))
					break;

				if (attempt < MaxRetries)
				{
					Thread.Sleep(RetryDelay);
					continue;
				}

				var processes = GetNumberOfInstancesOfHearThisRunning();
				if (processes > 1)
				{
					e = new Exception($"Error deserializing XML file. There were {processes} " +
						"instances of HearThis running (see HT-499)", e);
				}
			}

			return default;
		}

		private static int GetNumberOfInstancesOfHearThisRunning(bool logAndIgnoreExceptions = true)
		{
			try
			{
				var entryAssemblyPath = Assembly.GetEntryAssembly()?.Location;
				if (entryAssemblyPath == null) // Presumably can't happen, but just in case.
					return 1; // There might be others, but let's hope not.
				var processes = GetProcessesByName(GetFileNameWithoutExtension(entryAssemblyPath));

				// If there are multiple instances running, log the error but don't bother the user
				return processes.Length;
			}
			catch (Exception exception)
			{
				if (logAndIgnoreExceptions)
				{
					Logger.WriteError(exception);
					return 1; // There might be others, but let's hope not.
				}
				throw;
			}
		}
	}
}
