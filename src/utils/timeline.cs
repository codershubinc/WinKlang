using Windows.Media.Control;

public class TimelineInfo
{
    public required string CurrentTime { get; set; }
    public required string TotalTime { get; set; }
    public TimeSpan RawPosition { get; set; }
    public double PositionMs { get; set; }
    public double ProgressPercent { get; set; }
    public TimeSpan Duration { get; set; }
}

class Timeline
{
    public static TimelineInfo getTimeline(
        GlobalSystemMediaTransportControlsSession currentSession
    )
    {
        // Get timeline properties
        var timelineProperties = currentSession.GetTimelineProperties();

        // Format the time properly
        string currentTime = timelineProperties.Position.ToString(@"mm\:ss");
        string totalTime = timelineProperties.EndTime.ToString(@"mm\:ss");

        // Calculate progress percentage
        double progressPercent = 0;
        if (timelineProperties.EndTime.TotalMilliseconds > 500)
        {
            progressPercent = (timelineProperties.Position.TotalMilliseconds / timelineProperties.EndTime.TotalMilliseconds) * 100;
        }

        // Create and return the timeline info object
        var timelineInfo = new TimelineInfo
        {
            CurrentTime = currentTime,
            TotalTime = totalTime,
            RawPosition = timelineProperties.Position,
            PositionMs = timelineProperties.Position.TotalMilliseconds,
            ProgressPercent = progressPercent,
            Duration = timelineProperties.EndTime
        };

        // Optional: Still display info to console
        Console.WriteLine($"Time:   {currentTime} / {totalTime}");
        Console.WriteLine($"Timeline Position (raw): {timelineProperties.Position}");
        Console.WriteLine($"Timeline Position (ms): {timelineProperties.Position.TotalMilliseconds}");
        Console.WriteLine($"Progress: {progressPercent:F1}%");

        return timelineInfo;
    }
    // public static async Task<TimelineInfo> getTimelineAsync(
    //         GlobalSystemMediaTransportControlsSession currentSession
    //     )
    // {

    //     return getTimeline(currentSession);
    // }

}