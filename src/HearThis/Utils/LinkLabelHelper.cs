// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International. All Rights Reserved.
// <copyright from='2015' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace HearThis
{
	/// <summary>
	/// This class makes it possible for a link label to be localized without having to use
	/// separate strings/parameters to determine which portion of the string should be the link.
	/// For a simple link label with a single link, just enclose the portion of the text that is
	/// to be the link in square brackets in Designer. (This will serve to illustrate for the
	/// localizers how to format the localized string; a localization comment should also be added
	/// to explain the intent of the bracketed text.) In the class for the form containing
	/// the link label, add a handler for LocalizeItemDlg<XLiffDocument>.StringsLocalized. In the
	/// handler, call <see cref="SetLinkRegions"/> for any link label that uses this approach.
	/// In the form's constructor, in addition to hooking up the handler to the event, call the
	/// handler method explicitly after the call to InitializeComponent. This will hook up the link
	/// to the bracketed text and remove the brackets from the text, whether set explicitly in
	/// the Designer-generated code or by the L10nSharpExtender.
	/// </summary>
	public static class LinkLabelHelper
	{
		private class OrigLinkLabelLinkCollection
		{
			private readonly WeakReference<LinkLabel> _linkLabel;
			public LinkLabel.Link[] Links { get; }
			
			public OrigLinkLabelLinkCollection(LinkLabel linkLabel)
			{
				_linkLabel = new WeakReference<LinkLabel>(linkLabel);
				Links = new LinkLabel.Link[linkLabel.Links.Count];
				for (var index = 0; index < linkLabel.Links.Count; index++)
					Links[index] = linkLabel.Links[index];
			}

			public bool IsFor(LinkLabel linkLabel) =>
				_linkLabel.TryGetTarget(out var target) && target == linkLabel;
		}

		private static readonly List<OrigLinkLabelLinkCollection> _knownLinkLabelLinks =
			new List<OrigLinkLabelLinkCollection>();

		/// <summary>
		/// Sets the link label area to the text enclosed in square brackets and removes the brackets.
		/// If there is more than one existing link area, hook them up in order.
		/// ENHANCE: If there is ever a need for this, we could add the ability to number them so
		/// the localized order would not have to match the original order.
		/// </summary>
		/// <param name="linkLabel">The link label</param>
		public static void SetLinkRegions(this LinkLabel linkLabel)
		{
			var links = _knownLinkLabelLinks.SingleOrDefault(lll => lll.IsFor(linkLabel))?.Links;
			if (links == null)
			{
				var newOne = new OrigLinkLabelLinkCollection(linkLabel);
				links = newOne.Links;
				_knownLinkLabelLinks.Add(newOne);

				linkLabel.Disposed += (s, e) =>
				{
					for (int o = 0; o < _knownLinkLabelLinks.Count; o++)
					{
						if (_knownLinkLabelLinks[o].IsFor((LinkLabel)s))
						{
							_knownLinkLabelLinks.RemoveAt(o);
							break;
						}
					}
				};
			}

			var text = linkLabel.Text;
			var regex = new Regex(@"\[(?<linktext>[^\]]+)\]", RegexOptions.Compiled);
			var iMatchStart = 0;
			var sb = new StringBuilder();
			for (var i = 0; i < links.Length; i++)
			{
				var match = regex.Match(text, iMatchStart);
				if (match.Success)
				{
					if (match.Index > 0)
						sb.Append(text.Substring(iMatchStart, match.Index - iMatchStart));
					var linkText = match.Groups["linktext"].Value;
					sb.Append(linkText);
					var linkStart = match.Index - i * 2;
					// LinkLabel won't allow overlapping link areas, so we need to ensure that any
					// other links beyond the current one are far enough out so as not to cause
					// this one to get truncated.
					for (var j = links.Length - 1; j > i; j--)
					{
						var minStartPos = linkStart + linkText.Length + (j - i);
						if (links[j].Start < minStartPos)
						{
							if (minStartPos + links[j].Length > text.Length)
								links[j].Length = 1;
							links[j].Start = minStartPos;
						}
					}
					links[i].Start = match.Index - i * 2;
					links[i].Length = linkText.Length;
					iMatchStart = match.Index + match.Length;
				}
				else
				{
					sb.Append(text.Substring(iMatchStart));
					if (i > 0)
					{
						do
						{
							linkLabel.Links.RemoveAt(i);
						} while (linkLabel.Links.Count > i);
					}
					else
					{
						links[i].Start = iMatchStart;
						links[i].Length = text.Length - iMatchStart;
					}

					linkLabel.Text = sb.ToString();
					return;
				}
			}

			if (iMatchStart > 0)
			{
				sb.Append(text.Substring(iMatchStart));
				linkLabel.Text = sb.ToString();
				for (var l = linkLabel.Links.Count; l < links.Length; l++)
					linkLabel.Links.Add(links[l]);
			}
		}
	}
}
