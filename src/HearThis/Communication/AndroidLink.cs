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
		public string GetDeviceName()
		{
			WebClient myClient = new WebClient();
			// Todo: we need a way to discover the port. Conceivably more than one android is active and we need to
			// choose between them.
			Stream response = myClient.OpenRead("http://192.168.1.72:8087");
			string result;
			using (var reader = new StreamReader(response, Encoding.UTF8))
			{
				result = reader.ReadToEnd();
			}
			response.Close();
			return result;
		}
	}
}
