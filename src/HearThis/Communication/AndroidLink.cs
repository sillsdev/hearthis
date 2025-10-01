// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2014-2025, SIL Global.
// <copyright from='2014' to='2025' company='SIL Global'>
//		Copyright (c) 2014-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace HearThis.Communication
{
	/// <summary>
	/// The real implementation of talking to an Android device
	/// </summary>
	internal class AndroidLink : IAndroidLink
	{
		private readonly string _address;
		public Func<WebException, string, bool> RetryOnTimeout { get; }

		public AndroidLink(string ipAddress, Func<WebException, string, bool> retryOnTimeout)
		{
			AndroidAddress = ipAddress;
			_address = "http://" + AndroidAddress + ":8087";
			RetryOnTimeout = retryOnTimeout ?? ((ex, path) => false);
		}

		public string AndroidAddress { get; }

		public string GetDeviceName()
		{
			WebClient myClient = new WebClient();
			return GetString(myClient, _address);
		}

		private static string GetString(WebClient myClient, string address)
		{
			Stream response = myClient.OpenRead(address);
			string result;
			using (var reader = new StreamReader(response, Encoding.UTF8))
			{
				result = reader.ReadToEnd();
			}
			response.Close();
			return result;
		}

		private class FileRetrievalWebClient : WebClient
		{
			public static int TimeoutInSeconds { get; set; } = 100;

			protected override WebRequest GetWebRequest(Uri uri)
			{
				var w = base.GetWebRequest(uri);
				w.Timeout = (int) Math.Round(TimeSpan.FromSeconds(TimeoutInSeconds).TotalMilliseconds);
				Debug.WriteLine($"SYNC, AndroidLink...WebRequest, timeout set: {TimeoutInSeconds} secs"); // TEMPORARY
				return w;
			}
		}

		public bool GetFile(string androidPath, string destPath)
		{
			var myClient = new FileRetrievalWebClient();
			bool retry = false;
			do
			{
				try
				{
					myClient.DownloadFile(_address + "/getfile?path=" + Uri.EscapeDataString(androidPath), destPath);
				}
				catch (WebException ex)
				{
					if (ex.Response is HttpWebResponse response)
					{
						if (response.StatusCode == HttpStatusCode.NotFound)
							return false;

						if (response.StatusCode == HttpStatusCode.RequestTimeout)
						{
							retry = RetryOnTimeout.Invoke(ex, androidPath);
							if (retry)
							{
								// Increase the timeout for the retry. Note: This new value will be
								// used for future retrieval attempts as well, so if the increased
								// timeout proves to be the magic bullet, we won't end up nagging them
								// for every file. The default timeout is 100s, so it's already high
								// enough that a timeout should be rare. Although adding 100 more
								// seconds each time feels extreme, if extra time is needed and the
								// user is willing to wait, we might as well give it a good chance of
								// success. Presumably, if they retry more than a couple times,
								// they will  just give up.
								FileRetrievalWebClient.TimeoutInSeconds += 100;
								Debug.WriteLine($"SYNC, AndroidLink.GetFile, timeout set: {FileRetrievalWebClient.TimeoutInSeconds} secs"); // TEMPORARY
								continue;
							}

							return false;
						}
					}
					
					throw;
				}
			} while (retry);

			return true;
		}

		public bool TryGetData(string androidPath, out byte[] data)
		{
			WebClient myClient = new WebClient();
			try
			{
				data = myClient.DownloadData(_address + "/getfile?path=" + Uri.EscapeDataString(androidPath));
			}
			catch (WebException ex)
			{
				if (ex.Response is HttpWebResponse response && response.StatusCode == HttpStatusCode.NotFound)
				{
					data = new byte[0];
					return false;
				}
				throw;
			}
			return true;
		}

		public bool PutFile(string androidPath, byte[] data)
		{
			WebClient myClient = new WebClient();
			myClient.UploadData(_address + "/putfile?path=" + Uri.EscapeDataString(androidPath), data);
			return true;
		}

		public bool SendNotification(string status)
		{
			// Protocol change for communicating sync status to Android.
			// As of September 2025 HT sends *two* notifications, in this order:
			//   - minimum HTA version that implements this revised protocol
			//   - final sync status
			WebClient myClient = new WebClient();

			// TODO: replace hardcoded "1.0" version number with a variable.
			myClient.UploadData(_address + "/notify?minHtaVersion=" + Uri.EscapeDataString("1.0"), new byte[] {0});

			// WM, to test Android's timeout behavior: comment out the next line, causing PC to never finish sync.
			myClient.UploadData(_address + "/notify?status=" + Uri.EscapeDataString(status), new byte[] {0});

			return true;
		}

		/// <summary>
		/// The string returned has a line (\n separated) for each file or directory in the specified
		/// directory, or is empty if the specified item does not exist or is not a directory.
		/// Each line gives name;date;d/f, where date is yyyy-MM-dd HH:mm:ss and d indicates a
		/// directory, f a file.
		/// </summary>
		/// <param name="androidPath"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		public bool TryListFiles(string androidPath, out string list)
		{
			WebClient myClient = new WebClient();
			list = GetString(myClient, _address + "/list?path=" + Uri.EscapeDataString(androidPath));
			return true;
		}

		/// <summary>
		/// Leave unimplemented for now. We don't currently need to delete files on the remote android.
		/// </summary>
		/// <param name="androidPath"></param>
		public void DeleteFile(string androidPath)
		{
			throw new NotImplementedException();
		}
	}
}
