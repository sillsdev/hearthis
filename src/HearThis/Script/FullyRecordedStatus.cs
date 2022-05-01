// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International. All Rights Reserved.
// <copyright from='2017' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace HearThis.Script
{
	/// <summary>
	/// This class wraps a data structure recording what actors and characters are fully recorded,
	/// that is, a clip exists for every block for that character or actor.
	/// </summary>
	public class FullyRecordedStatus
	{
		// Keeps track of which provider it stores status about. Currently the only way to know
		// whether an actor is fully recorded is to retrieve its character list and check each
		// of them.
		private IActorCharacterProvider _provider;
		private HashSet<Tuple<string, string>> _status = new HashSet<Tuple<string, string>>();

		public FullyRecordedStatus(IActorCharacterProvider provider)
		{
			_provider = provider;
		}

		public bool AllRecorded(string actor, string character)
		{
			return _status.Contains(Tuple.Create(actor, character));
		}

		public bool AllRecorded(string actor)
		{
			foreach (var character in _provider.GetCharacters(actor))
			{
				if (!AllRecorded(actor, character))
				{
					return false;
				}
			}
			return true;
		}

		public bool Add(string actor, string character)
		{
			return _status.Add(Tuple.Create(actor, character));
		}

		public bool Remove(string actor, string character)
		{
			return _status.Remove(Tuple.Create(actor, character));
		}

		public int Count => _status.Count;
	}
}
