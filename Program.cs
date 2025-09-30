using Windows.Media.Control;

public class MusicMetadata
{
    public string Title { get; set; } = "";
    public string Artist { get; set; } = "";
    public string Album { get; set; } = "";
    public string Status { get; set; } = "";
    public string CurrentTime { get; set; } = "";
    public string Duration { get; set; } = "";
    public double ProgressPercent { get; set; } = 0;
    public string ArtworkUri { get; set; } = "";
    public long Timestamp { get; set; } = 0;
    public string CapturedAt { get; set; } = "";
}

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var gsmtcsm = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
            var currentSession = gsmtcsm.GetCurrentSession();

            // Handle help or no arguments
            if (args.Length > 0 && (args[0].ToLower() == "--help" || args[0].ToLower() == "-h"))
            {
                Controls.DisplayUsage();
                return;
            }

            if (currentSession != null)
            {
                // Handle different command line arguments
                if (args.Length > 0)
                {
                    switch (args[0].ToLower())
                    {
                        case "--json":
                            await OutputJsonAsync(currentSession);
                            break;
                        case "--play-pause":
                            await Controls.PlayPauseAsync(currentSession);
                            break;
                        case "--next":
                            await Controls.NextTrackAsync(currentSession);
                            break;
                        case "--prev":
                        case "--previous":
                            await Controls.PreviousTrackAsync(currentSession);
                            break;
                        default:
                            Console.WriteLine($"Unknown argument: {args[0]}");
                            Console.WriteLine("Use --help for usage information");
                            break;
                    }
                }
                else
                {
                    // Default: show formatted output
                    await OutputFormattedAsync(currentSession);
                }
            }
            else
            {
                if (args.Length > 0 && args[0].ToLower() == "--json")
                {
                    var emptyMetadata = new MusicMetadata
                    {
                        Title = "No media playing",
                        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                        CapturedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    OutputJsonData(emptyMetadata);
                }
                else
                {
                    Console.WriteLine("No media is currently playing.");
                }
            }
        }
        catch (Exception ex)
        {
            if (args.Length > 0 && args[0].ToLower() == "--json")
            {
                var errorMetadata = new MusicMetadata
                {
                    Title = $"Error: {ex.Message}",
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    CapturedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                OutputJsonData(errorMetadata);
            }
            else
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }

    private static async Task OutputJsonAsync(GlobalSystemMediaTransportControlsSession currentSession)
    {
        // Get media properties
        var mediaProperties = await currentSession.TryGetMediaPropertiesAsync();
        var playbackInfo = currentSession.GetPlaybackInfo();

        // Get timeline info using modular Timeline class
        var timelineInfo = Timeline.getTimeline(currentSession);

        // Get artwork using modular Artwork class
        var artworkFileName = await Artwork.SaveArtworkAsync(currentSession);

        var metadata = new MusicMetadata
        {
            Title = mediaProperties.Title ?? "Unknown Track",
            Artist = mediaProperties.Artist ?? "Unknown Artist",
            Album = string.IsNullOrEmpty(mediaProperties.AlbumTitle) ? "Unknown Album" : mediaProperties.AlbumTitle,
            Status = playbackInfo.PlaybackStatus.ToString(),
            CurrentTime = timelineInfo.CurrentTime,
            Duration = timelineInfo.TotalTime,
            ProgressPercent = timelineInfo.ProgressPercent,
            ArtworkUri = artworkFileName == "noArtwork" ? "" : artworkFileName,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            CapturedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        OutputJsonData(metadata);
    }

    private static async Task OutputFormattedAsync(GlobalSystemMediaTransportControlsSession currentSession)
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

        // Display Timeline Information (from Timeline.cs)
        await Timeline.DisplayTimelineInfoAsync(currentSession);

        // Display Artwork Information (from Artwork.cs)
        await Artwork.DisplayArtworkInfoAsync(currentSession);

        // System Information
        Console.WriteLine(">> SYSTEM INFO:");
        Console.WriteLine($"   @ Platform:    Windows Media Session Manager");
        Console.WriteLine($"   @ API:         .NET 9.0 Windows Runtime");
        Console.WriteLine($"   @ Build:       Single-file (~17MB self-contained)");
        Console.WriteLine();
        Console.WriteLine("=================================================================");

        // Enhanced Footer
        Console.WriteLine($"* Captured:  {DateTime.Now:dddd, MMMM dd, yyyy 'at' HH:mm:ss}");
        Console.WriteLine($"* WinKlang:   v0.0.4-controls | Modular with media controls");
        Console.WriteLine("=================================================================");
    }

    private static void OutputJsonData(MusicMetadata metadata)
    {
        // Simple JSON output without JsonSerializer to avoid trimming issues
        Console.WriteLine("{");
        Console.WriteLine($"  \"title\": \"{EscapeJsonString(metadata.Title)}\",");
        Console.WriteLine($"  \"artist\": \"{EscapeJsonString(metadata.Artist)}\",");
        Console.WriteLine($"  \"album\": \"{EscapeJsonString(metadata.Album)}\",");
        Console.WriteLine($"  \"status\": \"{EscapeJsonString(metadata.Status)}\",");
        Console.WriteLine($"  \"currentTime\": \"{EscapeJsonString(metadata.CurrentTime)}\",");
        Console.WriteLine($"  \"duration\": \"{EscapeJsonString(metadata.Duration)}\",");
        Console.WriteLine($"  \"progressPercent\": {metadata.ProgressPercent:F1},");
        Console.WriteLine($"  \"artworkUri\": \"{EscapeJsonString(metadata.ArtworkUri)}\",");
        Console.WriteLine($"  \"timestamp\": {metadata.Timestamp},");
        Console.WriteLine($"  \"capturedAt\": \"{EscapeJsonString(metadata.CapturedAt)}\"");
        Console.WriteLine("}");
    }

    private static string EscapeJsonString(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "";

        return input.Replace("\\", "\\\\")
                   .Replace("\"", "\\\"")
                   .Replace("\n", "\\n")
                   .Replace("\r", "\\r")
                   .Replace("\t", "\\t");
    }
}