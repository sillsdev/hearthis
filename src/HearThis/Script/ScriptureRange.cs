// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2023-2025, SIL Global.
// <copyright from='2023' to='2025' company='SIL Global'>
//		Copyright (c) 2023-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Xml.Serialization;
using SIL.Scripture;
using System.Collections.Generic;

namespace HearThis.Script
{
	/// <summary>
	/// A range of verse references defined by the start and end reference, where both start and
	/// end are considered to be included.
	/// </summary>
	[XmlRoot(ElementName="ScriptureRange")]
	public class ScriptureRange : IEquatable<ScriptureRange>, IComparable<ScriptureRange>, IComparable
	{
		[XmlIgnore]
		public BCVRef StartRef { get; private set; }
		[XmlIgnore]
		public BCVRef EndRef { get; private set; }

		/// <summary>
		/// This is needed to avoid having to know about or serialize the underlying classes
		/// (versification and book set info) that know how to find the previous and next verse.
		/// If the details of the underlying project were to change, I assume that the previously
		/// saved ranges would still be correct since they should reflect an understanding of what
		/// is actually happening in the text.)
		/// </summary>
		public static IScrVerseIterationHelper VerseIterationHelper { get; set; }

		/// <summary>
		/// Constructor needed for deserialization.
		/// </summary>
		public ScriptureRange()
		{
			SetStartAndEndRef(new BCVRef(), new BCVRef());
		}

		public ScriptureRange(BCVRef start, BCVRef end)
		{
			SetStartAndEndRef(start, end);
		}

		private void SetStartAndEndRef(BCVRef start, BCVRef end)
		{
			if (start.CompareTo(end) > 0)
			{
				// ReSharper disable once LocalizableElement
				throw new ArgumentException($"{nameof(end)} must be greater than or equal to {nameof(start)}.",
					nameof(end));
			}

			StartRef = start;
			EndRef = end;
		}

		/// <summary>
		/// Start reference as a BBBCCCVVV integer. DO NOT use setter in code. Intended only for
		/// deserialization!
		/// </summary>
		[XmlAttribute(AttributeName="start")]
		public int Start
		{
			get => StartRef.BBCCCVVV;
			// If the Start value is being set during deserialization, we don't want it to throw an
			// exception if End still has the default value of 0, so we (temporarily) set the End
			// value to match the Start, knowing that the End will be re-set as deserialization
			// proceeds.
			set => SetStartAndEndRef(value, End == default ? value : End);
		}

		/// <summary>
		/// End reference as a BBBCCCVVV integer. DO NOT use setter in code. Intended only for
		/// deserialization!
		/// </summary>
		[XmlAttribute(AttributeName = "end")]
		public int End
		{
			get => EndRef.BBCCCVVV;
			set => SetStartAndEndRef(Start, value);
		}

		public bool Includes(ScriptureRange other)
		{
			return Includes(other.StartRef) && Includes(other.EndRef);
		}

		public bool Includes(BCVRef bcvRef)
		{
			if (bcvRef == null)
				return false;

			return bcvRef.CompareTo(StartRef) >= 0 && bcvRef.CompareTo(EndRef) <= 0;
		}

		public bool Touches(ScriptureRange other)
		{
			return (VerseIterationHelper.TryGetPreviousVerse(other.Start, out var verseBeforeStartOfOtherRange) &&
					verseBeforeStartOfOtherRange.CompareTo(EndRef) == 0) ||
				(VerseIterationHelper.TryGetNextVerse(other.End, out var verseAfterEndOfOtherRange) &&
					verseAfterEndOfOtherRange.CompareTo(StartRef) == 0);
		}

		public bool Overlaps(ScriptureRange other)
		{
			// Calling _range.Overlaps(other._range) would work except that that method checks for "empty" ranges.
			return other.Start < End && other.End > Start;
		}

		/// <inheritdoc />
		public override bool Equals(object obj) => this.Equals(obj as ScriptureRange);

		public static bool operator <(ScriptureRange left, ScriptureRange right) => left.CompareTo(right) < 0;
		public static bool operator >(ScriptureRange left, ScriptureRange right) => left.CompareTo(right) > 0;
		public static bool operator <=(ScriptureRange left, ScriptureRange right) => left.CompareTo(right) <= 0;
		public static bool operator >=(ScriptureRange left, ScriptureRange right) => left.CompareTo(right) >= 0;

		public int CompareTo(ScriptureRange other)
		{
			if (other == null)
				return 1;
			int num = StartRef.CompareTo(other.StartRef);
			if (num == 0)
				num = EndRef.CompareTo(other.EndRef);
			return num;
		}

		public int CompareTo(object obj)
		{
			return !(obj is ScriptureRange otherRange) ? 1 : CompareTo(otherRange);
		}

		public bool Equals(ScriptureRange other)
		{
			return CompareTo(other) == 0;
		}

		public override int GetHashCode()
		{
			int hashCode = -1649319789;
			// StartRef and EndRef are not readonly because they can be changed during
			// deserialization, but they should never be changed in normal code.
			// ReSharper disable once NonReadonlyMemberInGetHashCode
			hashCode = hashCode * -1521134295 + EqualityComparer<BCVRef>.Default.GetHashCode(StartRef);
			// ReSharper disable once NonReadonlyMemberInGetHashCode
			hashCode = hashCode * -1521134295 + EqualityComparer<BCVRef>.Default.GetHashCode(EndRef);
			return hashCode;
		}

		private BCVRef GetPreviousVerse()
		{
			if (VerseIterationHelper.TryGetPreviousVerse(StartRef, out var v))
				return v;

			throw new Exception("If a range starts before another range, that other range cannot" +
				" start at the very first verse; therefore it should be possible to get the" +
				" previous verse");
		}

		private BCVRef GetNextVerse()
		{
			if (VerseIterationHelper.TryGetNextVerse(EndRef, out var v))
				return v;

			throw new Exception("If a range ends after another range, that other range cannot" +
				" end at the very last verse; therefore it should be possible to get the next" +
				" verse");
		}

		public IEnumerable<ScriptureRange> GetNewRangesToBreakByVerse(IReadOnlyList<ScriptureRange> existingRanges)
		{
			var returnedResult = false;

			for (var i = 0; i < existingRanges.Count; i++)
			{
				var existingRange = existingRanges[i];
				if (existingRange.Includes(this))
					yield break;

				if (Touches(existingRange) || Overlaps(existingRange))
				{
					if (!returnedResult && Start < existingRange.Start)
					{
						yield return new ScriptureRange(Start, existingRange.GetPreviousVerse());
						returnedResult = true;
					}

					if (End > existingRange.End)
					{
						var vStart = existingRange.GetNextVerse();

						if (i < existingRanges.Count - 1 && existingRanges[i + 1].Start < End)
						{
							var vEnd = existingRanges[i + 1].GetPreviousVerse();
							yield return new ScriptureRange(vStart, vEnd);
							returnedResult = true;
							if (existingRanges[i + 1].End >= End)
								yield break;
						}
						else
						{
							yield return new ScriptureRange(new BCVRef(vStart), End);
							yield break;
						}
					}

					if (End <= existingRange.End)
						yield break;
				}
			}

			if (!returnedResult)
				yield return this;
		}
	}
}
