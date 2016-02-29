using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace HearThis.Communication
{
	/// <summary>
	/// To make various things easier, there is an implementation of IAndroidLink that talks to the Windows ClipRepository.
	/// Only some functions are needed.
	/// </summary>
	class WindowsLink : IAndroidLink
	{
		private string _rootFolderPath;
		public WindowsLink(string rootFolderPath)
		{
			_rootFolderPath = rootFolderPath;
		}

		public string GetDeviceName()
		{
			throw new NotImplementedException();
		}

		public bool GetFile(string androidPath, string destPath)
		{
			throw new NotImplementedException();
		}

		public bool TryGetData(string androidPath, out byte[] data)
		{
			var path = Path.Combine(_rootFolderPath, androidPath);
			if (!File.Exists(path))
			{
				data = new byte[0];
				return false;
			}
			data = File.ReadAllBytes(path);
			return true;
		}

		public bool PutFile(string androidPath, byte[] data)
		{
			File.WriteAllBytes(Path.Combine(_rootFolderPath, androidPath), data);
			return true;
		}

		public bool TryListFiles(string androidPath, out string list)
		{
			var path = Path.Combine(_rootFolderPath, androidPath);
			list = "";
			if (!Directory.Exists(path))
				return false;
			var sb = new StringBuilder();
			foreach (var file in Directory.EnumerateFiles(path, "*.*"))
			{
				sb.Append(Path.GetFileName(file));
				sb.Append(";");
				sb.Append(
					new FileInfo(file).LastWriteTimeUtc.ToString(
						new DateTimeFormatInfo() {FullDateTimePattern = "yyyy-MM-dd HH:mm:ss"}));
				sb.Append(";f\n");
			}
			foreach (var dir in Directory.EnumerateDirectories(path))
			{
				sb.Append(Path.GetFileName(dir));
				sb.Append(";");
				sb.Append(
					new DirectoryInfo(dir).LastWriteTimeUtc.ToString(
						new DateTimeFormatInfo() { FullDateTimePattern = "yyyy-MM-dd HH:mm:ss" }));
				sb.Append(";d\n");
			}
			list = sb.ToString();
			return true;
		}

		public void DeleteFile(string androidPath)
		{
			if (File.Exists(androidPath))
				File.Delete(androidPath);
		}
	}
}
