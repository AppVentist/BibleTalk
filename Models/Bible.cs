using System.Text.Json.Serialization;

namespace BibleTalk.Models
{
    // Ein einzelner Vers mit seiner Nummer
    public class BibleVerse
    {
        public required int Number { get; init; }
        public required string Text { get; init; }
    }

    // Ein aufgelöster Bibelabschnitt: Anzeigetitel plus die Verse
    public class BiblePassage
    {
        public required string Title { get; init; }
        public required IReadOnlyList<BibleVerse> Verses { get; init; }
    }

    // Eine geparste Referenz, z. B. "1Kor 8,1-3"
    // VerseRanges leer = ganzes Kapitel
    public class BibleReference
    {
        public required int BookNumber { get; init; }
        public required string BookName { get; init; }
        public required int Chapter { get; init; }
        public IReadOnlyList<(int From, int To)> VerseRanges { get; init; } = [];

        // "1. Korinther 8,1-3" bzw. "Psalm 23"
        public string Display
        {
            get
            {
                if (VerseRanges.Count == 0)
                    return $"{BookName} {Chapter}";

                var parts = VerseRanges.Select(r =>
                    r.From == r.To ? $"{r.From}" : $"{r.From}-{r.To}");

                return $"{BookName} {Chapter},{string.Join(".", parts)}";
            }
        }
    }

    // Deserialisierungs-Ziel für Resources/Raw/bibles/<übersetzung>/<nr>.json
    // Aufbau: chapters["8"]["1"] = "Von dem Götzenopfer aber wissen wir; ..."
    // Die Schlüssel sind bewusst Strings: System.Text.Json behandelt
    // numerische Dictionary-Keys sonst je nach Version unterschiedlich.
    public class BibleBookFile
    {
        [JsonPropertyName("number")]
        public int Number { get; init; }

        [JsonPropertyName("name")]
        public string Name { get; init; } = string.Empty;

        [JsonPropertyName("chapters")]
        public Dictionary<string, Dictionary<string, string>> Chapters { get; init; } = [];
    }
}
