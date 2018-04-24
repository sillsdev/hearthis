using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HearThis.Communication;
using Ionic.Zip;

namespace HearThis.Script
{
	/// <summary>
	/// This class is responsible for reading a HearThisPack, which is a zip file of another HearThis's
	/// project directory (or a subset of it). The purpose of such a pack is to merge its data into this
	/// HearThis's corresponding project directory, so the reader class exposes the data using the Link
	/// interface originally developed for synchronization over a network.
	/// The class is disposable so that the temporary folder made by extracting the zip file can be
	/// deleted when no longer needed.
	/// </summary>
	public class HearThisPackReader : IDisposable
	{
		private string _sourcePath;
		private string _tempFolderPath;

		public HearThisPackReader(string sourcePath)
		{
			_sourcePath = sourcePath;
			var tempFolder = Path.GetTempPath();
			int suffix = 0;
			var fileName = Path.GetFileName(_sourcePath);
			do
			{
				_tempFolderPath = Path.Combine(tempFolder, fileName + suffix++);
			} while (Directory.Exists(_tempFolderPath) || File.Exists(_tempFolderPath));

			using (var zip = new ZipFile(_sourcePath, Encoding.UTF8))
			{
				zip.ExtractAll(_tempFolderPath);
			}
		}

		public IAndroidLink GetLink()
		{
			return new WindowsLink(_tempFolderPath);
		}

		/// <summary>
		/// A HearThisPack zip should have one root folder whose name corresponds to the project name
		/// </summary>
		public string ProjectName => Path.GetFileName(Directory.GetDirectories(_tempFolderPath).First());

		public void Dispose()
		{
			if (Directory.Exists(_tempFolderPath))
				Directory.Delete(_tempFolderPath, true);
		}
	}
}
