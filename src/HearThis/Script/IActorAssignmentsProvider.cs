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
	}
}
