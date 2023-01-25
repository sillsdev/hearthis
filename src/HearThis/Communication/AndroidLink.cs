// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International. All Rights Reserved.
// <copyright from='2014' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
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

		public bool SendNotification(string message)
		{
			WebClient myClient = new WebClient();
			myClient.UploadData(_address + "/notify?message=" + Uri.EscapeDataString(message), new byte[] {0});
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
