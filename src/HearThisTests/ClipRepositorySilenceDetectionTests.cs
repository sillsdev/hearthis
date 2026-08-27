using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using HearThis.Publishing;
using L10NSharp;
using NUnit.Framework;
using SIL.IO;
using SIL.Media;
using SIL.Progress;
using static SIL.IO.FileLocationUtilities;

namespace HearThisTests
{
	/// <summary>
	/// Tests for ClipRepository.GetDurationOfLeadingSilence and GetDurationOfTrailingSilence.
	/// These require FFmpeg from DistFiles.
	/// </summary>
	[TestFixture]
	public class ClipRepositorySilenceDetectionTests
	{
		private const double kToleranceInSeconds = 0.1;

		/// <summary>
		/// Disposable temp folder whose path is guaranteed to contain spaces.
		/// </summary>
		private sealed class TempFolderWithSpaces : IDisposable
		{
			public string Path { get; }

			public TempFolderWithSpaces()
			{
				Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
					"HearThis silence tests " + Guid.NewGuid());
				Directory.CreateDirectory(Path);
			}

			public void Dispose()
			{
				try { Directory.Delete(Path, true); }
				catch (IOException) { }
			}
		}

		[SetUp]
		public void SetUp()
		{
			LocalizationManager.StrictInitializationMode = false;
			// Normally set by the PublishingMethodBase constructor, which is not involved
			// when calling the silence-detection methods directly.
			FFmpegRunner.FFmpegLocation = GetFileDistributedWithApplication("FFmpeg", "ffmpeg.exe");
		}

		[Test]
		public void GetDurationOfLeadingSilence_ClipStartsWithSilence_ReturnsDurationOfThatSilence()
		{
			using (var wavFile = TempFile.WithExtension(".wav"))
			{
				WriteWavFile(wavFile.Path, (1.0, true), (1.0, false));

				var duration = ClipRepository.GetDurationOfLeadingSilence(wavFile.Path,
					new StringBuilderProgress());

				Assert.That(duration, Is.EqualTo(1.0).Within(kToleranceInSeconds));
			}
		}

		[Test]
		public void GetDurationOfLeadingSilence_SourcePathContainsSpaces_ReturnsDurationOfLeadingSilence()
		{
			using (var folder = new TempFolderWithSpaces())
			{
				var wavPath = Path.Combine(folder.Path, "clip one.wav");
				WriteWavFile(wavPath, (1.0, true), (1.0, false));

				var duration = ClipRepository.GetDurationOfLeadingSilence(wavPath,
					new StringBuilderProgress());

				Assert.That(duration, Is.EqualTo(1.0).Within(kToleranceInSeconds));
			}
		}

		[Test]
		public void GetDurationOfLeadingSilence_FFmpegLocationContainsSpaces_ReturnsDurationOfLeadingSilence()
		{
			// HearThis could be installed at a nonstandard location whose path contains spaces.
			var origFFmpegLocation = FFmpegRunner.FFmpegLocation;
			using (var folder = new TempFolderWithSpaces())
			using (var wavFile = TempFile.WithExtension(".wav"))
			{
				var ffmpegCopy = Path.Combine(folder.Path, "ffmpeg.exe");
				File.Copy(origFFmpegLocation, ffmpegCopy);
				WriteWavFile(wavFile.Path, (1.0, true), (1.0, false));
				try
				{
					FFmpegRunner.FFmpegLocation = ffmpegCopy;

					var duration = ClipRepository.GetDurationOfLeadingSilence(wavFile.Path,
						new StringBuilderProgress());

					Assert.That(duration, Is.EqualTo(1.0).Within(kToleranceInSeconds));
				}
				finally
				{
					FFmpegRunner.FFmpegLocation = origFFmpegLocation;
				}
			}
		}

		[Test]
		public void GetDurationOfLeadingSilence_ClipStartsWithSoundButHasMidClipPause_ReturnsZero()
		{
			using (var wavFile = TempFile.WithExtension(".wav"))
			{
				WriteWavFile(wavFile.Path, (0.75, false), (0.5, true), (0.75, false));

				var duration = ClipRepository.GetDurationOfLeadingSilence(wavFile.Path,
					new StringBuilderProgress());

				Assert.That(duration, Is.EqualTo(0),
					"A pause in the middle of a clip that starts with sound must not be " +
					"reported as leading silence");
			}
		}

