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
                // Check playback status first
                var playbackInfo = currentSession.GetPlaybackInfo();
                Console.WriteLine($"Initial Playback Status: {playbackInfo.PlaybackStatus}");

                // Wait longer for timeline to update
                Console.WriteLine("Waiting for timeline to update...");
                await Task.Delay(1000);

                var timelineInfo = Timeline.getTimeline(currentSession);

                // You can now use the timeline object properties
                Console.WriteLine($"Returned object - Current: {timelineInfo.CurrentTime}, Progress: {timelineInfo.ProgressPercent:F1}%");
                Console.WriteLine($"Playback Status: {currentSession.GetPlaybackInfo().PlaybackStatus}");
                var artworkFileName = await Artwork.SaveArtworkAsync(currentSession);
                Console.WriteLine($"Artwork saved as: {artworkFileName}");

                var artworkBuffer = await Artwork.GetArtworkBufferAsync(currentSession);
                //see whole buffer  as string for testing
                Console.WriteLine($"Artwork buffer length: {artworkBuffer.Length} bytes");
                // Console.WriteLine($"Artwork buffer: {BitConverter.ToString(artworkBuffer)}");
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