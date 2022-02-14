// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International. All Rights Reserved.
// <copyright from='2022' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Unicode;
using SIL.Unicode;

namespace HearThis
{
	public class WhitespaceCharacter
	{
		public char Character { get; }
		public string Name => UnicodeInfo.GetName(Character);
		public string CodePoint => $"U+{(int)Character:X4}";
		public string LongName =>  $"{CodePoint} {Name}";

		public static string ValueMember = nameof(Character);
		public static string NameMember = nameof(Name);
		public static string CodePointMember = nameof(CodePoint);
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

		public static IReadOnlyList<WhitespaceCharacter> AllWhitespaceCharacters => s_list;

		public static implicit operator char(WhitespaceCharacter wc) => wc.Character;
		public static explicit operator WhitespaceCharacter(char c) => new WhitespaceCharacter(c);
	}
}
