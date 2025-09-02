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

                // Wait for timeline to update
                Console.WriteLine(">> Synchronizing timeline data...");
                Console.WriteLine(">> If it gives wrong time data THEN its windows problem FK windows...");
                await Task.Delay(1000);

                // Enhanced Timeline Information
                var timelineInfo = Timeline.getTimeline(currentSession);
                Console.WriteLine(">> PLAYBACK TIMELINE:");
                Console.WriteLine($"   << Position:     {timelineInfo.CurrentTime}");
                Console.WriteLine($"   >> Duration:     {timelineInfo.TotalTime}");
                
                // Calculate remaining time
                var remainingTime = timelineInfo.Duration - timelineInfo.RawPosition;
                var remainingFormatted = remainingTime.ToString(@"mm\:ss");
                Console.WriteLine($"   ~ Remaining:    {remainingFormatted}");
                Console.WriteLine($"   % Progress:     {timelineInfo.ProgressPercent:F1}%");

                // Enhanced progress bar with time markers
                int progressBarWidth = 50;
                int filledWidth = (int)(timelineInfo.ProgressPercent / 100.0 * progressBarWidth);
                string progressBar = new string('=', filledWidth) + new string('-', progressBarWidth - filledWidth);
                Console.WriteLine($"   [{progressBar}] {timelineInfo.ProgressPercent:F1}%");
                
                // Time visualization
                Console.WriteLine($"   {timelineInfo.CurrentTime} ================================================ {timelineInfo.TotalTime}");
                Console.WriteLine();
                Console.WriteLine("=================================================================");

                // Enhanced Artwork Information
                Console.WriteLine(">> ALBUM ARTWORK:");
                var artworkFileName = await Artwork.SaveArtworkAsync(currentSession);
                if (artworkFileName != "noArtwork")
                {
                    var artworkBuffer = await Artwork.GetArtworkBufferAsync(currentSession);
                    var fileSizeKB = artworkBuffer.Length / 1024.0;
                    Console.WriteLine($"   [+] File:        {artworkFileName}");
                    Console.WriteLine($"   # Size:        {fileSizeKB:F1} KB ({artworkBuffer.Length:N0} bytes)");
                    Console.WriteLine($"   # Resolution:  ~150x150 px (Windows API limit)");
                    Console.WriteLine($"   ! Tip:         Use music service APIs for HD artwork");
                }
                else
                {
                    Console.WriteLine($"   [-] Status:      No artwork available");
                    Console.WriteLine($"   ! Tip:         Some players don't provide artwork via Windows API");
                }
                Console.WriteLine();
                Console.WriteLine("=================================================================");

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