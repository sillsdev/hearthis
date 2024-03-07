using Concentus.Enums;
using Concentus.Oggfile;
using Concentus.Structs;
using System.IO;
using L10NSharp;
using SIL.Progress;
using System;

namespace HearThis.Publishing
{
	public class OpusEncoder : IAudioEncoder
	{
		public void Encode(string sourcePath, string destPathWithoutExtension, IProgress progress)
		{
			progress.WriteMessage("   " + LocalizationManager.GetString("OpusEncoder.Progress", "Converting to OGG Opus format", "Appears in progress indicator"));

			using (var inputFileStream = new FileStream(sourcePath, FileMode.Open))
			using (var opusStream = new FileStream($"{destPathWithoutExtension}.opus", FileMode.Create))
			{
				OpusEncoder encoder = OpusEncoder.Create(48000, 1, OpusApplication.OPUS_APPLICATION_AUDIO);
				encoder.Bitrate = 64000;

				byte[] inputBuffer = new byte[4096]; // Adjust buffer size as needed

				while (inputFileStream.Read(inputBuffer, 0, inputBuffer.Length) > 0)
				{
					byte[] encoded = new byte[opusStream.Length];
					int bytesEncoded = encoder.Encode(inputBuffer, 0, inputBuffer.Length, encoded, 0, encoded.Length);
					opusStream.Write(encoded, 0, bytesEncoded);
				}
			}
		}
		

		public string FormatName => "opus";

		public int Bitrate { get; private set; }
	}
}
