using BibleTalk.Models;
using BibleTalk.Services;

namespace BibleTalk.ViewModels
{
    // Ein Lehrabschnitt der Jüngerschaft: Überschrift, Fließtext,
    // optional Bibeltext-Karte und Notizfeld.
    //
    // Bibeltext UND Notiz stecken in BibleStepViewModel - hier bleibt
    // nur noch, was für die Jüngerschaft eigen ist.
    public partial class LessonSectionViewModel : BibleStepViewModel
    {
        public LessonSectionViewModel(
            int number,
            LessonSection section,
            string noteKey,
            INoteService notes)
            : base(section.BibleText,
                   section.ValidPassages,
                   notes,
                   noteKey,
                   section.NotePrompt,
                   section.ShowNote)
        {
            Number = number;
            Heading = string.IsNullOrWhiteSpace(section.Heading)
                ? $"Abschnitt {number}"
                : section.Heading!;
            Paragraphs = section.ValidParagraphs;
            HasParagraphs = Paragraphs.Count > 0;
        }

        public int Number { get; }
        public string Heading { get; }

        // Absätze des Abschnitts - jeder wird ein eigenes Label
        public IReadOnlyList<string> Paragraphs { get; }
        public bool HasParagraphs { get; }
    }
}
