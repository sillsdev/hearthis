using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HearThis.Publishing;
using NUnit.Framework;
using Palaso.IO;

namespace HearThisTests
{
	[TestFixture]
	public class LineRecordingRepositoryTests
	{
		/// <summary>
		/// regression
		/// </summary>
		[Test, Ignore("known to fail")]
		public void MergeAudioFiles_DifferentChannels_StillMerges()
		{
			using (var output = new TempFile())
			using (var mono = TempFile.FromResource(Resource1._1Channel, ".wav"))
			using (var stereo = TempFile.FromResource(Resource1._2Channel, ".wav"))
			{
				var filesToJoin = new List<string>();
				filesToJoin.Add(mono.Path);
				filesToJoin.Add(stereo.Path);
				var progress = new Palaso.Progress.StringBuilderProgress();

				LineRecordingRepository.MergeAudioFiles(filesToJoin, output.Path, progress);
				Assert.IsFalse(progress.ErrorEncountered);
				Assert.IsTrue(File.Exists(output.Path));
			}
		}
	}
}
