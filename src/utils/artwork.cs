using Windows.Media.Control;

public class Artwork
{
    // returns filename as string 
    public static async Task<string> SaveArtworkAsync(
        GlobalSystemMediaTransportControlsSession currentSession
        )
    {
        var mediaProperties = await currentSession.TryGetMediaPropertiesAsync();
        var generateFileName = $"{mediaProperties.Artist} - {mediaProperties.Title}.jpg";
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitizedFileName = string.Join("_", generateFileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
        if (mediaProperties.Thumbnail != null)
        {
            // Get the thumbnail stream
            var thumbnailStreamRef = mediaProperties.Thumbnail;
            // Open the thumbnail stream for reading
            using var thumbnailStream = await thumbnailStreamRef.OpenReadAsync();
            // Save the thumbnail to the specified file path
            using var fileStream = new FileStream(sanitizedFileName, FileMode.Create);
            // Copy the thumbnail stream to the file stream
            await thumbnailStream.AsStreamForRead().CopyToAsync(fileStream);
            // Return the file path
            return sanitizedFileName;
        }
        return "noArtwork";
    }

    public static async Task<byte[]> GetArtworkBufferAsync(
        GlobalSystemMediaTransportControlsSession currentSession
        )
    {
        var mediaProperties = await currentSession.TryGetMediaPropertiesAsync();
        if (mediaProperties.Thumbnail != null)
        {
            // Get the thumbnail stream
            var thumbnailStreamRef = mediaProperties.Thumbnail;
            // Open the thumbnail stream for reading
            using var thumbnailStream = await thumbnailStreamRef.OpenReadAsync();
            // Convert the stream to a byte array
            using var memoryStream = new MemoryStream();
            await thumbnailStream.AsStreamForRead().CopyToAsync(memoryStream);
            byte[] artworkBuffer = memoryStream.ToArray();
            return artworkBuffer;
        }
        return Array.Empty<byte>();
    }

    public static async Task DisplayArtworkInfoAsync(
        GlobalSystemMediaTransportControlsSession currentSession
        )
    {
        // Display artwork section header
        Console.WriteLine(">> ALBUM ARTWORK:");
        
        // Get artwork information
        var artworkFileName = await SaveArtworkAsync(currentSession);
        
        if (artworkFileName != "noArtwork")
        {
            var artworkBuffer = await GetArtworkBufferAsync(currentSession);
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
    }
}