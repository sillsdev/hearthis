// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International. All Rights Reserved.
// <copyright from='2011' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.IO;

namespace HearThis.Publishing
{
	/// <summary>
	/// From the nscribe manual:
	/// All audio content that you wish to use with the nScribe™ software must be in the form of mono, 16 bit, 44.1 khz, WAV (.wav) files.
	/// If you plan to use ‘Auto-Arrange Nested’ function (see next section), make sure to give the audio tag file the same name as the tier folder name and add the three-letter prefix “TAG”. For example, for a tier named “Luke” create an audio tag named “TAGLuke.wav”. See Appendix B for more information.
	/// </summary>
	public class MegaVoicePublishingMethod : HierarchicalPublishingMethodBase
	{
		private readonly Dictionary<string, int> filesOutput = new Dictionary<string, int>();

		private readonly Dictionary<string, List<int>> hashTable = new Dictionary<string, List<int>>();

		public MegaVoicePublishingMethod() : base(new WavEncoder())
		{
		}

		public override string GetFilePathWithoutExtension(string rootFolderPath, string bookName, int chapterNumber)
		{
			// Megavoice requires files numbered sequentially from 001 for each book.
			int fileNumber;
			filesOutput.TryGetValue(bookName, out fileNumber); // if not found it will be zero.

			fileNumber = GetUniqueNameForChapter(bookName, chapterNumber);

			filesOutput[bookName] = fileNumber;
			string chapterIndex = fileNumber.ToString("000");
			string fileName = string.Format("{0}-{1}",  bookName, chapterIndex);

			var path = GetFolderPath(rootFolderPath, bookName);
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			return Path.Combine(path, fileName);
		}

		protected override string FolderFormat
		{
			get { return "{1}-{0}"; }
		}

		public override string RootDirectoryName
		{
			get { return "MegaVoice"; }
		}

		/// <summary>
		/// get the unique file name for Megavoice sequential naming.
		/// </summary>
		/// <param name="bookName"></param>
		/// <param name="chapterNumber"></param>
		/// <returns></returns>
		private int GetUniqueNameForChapter(string bookName, int chapterNumber)
		{
			List<int> list;

			//if book already exists
			if (hashTable.TryGetValue(bookName, out list))
			{
				// if the chapter already exists
				if (list.Contains(chapterNumber))
					return list.IndexOf(chapterNumber) + 1;

				list.Add(chapterNumber);
				return list.IndexOf(chapterNumber) + 1;
			}

			list = new List<int>();
			list.Add(chapterNumber);
			hashTable.Add(bookName, list);
			return list.IndexOf(chapterNumber) + 1;
		}
	}
}
