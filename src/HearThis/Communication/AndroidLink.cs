using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HearThis.Communication
{
	/// <summary>
	/// The real implementation of talking to an Android device
	/// </summary>
	internal class AndroidLink : IAndroidLink

	{
		private string _address;
		private string _ipAddress;

		public string AndroidAddress {
			get
			{
				return _ipAddress;
			}
			set
			{
				_ipAddress = value;
				_address = "http://" + value + ":8087";
			}
		}

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

		public bool GetFile(string androidPath, string destPath)
		{
			WebClient myClient = new WebClient();
			try
			{
				myClient.DownloadFile(_address + "/getfile?path=" + Uri.EscapeDataString(androidPath), destPath);
			}
			catch (WebException ex)
			{
				var response = ex.Response as HttpWebResponse;
				if (response != null && response.StatusCode == HttpStatusCode.NotFound)
				{
					return false;
				}
				throw;
			}
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
				var response = ex.Response as HttpWebResponse;
				if (response != null && response.StatusCode == HttpStatusCode.NotFound)
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
