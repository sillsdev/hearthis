using System;
using System.Globalization;
using System.IO;
using System.Text;
using SIL.IO;

namespace HearThis.Communication
{
	/// <summary>
	/// To make various things easier, there is an implementation of IAndroidLink that talks to the Windows ClipRepository.
	/// Only some functions are needed.
	/// </summary>
	class WindowsLink : IAndroidLink
	{
		private readonly string _rootFolderPath;
		public WindowsLink(string rootFolderPath)
		{
			_rootFolderPath = rootFolderPath;
		}

		public string GetDeviceName()
		{
			throw new NotImplementedException();
		}

		public bool GetFile(string sourceInRootFolder, string destPath)
		{
			var source = Path.Combine(_rootFolderPath, sourceInRootFolder);
			if (!RobustFile.Exists(source))
				return false;
			RobustFile.Copy(source, destPath, true);
			return true;
		}

		public bool TryGetData(string androidPath, out byte[] data)
		{
			var path = Path.Combine(_rootFolderPath, androidPath);
			if (!RobustFile.Exists(path))
			{
				data = new byte[0];
				return false;
			}
			data = RobustFile.ReadAllBytes(path);
			return true;
		}

		public bool PutFile(string androidPath, byte[] data)
		{
			RobustFile.WriteAllBytes(Path.Combine(_rootFolderPath, androidPath), data);
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
				var filename = Path.GetFileName(file);
				// REVIEW: We need to consider whether/when changes to info.xml files
				// might need to be regarded as significant for determining which
				// version to use in merge, since the "Check For Problems" view makes
				// it more likely for the info file to change without any clips being
				// modified.
				if (filename == "info.xml")
					continue;
				sb.Append(filename);
				sb.Append(";");
				sb.Append(
					new FileInfo(file).LastWriteTimeUtc.ToString(
						new DateTimeFormatInfo {FullDateTimePattern = "yyyy-MM-dd HH:mm:ss"}));
				sb.Append(";f\n");
			}
			foreach (var dir in Directory.EnumerateDirectories(path))
			{
				sb.Append(Path.GetFileName(dir));
				sb.Append(";");
				sb.Append(
					new DirectoryInfo(dir).LastWriteTimeUtc.ToString(
						new DateTimeFormatInfo { FullDateTimePattern = "yyyy-MM-dd HH:mm:ss" }));
				sb.Append(";d\n");
			}
			list = sb.ToString();
			return true;
		}

		public void DeleteFile(string androidPath)
		{
			if (RobustFile.Exists(androidPath))
				RobustFile.Delete(androidPath);
		}
	}
}
