using L10NSharp;
using SIL.CommandLineProcessing;
using SIL.IO;
using SIL.Progress;

namespace HearThis.Publishing
{
    /// <summary>
    /// This class is used to encode audio files to the Opus format.
    /// </summary>
    public class OpusEncoder : IAudioEncoder
    {
        public void Encode(string sourcePath, string destPathWithoutExtension, IProgress progress)
        {
            // Display a progress message
            progress.WriteMessage("   " + LocalizationManager.GetString("OpusEncoder.Progress", "Converting to opus format", "Appears in progress indicator"));

            // Construct the arguments for opusenc
            string args = $"--bitrate 64 \"{sourcePath}\" \"{destPathWithoutExtension}.opus\"";

            // Get the path to the opusenc executable
            string exePath = "path/to/opusenc"; // Replace "path/to/opusenc" with the actual path to opusenc

            // Write the command and arguments to the progress
            progress.WriteVerbose(exePath + " " + args);

            // Run opusenc with the specified arguments
            var result = CommandLineRunner.Run(exePath, args, "", 60 * 10, progress);

            // Check for errors
            if (result.StandardError.Contains("FAIL"))
                progress.WriteError(result.StandardError);
        }

        public string FormatName => "opus";
    }
}
