using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace HearThis.Communication
{
	/// <summary>
	/// Defines the method that may be used to communicate with HearThisAndroid
	/// </summary>
	public interface IAndroidLink
	{
		string GetDeviceName();
		bool GetFile(string androidPath, string destPath);
		bool TryGetData(string androidPath, out byte[] data);
		bool PutFile(string androidPath, byte[] data);
		bool TryListFiles(string androidPath, out string list);
		void DeleteFile(string androidPath);
	}
}
