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
	interface IAndroidLink
	{
		string GetDeviceName();
		bool GetFile(string androidPath, string destPath);
	}
}
