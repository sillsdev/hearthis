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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using L10NSharp;
using SIL.IO;
using SIL.Media;
using static System.String;

namespace HearThis
{
	public static class Utils
	{
		public static string CreateDirectory(params string[] pathParts)
		{
			return Directory.CreateDirectory(Path.Combine(pathParts)).FullName;
		}

		public static ISimpleAudioSession GetPlayer(Form parent, string path)
		{
			while (true)
			{
				try
				{
					return AudioFactory.CreateAudioSession(path);
				}
				catch (Exception e)
				{
					string msg = Format(LocalizationManager.GetString("Program.FailedToCreateAudioSession",
						"The following error occurred while preparing an audio session to be able to play back recordings:\r\n{0}\r\n" +
						"{1} will not work correctly without speakers. Ensure that your speakers are enabled and functioning properly.\r\n" +
						"Would you like {1} to try again?"), e.Message, Program.kProduct);
					if (DialogResult.No == MessageBox.Show(parent, msg, Program.kProduct, MessageBoxButtons.YesNo))
						return null;
				}
			}
		}
	}

	// ENHANCE: Modify the Move method in Libpalaso's RobustFile class to take this optional third parameter
	// and then look at the myriad places where it's used that could be simplified by removing the code to
	// check for and delete the destination file.
	public static class RobustFileAddOn
	{
		public static void Move(string sourceFileName, string destFileName, bool overWrite = false)
		{
			if (overWrite && RobustFile.Exists(destFileName))
				RobustFile.Delete(destFileName);
			RobustFile.Move(sourceFileName, destFileName);
		}
	}

	public static class LinkLabelHelper
	{
		private class OrigLinkLabelLinkCollection
		{
			private WeakReference<LinkLabel> _linkLabel;
			public LinkLabel.Link[] Links { get; private set; }
			
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

		static List<OrigLinkLabelLinkCollection> _knownLinkLabelLinks =
			new List<OrigLinkLabelLinkCollection>();

		/// <summary>
		/// Sets the link label area to the text enclosed in curly braces and removes the braces.
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
			var regex = new Regex(@"\{(?<linktext>[^\}]*)\}", RegexOptions.Compiled);
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
