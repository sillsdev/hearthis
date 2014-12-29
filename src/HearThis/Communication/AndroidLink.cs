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
		// Todo: we need a way to discover the port. Conceivably more than one android is active and we need to
		// choose between them.
		private string _address = "http://192.168.1.72:8087";

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
			myClient.DownloadFile(_address + "/getfile?path=" + Uri.EscapeDataString(androidPath), destPath);
			return true;
		}

		public bool TryGetData(string androidPath, out byte[] data)
		{
			WebClient myClient = new WebClient();
			data = myClient.DownloadData(_address + "/getfile?path=" + Uri.EscapeDataString(androidPath));
			return true;
		}

		public bool PutFile(string androidPath, byte[] data)
		{
			WebClient myClient = new WebClient();
			myClient.UploadData(_address + "/putfile?path=" + Uri.EscapeDataString(androidPath), data);
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
	}
}
