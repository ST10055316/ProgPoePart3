using System;
using System.IO;
using System.Media;
using System.Windows.Media; // For Brushes

namespace CyberSecurityPoeWPF // IMPORTANT: Ensure this matches your project's root namespace
{
    internal class AudioPlayer
    {
        private readonly string _baseDirectory;
        private readonly Action<string, Brush> _logMessageAction; // Delegate to log messages to UI

        /// <summary>
        /// Initializes a new instance of the AudioPlayer class.
        /// </summary>
        /// <param name="logMessageAction">An action to call for logging messages to the UI (e.g., AppendChatbotMessage from MainWindow).</param>
        public AudioPlayer(Action<string, Brush> logMessageAction)
        {
            _logMessageAction = logMessageAction;
            // Get the base directory where the executable is running.
            // This is typically the 'bin/Debug/netX.0/' folder of your project.
            _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// Plays the greeting audio file asynchronously.
        /// It constructs the file path relative to the application's base directory.
        /// Reports any file not found or playback errors to the UI via the provided logMessageAction delegate.
        /// </summary>
        public void PlayGreeting()
        {
            // Construct the full path to the WAV file.
            // The 'Audio images' folder should be placed directly inside your project's root directory,
            // and its 'Copy to Output Directory' property in Visual Studio should be set to 'Copy if newer' or 'Copy always'.
            string filePath = Path.Combine(_baseDirectory, "Audio", "greetings.wav");

            // Log the path to the UI for debugging purposes
            _logMessageAction($"[AudioPlayer trying to load from]: {filePath}", Brushes.Gray);

            try
            {
                if (File.Exists(filePath))
                {
                    _logMessageAction($"[Audio file found]: {filePath}", Brushes.Green);
                    // Use SoundPlayer for WAV files. Play() for asynchronous playback,
                    // which prevents freezing the UI thread.
                    SoundPlayer player = new SoundPlayer(filePath);
                    player.Play(); // Asynchronous playback
                }
                else
                {
                    // Log a clear error message to the UI if the audio file is not found.
                    _logMessageAction($"[Audio greeting file NOT FOUND at]: {filePath}", Brushes.Red);
                }
            }
            catch (Exception ex)
            {
                // Catch any other exceptions that might occur during audio playback
                _logMessageAction($"[An error occurred while playing audio from {filePath}]: {ex.Message}", Brushes.Red);
            }
        }
    }
}
