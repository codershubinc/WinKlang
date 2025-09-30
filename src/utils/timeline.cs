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
    public static TimelineInfo getTimeline(GlobalSystemMediaTransportControlsSession currentSession)
    {
        var timelineProperties = currentSession.GetTimelineProperties();
        string currentTime = timelineProperties.Position.ToString(@"mm\:ss");
        string totalTime = timelineProperties.EndTime.ToString(@"mm\:ss");

        double progressPercent = 0;
        if (timelineProperties.EndTime.TotalMilliseconds > 0)
        {
            progressPercent = (timelineProperties.Position.TotalMilliseconds / timelineProperties.EndTime.TotalMilliseconds) * 100;
        }

        var timelineInfo = new TimelineInfo
        {
            CurrentTime = currentTime,
            TotalTime = totalTime,
            RawPosition = timelineProperties.Position,
            PositionMs = timelineProperties.Position.TotalMilliseconds,
            ProgressPercent = progressPercent,
            Duration = timelineProperties.EndTime
        };

        return timelineInfo;
    }

    public static async Task DisplayTimelineInfoAsync(GlobalSystemMediaTransportControlsSession currentSession)
    {
        Console.WriteLine(">> Synchronizing timeline data...");
        Console.WriteLine(">> If it gives wrong time data THEN its windows problem FK windows...");

        var timelineInfo = getTimeline(currentSession);

        Console.WriteLine(">> PLAYBACK TIMELINE:");
        Console.WriteLine($"   << Position:     {timelineInfo.CurrentTime}");
        Console.WriteLine($"   >> Duration:     {timelineInfo.TotalTime}");

        var remainingTime = timelineInfo.Duration - timelineInfo.RawPosition;
        var remainingFormatted = remainingTime.ToString(@"mm\:ss");
        Console.WriteLine($"   ~ Remaining:    {remainingFormatted}");
        Console.WriteLine($"   % Progress:     {timelineInfo.ProgressPercent:F1}%");

        int progressBarWidth = 50;
        int filledWidth = (int)(timelineInfo.ProgressPercent / 100.0 * progressBarWidth);
        string progressBar = new string('=', filledWidth) + new string('-', progressBarWidth - filledWidth);
        Console.WriteLine($"   [{progressBar}] {timelineInfo.ProgressPercent:F1}%");

        Console.WriteLine($"   {timelineInfo.CurrentTime} ================================================ {timelineInfo.TotalTime}");

        Console.WriteLine();
        Console.WriteLine(">> DEBUG INFO:");
        Console.WriteLine($"   Time:   {timelineInfo.CurrentTime} / {timelineInfo.TotalTime}");
        Console.WriteLine($"   Timeline Position (raw): {timelineInfo.RawPosition}");
        Console.WriteLine($"   Timeline Position (ms): {timelineInfo.PositionMs}");
        Console.WriteLine($"   Progress: {timelineInfo.ProgressPercent:F1}%");
        Console.WriteLine();
        Console.WriteLine("=================================================================");
    }
}
