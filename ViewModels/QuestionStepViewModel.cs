using BibleTalk.Services;

namespace BibleTalk.ViewModels
{
    // Eine EINZELNE Frage im Fokus-Modus.
    // Die JSON gruppiert Fragen in Blöcken (ein Block = ein Bibelabschnitt mit
    // mehreren Gesprächs-, Anwendungs- und Zusatzfragen). Für den Fokus-Modus
    // wird diese Struktur flachgeklopft: jede Frage wird ein eigener Schritt,
    // trägt aber den Kontext ihres Blocks (Abschnitt, Bibelstelle, Text) mit.
    //
    // Bibeltext-Karte und Notizfeld stecken in BibleStepViewModel.
    public partial class QuestionStepViewModel : BibleStepViewModel
    {
        public QuestionStepViewModel(
            int number,
            int sectionNumber,
            string sectionLabel,
            string? bibleReference,
            string? bibleText,
            IReadOnlyList<string> passages,
            QuestionKind kind,
            string text,
            INoteService? notes = null,
            string? noteKey = null,
            string? notePrompt = null,
            bool showNote = false)
            : base(bibleText, passages, notes, noteKey, notePrompt, showNote)
        {
            Number = number;
            SectionNumber = sectionNumber;
            SectionLabel = sectionLabel;
            BibleReference = bibleReference;
            Kind = kind;
            Text = text;
        }

        // Fortlaufende Nummer über ALLE Fragen des Themas hinweg -
        // unabhängig davon, ob Gesprächs-, Anwendungs- oder Zusatzfrage.
        public int Number { get; }

        public int SectionNumber { get; }
        public string SectionLabel { get; }
        public string? BibleReference { get; }

        public QuestionKind Kind { get; }
        public string Text { get; }

        // Kopfzeile über der Frage, z. B. "Abschnitt 2 · Anwendungsfrage"
        public string SectionHeader => $"Abschnitt {SectionNumber} · {KindLabel}";

        public string KindLabel => Kind switch
        {
            QuestionKind.Application => "Anwendungsfrage",
            QuestionKind.Added => "Zusatzfrage",
            _ => "Gesprächsfrage"
        };

        // Farbcode je Fragentyp - blau für Gespräch, orange für Anwendung,
        // grau für Zusatzfragen. So bleibt die Seite ruhig, aber unterscheidbar.
        public Color KindColor => Kind switch
        {
            QuestionKind.Application => Color.FromArgb("#D97706"),
            QuestionKind.Added => Color.FromArgb("#6E6E6E"),
            _ => Color.FromArgb("#2E5797")
        };

        // Die Blocküberschrift enthält meist schon die Bibelstelle.
        // Nur wenn ein eigenes bibleReference-Feld gefüllt ist, hat es Vorrang.
        public string ReferenceDisplay =>
            string.IsNullOrWhiteSpace(BibleReference) ? SectionLabel : BibleReference!;

        // Kurzform des Fragentyps - Bestandteil des Notiz-Schlüssels
        public static string KindKey(QuestionKind kind) => kind switch
        {
            QuestionKind.Application => "a",
            QuestionKind.Added => "z",
            _ => "b"
        };
    }

    public enum QuestionKind
    {
        Bible,
        Application,
        Added
    }
}
