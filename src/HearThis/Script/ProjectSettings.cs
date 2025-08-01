// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2016-2025, SIL Global.
// <copyright from='2016' to='2025' company='SIL Global'>
//		Copyright (c) 2016-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using HearThis.Properties;
using SIL.Extensions;
using SIL.ObjectModel;

namespace HearThis.Script
{
	[Serializable]
	[XmlRoot(ElementName = "ProjectInfo", Namespace = "", IsNullable = false)]
	public class ProjectSettings
	{
		private const string kDefaultClauseBreakCharacters = ", ; :";
		private static readonly Regex _regexUnicodeCharacter = new Regex(@"\bU\+(?<codepoint>[0-9A-F]{4})\b");

		private bool _newlyCreatedSettingsForExistingProject;
		private IReadOnlySet<char> _additionalBlockBreakCharactersSet;
		private string _cachedAdditionalBlockBreakCharacters;
		private IReadOnlySet<char> _clauseBreakCharactersSet;
		private string _cachedClauseBreakCharacters;

		public ProjectSettings()
		{
			// Set defaults
			ClauseBreakCharacters = kDefaultClauseBreakCharacters;
			AdditionalBlockBreakCharacters = "";

			Version = SafeSettings.Get(() => Settings.Default.CurrentDataVersion);
		}

		/// <summary>
		/// This setting is not persisted, as it is only needed during data migration. It allows us to
		/// correctly handle two edge cases: 1) a project from an early version of HearThis is
		/// getting settings for the very first time or 2) the settings were corrupted in a way that
		/// prevented them from being deserialized. Note that in the latter case, we can't know for
		/// sure which data migration steps need to be done because we don't know which version the
		/// project was at when things went awry.
		/// </summary>
		[XmlIgnore]
		internal bool NewlyCreatedSettingsForExistingProject
		{
			get => _newlyCreatedSettingsForExistingProject;
			set
			{
				_newlyCreatedSettingsForExistingProject = value;
				Version = 0;
			}
		}

		[XmlAttribute("version")]
		public int Version { get; set; }

		[XmlAttribute("breakAtParagraphBreaks")]
		[DefaultValue(false)]
		public bool BreakAtParagraphBreaks { get; set; }

		[XmlAttribute("breakQuotesIntoBlocks")]
		[DefaultValue(false)]
		public bool BreakQuotesIntoBlocks { get; set; }

		[XmlAttribute("clauseBreakCharacters")]
		[DefaultValue(kDefaultClauseBreakCharacters)]
		public string ClauseBreakCharacters{
			get
			{
				if (_cachedClauseBreakCharacters == null)
					_cachedClauseBreakCharacters = CharacterSetToString(_clauseBreakCharactersSet);

				return _cachedClauseBreakCharacters;
			}
			set
			{
				if (_cachedClauseBreakCharacters == value)
					return;

				_cachedClauseBreakCharacters = value ?? kDefaultClauseBreakCharacters;
				_clauseBreakCharactersSet = null;
			}
		}

		[XmlElement(ElementName="RangesToBreakByVerse")]
		public RangesToBreakByVerse RangesToBreakByVerse { get; set; }

		public string ClauseBreakCharactersExcludingWhitespace =>
			CharacterSetToString(ClauseBreakCharacterSet, false);

		[XmlIgnore]
		public IReadOnlySet<char> ClauseBreakCharacterSet
		{
			get
			{
				if (_clauseBreakCharactersSet == null)
					_clauseBreakCharactersSet = StringToCharacterSet(_cachedClauseBreakCharacters ?? "");

				return _clauseBreakCharactersSet;
			}
			set
			{
				if ((value == null && _clauseBreakCharactersSet == null) ||
				    (value != null && _clauseBreakCharactersSet != null &&
					    value.SetEquals(_clauseBreakCharactersSet)))
					return;
				_cachedClauseBreakCharacters = null;
				_clauseBreakCharactersSet = value;
			}
		}

		[XmlAttribute("additionalBlockBreakCharacters")]
		[DefaultValue("")]
		public string AdditionalBlockBreakCharacters
		{
			get
			{
				if (_cachedAdditionalBlockBreakCharacters == null)
					_cachedAdditionalBlockBreakCharacters = CharacterSetToString(_additionalBlockBreakCharactersSet);

				return _cachedAdditionalBlockBreakCharacters;
			}
			set
			{
				if (_cachedAdditionalBlockBreakCharacters == value)
					return;

				_cachedAdditionalBlockBreakCharacters = value ?? "";
				_additionalBlockBreakCharactersSet = null;
			}
		}

		public string AdditionalBlockBreakCharactersExcludingWhitespace =>
			CharacterSetToString(AdditionalBlockBreakCharacterSet, false);

		[XmlIgnore]
		public IReadOnlySet<char> AdditionalBlockBreakCharacterSet
		{
			get
			{
				if (_additionalBlockBreakCharactersSet == null)
					_additionalBlockBreakCharactersSet = StringToCharacterSet(_cachedAdditionalBlockBreakCharacters ?? "");

				return _additionalBlockBreakCharactersSet;
			}
			set
			{
				if ((value == null && _additionalBlockBreakCharactersSet == null) ||
				    (value != null && _additionalBlockBreakCharactersSet != null &&
					    value.SetEquals(_additionalBlockBreakCharactersSet)))
					return;
				_cachedAdditionalBlockBreakCharacters = null;
				_additionalBlockBreakCharactersSet = value;
			}
		}

		/// <summary>
		/// For now this is a string representing the version number of a
		/// single failed data migration. If in the future we have another data
		/// migration step that could fail and require manual intervention, this
		/// could be turned into a delimited list of numbers.
		/// </summary>
		[XmlAttribute("nagAboutDataMigrationReport")]
		[DefaultValue("")]
		public string LastDataMigrationReportNag { get; set; }

		private static string CharacterSetToString(IReadOnlyCollection<char> set, bool includeWhitespaceChars = true)
		{
			var sb = new StringBuilder();
			foreach (var c in set)
			{
				if (char.IsWhiteSpace(c))
				{
					if (includeWhitespaceChars)
						sb.Append(c == ' ' ? "\\s " : $"\\u{(int)c:X4} ");
				}
				else
				{
					sb.Insert(0, $"{c} ");
				}
			}

			if (sb.Length > 1)
				sb.Length--; // Remove trailing space

			return sb.ToString();
		}

		public static IReadOnlySet<char> StringToCharacterSet(string temp)
		{
			ISet<char> set = new HashSet<char>();
			{
				Match match;
				while ((match = _regexUnicodeCharacter.Match(temp)).Success)
				{
					set.Add(char.ConvertFromUtf32(int.Parse(match.Groups["codepoint"].Value, NumberStyles.AllowHexSpecifier))[0]);
					temp = temp.Remove(match.Index, match.Length);
				}

				temp = temp.Replace(" ", string.Empty);
				set.AddRange(temp.ToHashSet());
				return new ReadOnlySet<char>(set);
			}
		}

		public static char GetSpaceChar(string s)
		{
			var match = _regexUnicodeCharacter.Match(s);
			if (match.Success)
				return char.ConvertFromUtf32(int.Parse(match.Groups["codepoint"].Value, NumberStyles.AllowHexSpecifier))[0];

			throw new ArgumentException($"String \"{s}\"does not represent a Unicode space character (U+XXXX)", nameof(s));
		}
	}
}
