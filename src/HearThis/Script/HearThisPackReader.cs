// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2025, SIL Global. All Rights Reserved.
// <copyright from='2017' to='2025' company='SIL Global'>
//		Copyright (c) 2025, SIL Global. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using HearThis.Communication;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using System.Linq;

namespace HearThis.Script
{
	/// <summary>
	/// This class is responsible for reading a HearThisPack, which is a zip file of another
	/// HearThis project directory (or a subset of it). The purpose of such a pack is to merge its
	/// data into the corresponding project directory of this HearThis project, so the reader class
	/// exposes the data using the <see cref="IAndroidLink"/>> interface originally developed for
	/// synchronization over a network. The class is disposable so that the temporary folder made
	/// by extracting the zip file can be deleted when no longer needed.
	/// </summary>
	public class HearThisPackReader : IDisposable
	{
		private string _sourcePath;
		private readonly string _tempFolderPath;

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

			new FastZip().ExtractZip(_sourcePath, _tempFolderPath, null);
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
