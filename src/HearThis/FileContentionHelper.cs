using System;
using System.IO;
using System.Reflection;
using System.Threading;
using SIL.Reporting;
using SIL.Xml;
using static System.Diagnostics.Process;
using static System.IO.Path;

namespace HearThis
{
	/// <summary>
	/// This class contains helper methods for minimizing problems when there is contention over
	/// user settings caused by running multiple instances of HearThis. See HT-497. Note that
	/// probably some or all of these problems could be solved (maybe better) by making rigorous
	/// use of the settings lock mutex. I think this would require obtaining the mutex any time
	/// we wanted to read a setting that we were potentially about to change and holding it until
	/// we either changed it or decided not too. In each case, we'd need to handle blocking in
	/// an appropriate way. It might be worth attempting, but I'm hoping this will be adequate to
	/// avoid crashes. In any case, I think there is a final Save that is done at shutdown that we
	/// don't hAve control over, so we need to handle that case too. See HT-503.
	/// </summary>
	internal static class FileContentionHelper
	{
		internal const int kMaxRetries = 3;
		internal static readonly TimeSpan RetryDelay = TimeSpan.FromMilliseconds(500);

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
			for (int attempt = 1; attempt <= kMaxRetries; attempt++)
			{
				T result = XmlSerializationHelper.DeserializeFromFile<T>(filename, out e);
				if (result != null)
					return result;

				if (!(e is IOException))
					break;

				if (attempt < kMaxRetries)
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

		internal static int GetNumberOfInstancesOfHearThisRunning(bool logAndIgnoreExceptions = true)
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
