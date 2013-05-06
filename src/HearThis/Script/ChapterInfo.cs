using System.IO;
using HearThis.Properties;
using HearThis.Publishing;
using Palaso.IO;

namespace HearThis.Script
{
	public class ChapterInfo
	{
		private readonly string _projectName;
		private readonly string _bookName;
		private readonly int _bookNumber;
		public readonly int ChapterNumber1Based;
		public readonly int VersesPossible;
		private readonly IScriptProvider _scriptProvider;

		/// <summary>
		///
		/// </summary>
		/// <param name="projectName"></param>
		/// <param name="bookName"></param>
		/// <param name="bookNumber"></param>
		/// <param name="chapterNumber1Based">[0] == intro, [1] == chapter 1, etc.</param>
		/// <param name="versesPossible"></param>
		/// <param name="scriptProvider"></param>
		public ChapterInfo(string projectName, string bookName, int bookNumber, int chapterNumber1Based, int versesPossible, IScriptProvider scriptProvider)
		{
			_projectName = projectName;
			_bookName = bookName;
			_bookNumber = bookNumber;
			ChapterNumber1Based = chapterNumber1Based;
			VersesPossible = versesPossible;
			_scriptProvider = scriptProvider;
		}

		public bool IsEmpty
		{
			get { return GetScriptLineCount() == 0; }
		}

		public int GetScriptLineCount()
		{
			return _scriptProvider.GetScriptLineCount(_bookNumber, ChapterNumber1Based);
		}

		public int CalculatePercentageRecorded()
		{
				var repo = new LineRecordingRepository();
			int scriptLineCount = _scriptProvider.GetScriptLineCount(_bookNumber,ChapterNumber1Based);
			if (scriptLineCount == 0)
				return 0;//should it be 0 or 100 or -1 or what?
			return 100* repo.GetCountOfRecordingsForChapter(_projectName, _bookName, ChapterNumber1Based)/scriptLineCount;
		}

		public int CalculatePercentageTranslated()
		{
			 return (_scriptProvider.GetTranslatedVerseCount(_bookNumber, ChapterNumber1Based));
		}

		public void MakeDummyRecordings()
		{
			LineRecordingRepository repository = new LineRecordingRepository();
			using (TempFile sound = new TempFile())
			{
				byte[] buffer = new byte[Resources.think.Length];
				Resources.think.Read(buffer, 0, buffer.Length);
				File.WriteAllBytes(sound.Path, buffer);
				for (int line = 0; line < GetScriptLineCount(); line++)
				{
					var path = repository.GetPathToLineRecording(_projectName, _bookName, ChapterNumber1Based, line);

					if (!File.Exists(path))
					{
						File.Copy(sound.Path, path, false);
					}
				}
			}
		}

		public void RemoveRecordings()
		{
			LineRecordingRepository repository = new LineRecordingRepository();
			var dir = repository.GetChapterFolder(_projectName, _bookName, ChapterNumber1Based);
			Directory.Delete(dir,true);
		}
	}
}