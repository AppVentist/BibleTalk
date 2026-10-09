using System.Text.Json.Serialization;

namespace BibleTalk.Models
{
    // Ein Lehrabschnitt einer Jüngerschafts-Einheit: Fließtext statt Frage,
    // optional mit Bibelstelle und Notizfeld.
    //
    // Bewusst KEIN "required"-Feld: eine Einheit, in der ein Abschnitt noch
    // unvollständig ist, darf nicht die gesamte Datei stillschweigend kippen.
    public class LessonSection
    {
        // Stabiler Schlüssel für die gespeicherte Notiz.
        // WICHTIG: einmal vergeben, nie mehr ändern - sonst verliert der
        // Nutzer beim nächsten Inhalts-Update seine Notiz. Fehlt das Feld,
        // wird auf die Position zurückgegriffen (dann ist Umsortieren riskant).
        public string? Id { get; init; }

        public string? Heading { get; init; }

        // Akzeptiert einen einzelnen String ODER ein Array von Absätzen
        [JsonConverter(typeof(StringOrArrayConverter))]
        public List<string> Text { get; init; } = [];

        // Maschinenlesbare Bibelstellen, z. B. ["Mt 16,13-20"]
        public List<string>? Passages { get; init; }

        // Sonderfall: von Hand gepflegter Text statt Nachschlagen
        public string? BibleText { get; init; }

        // Beschriftung über dem Notizfeld, z. B. "Was nimmst du dir vor?"
        public string? NotePrompt { get; init; }

        // Abschnitte ohne Notizfeld (z. B. reine Einleitung): "showNote": false
        public bool ShowNote { get; init; } = true;

        [JsonIgnore]
        public IReadOnlyList<string> ValidPassages =>
            Passages?.Where(p => !string.IsNullOrWhiteSpace(p)).ToList() ?? [];

        [JsonIgnore]
        public IReadOnlyList<string> ValidParagraphs =>
            Text.Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
    }
}
