using BibleTalk.Models;
namespace BibleTalk.Services;

public static class ReadingHistory
{
    public static void Save(Series series, Topic topic)
    {
        Preferences.Default.Set("reading.series", series.Id);
        Preferences.Default.Set("reading.kind", (int)series.Kind);
        Preferences.Default.Set("reading.topic", topic.Number);
    }
    public static (string Id, SeriesKind Kind, int Number) Last =>
        (Preferences.Default.Get("reading.series", ""),
         (SeriesKind)Preferences.Default.Get("reading.kind", 0),
         Preferences.Default.Get("reading.topic", 0));
}
