using System;
using System.IO;
using L10NSharp;
using SIL.Progress;
using Vosk;

namespace HearThis.Publishing
{
	public class SRTEncoder : IAudioEncoder
	{
		public void Encode(string sourcePath, string destPathWithoutExtension, IProgress progress)
		{
			progress.WriteMessage("   " + LocalizationManager.GetString("SRTEncoder.Progress", "Converting to SRT format", "Appears in progress indicator"));

			try
			{
				using (var model = new Model("model"))
				{
					using (var recognizer = new VoskRecognizer(model, 16000.0f))
					{
						using (var audioStream = new FileStream(sourcePath, FileMode.Open))
						{
							using (var srtWriter = new StreamWriter(destPathWithoutExtension + ".srt"))
							{
								long startTime = 0;
								long endTime = 0;
								int index = 1;

								var buffer = new byte[4096];
								int bytesRead;
								while ((bytesRead = audioStream.Read(buffer, 0, buffer.Length)) > 0)
								{
									float[] fbuffer = new float[bytesRead / 2];
									Buffer.BlockCopy(buffer, 0, fbuffer, 0, bytesRead);

									if (recognizer.AcceptWaveform(fbuffer, fbuffer.Length))
									{
										var result = recognizer.Result();
										string currentSubtitle = string.Empty;
										foreach (var word in result)
										{
											currentSubtitle += word;
											if (currentSubtitle.Contains(" ")) // Check if the word contains a space character
											{
												if (!string.IsNullOrWhiteSpace(currentSubtitle))
												{
													srtWriter.WriteLine(index++);
													srtWriter.WriteLine($"{FormatTime(startTime)} --> {FormatTime(endTime)}");
													srtWriter.WriteLine(currentSubtitle);
													srtWriter.WriteLine();
													currentSubtitle = string.Empty;
												}
											}
										}
									}
									endTime += (long)((double)bytesRead / 16000.0 * 1000);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				progress.WriteError($"Error: {ex.Message}");
			}
		}

		private string FormatTime(long milliseconds)
		{
			var ts = TimeSpan.FromMilliseconds(milliseconds);
			return $"{ts.Hours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2},{ts.Milliseconds:D3}";
		}

		public string FormatName => "srt";
	}
}
