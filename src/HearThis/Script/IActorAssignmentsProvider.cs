using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HearThis.Script
{
	/// <summary>
	/// An additional interface which may optionally be implemented by a ScriptProvider
	/// (like MultiVoiceScriptProvider) which can provide information about planned actors
	/// and the characters they will record.
	/// </summary>
	public interface IActorCharacterProvider
	{
		IEnumerable<string> Actors { get; }
		IEnumerable<string> GetCharacters(string actor);
		void RestrictToCharacter(string actor, string character);
		// The current one set by RestrictToCharacter
		string Actor { get; }
		string Character { get; }
		// Is the indicated block (in the original sequence) in the set the current character should record?
		bool IsBlockInCharacter(int book, int chapter, int lineno0based);
		// Both startLine and result are unfiltered.
		// Returns startLine if no later line is unrecorded for current character,
		// or if no character filtering is active.
		int GetNextUnrecordedLineInChapterForCharacter(int book, int chapter, int startLine);
		// The next chapter in which the current character has an unrecorded line.
		// If there is something unrecorded for character in the startChapter, return that.
		// If there is nothing after startChapter in the book for this character, return startChapter.
		// If there is no current character, return startChapter.
		int GetNextUnrecordedChapterForCharacter(int book, int startChapter);
	}
}
