using Windows.Media.Control;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var gsmtcsm = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
            var currentSession = gsmtcsm.GetCurrentSession();

            if (currentSession != null)
            {
                // Get media properties first
                var mediaProperties = await currentSession.TryGetMediaPropertiesAsync();
                var playbackInfo = currentSession.GetPlaybackInfo();

                // Display stylish header
                Console.WriteLine();
                Console.WriteLine("=================================================================");
                Console.WriteLine("                   >> NOW PLAYING - WinKlang <<                 ");
                Console.WriteLine("=================================================================");
                Console.WriteLine();

                // Media Information with better formatting
                Console.WriteLine(">> TRACK DETAILS:");
                Console.WriteLine($"   ? Title:     {mediaProperties.Title ?? "Unknown Track"}");
                Console.WriteLine($"   ? Artist:    {mediaProperties.Artist ?? "Unknown Artist"}");
                Console.WriteLine($"   ? Album:     {(string.IsNullOrEmpty(mediaProperties.AlbumTitle) ? "Single/Unknown" : mediaProperties.AlbumTitle)}");
                
                // Enhanced status with more details
                string statusIcon = playbackInfo.PlaybackStatus switch
                {
                    GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing => "[>]",
                    GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused => "[||]",
                    GlobalSystemMediaTransportControlsSessionPlaybackStatus.Stopped => "[#]",
                    _ => "[?]"
                };
                Console.WriteLine($"   {statusIcon} Status:    {playbackInfo.PlaybackStatus}");
                
                // Show playback controls available
                var controls = playbackInfo.Controls;
                var availableControls = new List<string>();
                if (controls.IsPauseEnabled) availableControls.Add("Pause");
                if (controls.IsPlayEnabled) availableControls.Add("Play");
                if (controls.IsNextEnabled) availableControls.Add("Next");
                if (controls.IsPreviousEnabled) availableControls.Add("Previous");
                
                Console.WriteLine($"   ? Controls:  {string.Join(", ", availableControls)}");
                Console.WriteLine();
                Console.WriteLine("=================================================================");

                // Display Timeline Information (moved to Timeline.cs)
                await Timeline.DisplayTimelineInfoAsync(currentSession);

                // Display Artwork Information (moved to Artwork.cs)
                await Artwork.DisplayArtworkInfoAsync(currentSession);

                // System Information
                Console.WriteLine(">> SYSTEM INFO:");
                Console.WriteLine($"   @ Platform:    Windows Media Session Manager");
                Console.WriteLine($"   @ API:         .NET 9.0 Windows Runtime");
                Console.WriteLine($"   @ Build:       Single-file (~11MB optimized)");
                Console.WriteLine();
                Console.WriteLine("=================================================================");

                // Enhanced Footer
                Console.WriteLine($"* Captured:  {DateTime.Now:dddd, MMMM dd, yyyy 'at' HH:mm:ss}");
                Console.WriteLine($"* WinKlang:   v0.0.1-beta | Made with <3 for music lovers");
                Console.WriteLine("=================================================================");
            }
            else
            {
                Console.WriteLine("No media is currently playing.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}