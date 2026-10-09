using System.Text.Json.Serialization;

namespace BibleTalk.Models
{
    // Ein Thema (Lektion) innerhalb einer Serie
    public class Topic
    {
        public required int Number { get; init; }
        public required string Title { get; init; }
        public string? MemoryVerse { get; init; }
        public required string Description { get; init; }

        // Startfrage der Lektion (aus der JSON)
        public string? StartingQuestion { get; init; }
        public string? Category { get; init; }
        public string? ConversationReference { get; init; }
        public int DurationMinutes { get; init; }
        
        // Optional: Themen ohne Fragen bleiben gültig (leere Liste)
        public IReadOnlyList<Question> Questions { get; init; } = [];

        // Jüngerschaft: Lehrabschnitte mit Fließtext und Notizfeld.
        // Quartalshefte haben dieses Feld nie - deshalb Default statt required.
        // Ist es gefüllt, öffnet die App die LessonPage statt der QuestionsPage.
        public IReadOnlyList<LessonSection> Sections { get; init; } = [];

        [JsonIgnore]
        public bool HasSections => Sections.Count > 0;

        // Gibt es überhaupt etwas zu öffnen? Steuert den Button auf der TopicPage.
        [JsonIgnore]
        public bool HasContent => HasQuestions || HasSections;

        // Steuert die Sichtbarkeit des Buttons "Gesprächsfragen"
        [JsonIgnore]
        public bool HasQuestions => Questions.Count > 0;

        // Steuert die Sichtbarkeit der Beschreibung
        [JsonIgnore]
        public bool HasMemoryVerse => !string.IsNullOrWhiteSpace(MemoryVerse);
        
        // Steuert die Sichtbarkeit der Beschreibung
        [JsonIgnore]
        public bool HasDescription => !string.IsNullOrWhiteSpace(Description);
        
        // Steuert die Sichtbarkeit der Startfrage
        [JsonIgnore]
        public bool HasStartingQuestion => !string.IsNullOrWhiteSpace(StartingQuestion);
    }
}