		[Test]
		public void GetDurationOfTrailingSilence_SourcePathContainsSpaces_ReturnsDurationOfTrailingSilence()
		{
			using (var folder = new TempFolderWithSpaces())
			{
				var wavPath = Path.Combine(folder.Path, "clip two.wav");
				WriteWavFile(wavPath, (1.0, false), (1.0, true));

				var duration = ClipRepository.GetDurationOfTrailingSilence(wavPath,
					new StringBuilderProgress());

				Assert.That(duration, Is.EqualTo(1.0).Within(kToleranceInSeconds));
				Assert.That(wavPath, Does.Exist, "Source clip must survive the measurement");
			}
		}

		[Test]
		public void ParseDurationOfLeadingSilence_SilenceStartsAtZero_ReturnsReportedDuration()
		{
			var output = "[silencedetect @ 000001c8] silence_start: 0\r\n" +
				"[silencedetect @ 000001c8] silence_end: 1.00002 | silence_duration: 1.00002\r\n";

			Assert.That(ClipRepository.ParseDurationOfLeadingSilence(output),
				Is.EqualTo(1.00002).Within(0.000001));
		}

		[Test]
		public void ParseDurationOfLeadingSilence_FirstSilenceStartsMidClip_ReturnsZero()
		{
			var output = "[silencedetect @ 000001c8] silence_start: 0.75\r\n" +
				"[silencedetect @ 000001c8] silence_end: 1.25 | silence_duration: 0.5\r\n";

			Assert.That(ClipRepository.ParseDurationOfLeadingSilence(output), Is.EqualTo(0));
		}

		[Test]
		public void ParseDurationOfLeadingSilence_NoSilenceReported_ReturnsZero()
		{
			var output = "size=N/A time=00:00:02.00 bitrate=N/A speed= 214x\r\n";

			Assert.That(ClipRepository.ParseDurationOfLeadingSilence(output), Is.EqualTo(0));
		}

		[Test]
		public void ParseDurationOfLeadingSilence_CommaDecimalSeparatorCulture_ParsesFFmpegOutputAsInvariant()
		{
			var output = "[silencedetect @ 000001c8] silence_start: 0\r\n" +
				"[silencedetect @ 000001c8] silence_end: 1.00002 | silence_duration: 1.00002\r\n";

			var origCulture = Thread.CurrentThread.CurrentCulture;
			try
			{
				Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");

				Assert.That(ClipRepository.ParseDurationOfLeadingSilence(output),
					Is.EqualTo(1.00002).Within(0.000001));
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = origCulture;
			}
		}

		/// <summary>
		/// Writes a 16-bit mono PCM WAV file consisting of the requested segments of pure
		/// silence (digital zero) or a 440 Hz tone at about -6 dBFS (well above the -35 dB
		/// threshold used by the silence detection).
		/// </summary>
		internal static void WriteWavFile(string path, params (double Seconds, bool Silent)[] segments)
		{
			const int sampleRate = 44100;
			var samples = new List<short>();
			foreach (var segment in segments)
			{
				var count = (int)(segment.Seconds * sampleRate);
				for (var i = 0; i < count; i++)
				{
					samples.Add(segment.Silent ? (short)0
						: (short)(short.MaxValue * 0.5 * Math.Sin(2 * Math.PI * 440 * i / sampleRate)));
				}
			}

			using (var writer = new BinaryWriter(File.Create(path)))
			{
				var dataSize = samples.Count * sizeof(short);
				writer.Write(Encoding.ASCII.GetBytes("RIFF"));
				writer.Write(36 + dataSize);
				writer.Write(Encoding.ASCII.GetBytes("WAVE"));
				writer.Write(Encoding.ASCII.GetBytes("fmt "));
				writer.Write(16); // fmt chunk size
				writer.Write((short)1); // PCM
				writer.Write((short)1); // mono
				writer.Write(sampleRate);
				writer.Write(sampleRate * sizeof(short)); // byte rate
				writer.Write((short)sizeof(short)); // block align
				writer.Write((short)16); // bits per sample
				writer.Write(Encoding.ASCII.GetBytes("data"));
				writer.Write(dataSize);
				foreach (var sample in samples)
					writer.Write(sample);
			}
		}
	}
}
