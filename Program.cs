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
                // --- THIS IS THE FIX ---
                // Wait a quarter of a second to allow Windows to update the timeline info.
                await Task.Delay(250);

                // Now, get the timeline information
                var timelineProperties = currentSession.GetTimelineProperties();
                
                var mediaProperties = await currentSession.TryGetMediaPropertiesAsync();

                Console.WriteLine($"Title:  {mediaProperties.Title}");
                Console.WriteLine($"Artist: {mediaProperties.Artist}");
                Console.WriteLine($"Album:  {mediaProperties.AlbumTitle}");
                
                string currentTime = timelineProperties.Position.ToString(@"mm\:ss");
                string totalTime = timelineProperties.EndTime.ToString(@"mm\:ss");
                Console.WriteLine($"Time:   {currentTime} / {totalTime}");

                if (mediaProperties.Thumbnail != null)
                {
                    var thumbnailStreamRef = mediaProperties.Thumbnail;
                    using (var thumbnailStream = await thumbnailStreamRef.OpenReadAsync())
                    {
                        using var fileStream = new FileStream("artwork.jpg", FileMode.Create);
                        await thumbnailStream.AsStreamForRead().CopyToAsync(fileStream);
                    }
                    Console.WriteLine("Artwork saved successfully to artwork.jpg");
                }
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