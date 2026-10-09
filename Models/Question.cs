using System.Text.Json.Serialization;

namespace BibleTalk.Models
{
    // Eine einzelne Frage innerhalb eines Themas.
    // Das Label kommt aus der JSON ("Startfrage", "Frage 1", "Zusatzfrage", ...),
    // dadurch sind Anzahl und Reihenfolge der Fragen frei - ohne Codeänderung.
    public class Question
    {
        // Stabiler Schlüssel für die Notizen dieses Blocks.
        // WICHTIG: einmal vergeben, nie mehr ändern - sonst sind die
        // Notizen des Nutzers beim nächsten Inhalts-Update weg.
        // Fehlt das Feld, wird auf die Position im Thema zurückgegriffen.
        public string? Id { get; init; }

        // Notizfeld unter den Fragen dieses Blocks abschalten:
        // "showNote": false
        public bool ShowNote { get; init; } = true;

        // Beschriftung über dem Notizfeld
        public string? NotePrompt { get; init; }

        // Optional: Titel der aufklappbaren Box, z. B. "Frage 1".
        // Fehlt er in der JSON, wird automatisch "Frage N" angezeigt.
        public string? Label { get; init; }

        // Akzeptiert in der JSON einen einzelnen String ODER ein Array.
        // Bewusst NICHT required: ein fehlendes Feld darf nicht die
        // gesamte Datei unbrauchbar machen.
        [JsonConverter(typeof(StringOrArrayConverter))]
        public List<string> BibleQuestion { get; init; } = [];

        [JsonConverter(typeof(StringOrArrayConverter))]
        public List<string> ApplicationQuestion { get; init; } = [];

        // Beliebig viele Zusatzfragen
        public List<string>? AddedQuestions { get; init; }

        // Optional: Bibelstelle zur Frage, z. B. "Joh 15,9-17"
        public string? BibleReference { get; init; }

        // Optional: ausgeschriebener Bibeltext zur Stelle.
        // Nur noch für Sonderfälle - normalerweise wird der Text über
        // "passages" aus der gebündelten Übersetzung geholt.
        public string? BibleText { get; init; }

        // Optional: maschinenlesbare Bibelstellen, z. B. ["1Kor 8,1-3", "Kol 4,18"].
        // Bewusst NICHT required: Blöcke ohne Angabe zeigen einfach keinen Text.
        public List<string>? Passages { get; init; }

        [JsonIgnore]
        public bool HasBibleQuestion => BibleQuestion.Count > 0;

        [JsonIgnore]
        public bool HasApplicationQuestion => ApplicationQuestion.Count > 0;

        [JsonIgnore]
        public bool HasBibleReference => !string.IsNullOrWhiteSpace(BibleReference);

        [JsonIgnore]
        public bool HasBibleText => !string.IsNullOrWhiteSpace(BibleText);

        [JsonIgnore]
        public IReadOnlyList<string> ValidPassages =>
            Passages?.Where(p => !string.IsNullOrWhiteSpace(p)).ToList() ?? [];

        [JsonIgnore]
        public bool HasPassages => ValidPassages.Count > 0;

        [JsonIgnore]
        public bool HasAddedQuestions =>
            AddedQuestions is { Count: > 0 } &&
            AddedQuestions.Any(q => !string.IsNullOrWhiteSpace(q));
    }
}
