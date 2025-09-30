using Windows.Media.Control;

public class Controls
{
    public static async Task<bool> PlayPauseAsync(GlobalSystemMediaTransportControlsSession currentSession)
    {
        try
        {
            var playbackInfo = currentSession.GetPlaybackInfo();
            
            if (playbackInfo.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing)
            {
                if (playbackInfo.Controls.IsPauseEnabled)
                {
                    await currentSession.TryPauseAsync();
                    Console.WriteLine(">> Media paused successfully");
                    return true;
                }
                else
                {
                    Console.WriteLine(">> Error: Pause control not available");
                    return false;
                }
            }
            else
            {
                if (playbackInfo.Controls.IsPlayEnabled)
                {
                    await currentSession.TryPlayAsync();
                    Console.WriteLine(">> Media resumed successfully");
                    return true;
                }
                else
                {
                    Console.WriteLine(">> Error: Play control not available");
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($">> Error controlling playback: {ex.Message}");
            return false;
        }
    }

    public static async Task<bool> NextTrackAsync(GlobalSystemMediaTransportControlsSession currentSession)
    {
        try
        {
            var playbackInfo = currentSession.GetPlaybackInfo();
            
            if (playbackInfo.Controls.IsNextEnabled)
            {
                await currentSession.TrySkipNextAsync();
                Console.WriteLine(">> Skipped to next track successfully");
                return true;
            }
            else
            {
                Console.WriteLine(">> Error: Next track control not available");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($">> Error skipping to next track: {ex.Message}");
            return false;
        }
    }

    public static async Task<bool> PreviousTrackAsync(GlobalSystemMediaTransportControlsSession currentSession)
    {
        try
        {
            var playbackInfo = currentSession.GetPlaybackInfo();
            
            if (playbackInfo.Controls.IsPreviousEnabled)
            {
                await currentSession.TrySkipPreviousAsync();
                Console.WriteLine(">> Skipped to previous track successfully");
                return true;
            }
            else
            {
                Console.WriteLine(">> Error: Previous track control not available");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($">> Error skipping to previous track: {ex.Message}");
            return false;
        }
    }

    public static void DisplayUsage()
    {
        Console.WriteLine("WinKlang - Windows Media Session Manager Tool");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  WinKlang.exe                Display current media information");
        Console.WriteLine("  WinKlang.exe --json         Display current media information in JSON format");
        Console.WriteLine("  WinKlang.exe --play-pause   Toggle play/pause");
        Console.WriteLine("  WinKlang.exe --next          Skip to next track");
        Console.WriteLine("  WinKlang.exe --prev          Skip to previous track");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  WinKlang.exe --json | jq .title    # Get just the title using jq");
        Console.WriteLine("  WinKlang.exe --play-pause          # Pause if playing, play if paused");
        Console.WriteLine();
    }
}
