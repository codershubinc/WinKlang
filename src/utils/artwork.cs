using Windows.Media.Control;

public class Artwork
{
    public static async Task<string> SaveArtworkAsync(GlobalSystemMediaTransportControlsSession currentSession)
    {
        var mediaProperties = await currentSession.TryGetMediaPropertiesAsync();
        var generateFileName = $"{mediaProperties.Artist} - {mediaProperties.Title}.jpg";
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitizedFileName = string.Join("_", generateFileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
        
        if (mediaProperties.Thumbnail != null)
        {
            var thumbnailStreamRef = mediaProperties.Thumbnail;
            using var thumbnailStream = await thumbnailStreamRef.OpenReadAsync();
            using var fileStream = new FileStream(sanitizedFileName, FileMode.Create);
            await thumbnailStream.AsStreamForRead().CopyToAsync(fileStream);
            return sanitizedFileName;
        }
        return "noArtwork";
    }

    public static async Task<byte[]> GetArtworkBufferAsync(GlobalSystemMediaTransportControlsSession currentSession)
    {
        var mediaProperties = await currentSession.TryGetMediaPropertiesAsync();
        if (mediaProperties.Thumbnail != null)
        {
            var thumbnailStreamRef = mediaProperties.Thumbnail;
            using var thumbnailStream = await thumbnailStreamRef.OpenReadAsync();
            using var memoryStream = new MemoryStream();
            await thumbnailStream.AsStreamForRead().CopyToAsync(memoryStream);
            byte[] artworkBuffer = memoryStream.ToArray();
            return artworkBuffer;
        }
        return Array.Empty<byte>();
    }

    public static async Task DisplayArtworkInfoAsync(GlobalSystemMediaTransportControlsSession currentSession)
    {
        Console.WriteLine(">> ALBUM ARTWORK:");
        
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
