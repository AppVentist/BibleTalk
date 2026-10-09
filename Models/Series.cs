using System.Text.Json.Serialization;

namespace BibleTalk.Models
{
    // Welchem Bereich eine Serie angehört. Steht bewusst NICHT in der JSON,
    // sondern wird beim Laden gesetzt - so kann eine Datei nicht im falschen
    // Bereich landen, nur weil jemand ein Feld vergisst.
    public enum SeriesKind
    {
        Quarterly,
        Discipleship,
        Homegroup,
        Conversation
    }

    // Eine Serie aus Themen: Quartalsheft, Jüngerschaft, Hauskreisthemen
    public class Series
    {
        public required string Title { get; init; }

        // Bsp: bei Standardausgabe: "JUL | AUG | SEP 2026", bei Jüngerschaft: "8 Einheiten" usw.
        // In der JSON weiterhin "period", damit die bestehenden Heft-Dateien
        // unverändert bleiben.
        [JsonPropertyName("period")]
        public string? Subtitle { get; init; }

        public required string CoverImage { get; init; }

        // NICHT required: eine vorbereitete, noch leere Datei darf nicht die
        // Deserialisierung kippen und damit wortlos verschwinden.
        public IReadOnlyList<Topic> Topics { get; init; } = [];

        [JsonIgnore]
        public SeriesKind Kind { get; set; } = SeriesKind.Quarterly;

        // Wie eine Einheit in diesem Bereich heißt - steuert die Überschrift
        // auf der TopicPage ("Thema 3" vs. "Einheit 3" vs. "Abend 3")
        [JsonIgnore]
        public string TopicWord => Kind switch
        {
            SeriesKind.Discipleship => "Einheit",
            SeriesKind.Homegroup => "Abend",
            _ => "Thema"
        };

        // Bezeichner aus index.json, z. B. "2026_q3". Steht bewusst NICHT in
        // der Serien-Datei selbst, sondern wird beim Laden gesetzt.
        [JsonIgnore]
        public string Id { get; set; } = string.Empty;

        // Stabiler, dateisystem-tauglicher Bezeichner der Serie.
        // Wird für die Schlüssel der Notizen gebraucht und haengt deshalb an
        // der Id aus index.json, NICHT am Titel: ein korrigierter Tippfehler
        // im Titel wuerde sonst saemtliche Notizen der Serie unerreichbar
        // machen. Nur falls die Id fehlt, wird ersatzweise der Titel benutzt.
        [JsonIgnore]
        public string Slug => Normalize(string.IsNullOrWhiteSpace(Id) ? Title : Id);

        private static string Normalize(string value) =>
            new string(value.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();

        // Überschrift der Themenliste auf der SeriesPage ("Themen" vs. "Einheiten" vs. "Abende")
        [JsonIgnore]
        public string TopicListHeader => Kind switch
        {
            SeriesKind.Discipleship => "Einheiten",
            SeriesKind.Homegroup => "Abende",
            _ => "Themen"
        };
    }
}
