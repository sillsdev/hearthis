// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022-2025, SIL Global.
// <copyright from='2022' to='2025' company='SIL Global'>
//		Copyright (c) 2022-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Unicode;
using JetBrains.Annotations;
using SIL.Unicode;

namespace HearThis
{
	/// <summary>
	/// Convenience class to represent a whitespace character with various string
	/// representations for display purposes.
	/// </summary>
	/// <remarks>This is not specific to HearThis in any way and could be moved to a
	/// common library.</remarks>
	public class WhitespaceCharacter
	{
		public char Character { get; }
		public string Name => UnicodeInfo.GetName(Character);
		public string CodePoint => $"U+{(int)Character:X4}";
		public string LongName =>  $"{Name} ({CodePoint})";

		[PublicAPI]
		public static string ValueMember = nameof(Character);
		[PublicAPI]
		public static string NameMember = nameof(Name);
		[PublicAPI]
		public static string CodePointMember = nameof(CodePoint);
		[PublicAPI]
		public static string LongNameMember = nameof(LongName);

		private static readonly WhitespaceCharacter[] s_list;

		static WhitespaceCharacter()
		{
			s_list = CharacterUtils.GetAllCharactersInUnicodeCategory(UnicodeCategory.SpaceSeparator)
				.Select(c => new WhitespaceCharacter(c)).ToArray();
		}

		private WhitespaceCharacter(char character)
		{
			Character = character;
		}

		public override string ToString() => CodePoint;

		/// <summary>
		/// Collection of all whitespace characters (intended for use as a data source, etc.)
		/// </summary>
		public static IReadOnlyList<WhitespaceCharacter> AllWhitespaceCharacters => s_list;

		public static implicit operator char(WhitespaceCharacter wc) => wc.Character;
		public static explicit operator WhitespaceCharacter(char c) => new WhitespaceCharacter(c);
	}
}
