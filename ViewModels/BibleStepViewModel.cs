using System.Collections.ObjectModel;
using BibleTalk.Models;
using BibleTalk.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BibleTalk.ViewModels
{
    // Gemeinsame Basis für ALLES, was als einzelner Schritt angezeigt wird:
    // eine Einzelfrage (QuestionStepViewModel) ebenso wie ein Lehrabschnitt
    // der Jüngerschaft (LessonSectionViewModel).
    //
    // Bündelt zwei Dinge, die sonst mehrfach existieren müssten:
    //   1. die Bibeltext-Karte (Views/Controls/BibleTextCard.xaml)
    //   2. das Notizfeld samt Speicherung
    public abstract partial class BibleStepViewModel : ObservableObject
    {
        // Schreibintervall der Notiz. Ohne Verzoegerung landete pro Tastendruck
        // ein Preferences-Commit auf der Platte.
        private static readonly TimeSpan SaveDelay = TimeSpan.FromMilliseconds(500);

        private readonly INoteService? _notes;
        private readonly string? _noteKey;

        private CancellationTokenSource? _saveCts;

        protected BibleStepViewModel(
            string? bibleText,
            IReadOnlyList<string> passages,
            INoteService? notes = null,
            string? noteKey = null,
            string? notePrompt = null,
            bool showNote = false)
        {
            BibleText = bibleText;
            Passages = passages;

            _notes = notes;
            _noteKey = noteKey;

            NotePrompt = string.IsNullOrWhiteSpace(notePrompt) ? "Meine Notizen" : notePrompt!;

            // Ohne Schlüssel oder ohne Dienst wäre die Notiz nach dem
            // Wegblättern weg - dann lieber gar kein Feld anbieten.
            ShowNote = showNote && notes is not null && !string.IsNullOrWhiteSpace(noteKey);

            // Direkt auf das FELD schreiben, nicht auf die Property:
            // sonst löst der Setter beim Laden sofort ein Speichern aus.
            if (ShowNote)
                note = notes!.Get(noteKey!);
        }

        // ---------- Bibeltext ----------

        // Von Hand gepflegter Text aus der JSON (Sonderfall)
        public string? BibleText { get; }

        // Rohangaben aus der JSON, z. B. ["1Kor 8,1-3"]
        public IReadOnlyList<string> Passages { get; }

        public bool HasManualBibleText => !string.IsNullOrWhiteSpace(BibleText);

        // Aufgelöste Abschnitte aus der gebündelten Übersetzung
        public ObservableCollection<BiblePassage> BiblePassages { get; } = [];

        // Quellenangabe unter dem Text, z. B. "Luther 1912"
        [ObservableProperty]
        private string? translationName;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasBibleSection))]
        private bool isLoadingBibleText;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasBibleSection))]
        private bool hasResolvedPassages;

        // Steuert, ob die Bibeltext-Karte überhaupt erscheint
        public bool HasBibleSection =>
            HasManualBibleText || HasResolvedPassages || IsLoadingBibleText;

        // Startet aufgeklappt: der Text wird meist mitgelesen
        [ObservableProperty]
        private bool isBibleTextExpanded = true;

        [RelayCommand]
        private void ToggleBibleText() => IsBibleTextExpanded = !IsBibleTextExpanded;

        // Wird vom BibleTextResolver nach dem Laden aufgerufen
        public void ApplyPassages(IEnumerable<BiblePassage> passages, string translation)
        {
            BiblePassages.Clear();
            foreach (var passage in passages)
                BiblePassages.Add(passage);

            TranslationName = translation;
            HasResolvedPassages = BiblePassages.Count > 0;
        }

        // ---------- Notiz ----------

        // Beschriftung über dem Feld, z. B. "Was nimmst du dir vor?"
        public string NotePrompt { get; }

        // Steuert die Sichtbarkeit des gesamten Notizblocks
        public bool ShowNote { get; }

        [ObservableProperty]
        private string note = string.Empty;

        // Speichert entprellt: erst wenn eine halbe Sekunde lang nichts mehr
        // getippt wurde. Jeder neue Tastendruck verwirft den vorherigen Lauf.
        partial void OnNoteChanged(string value)
        {
            if (!CanStore)
                return;

            _saveCts?.Cancel();
            _saveCts?.Dispose();

            var cts = new CancellationTokenSource();
            _saveCts = cts;

            _ = SaveDelayedAsync(value, cts.Token);
        }

        private async Task SaveDelayedAsync(string value, CancellationToken token)
        {
            try
            {
                await Task.Delay(SaveDelay, token);
                _notes!.Set(_noteKey!, value);
            }
            catch (OperationCanceledException)
            {
                // Es wurde weitergetippt - der naechste Lauf schreibt.
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Notiz speichern fehlgeschlagen: {ex}");
            }
        }

        // Schreibt eine noch ausstehende Aenderung sofort. Wird beim Verlassen
        // der Seite aufgerufen, damit die letzten Zeichen nicht in der
        // Verzoegerung haengen bleiben.
        public void FlushNote()
        {
            if (!CanStore)
                return;

            _saveCts?.Cancel();
            _saveCts?.Dispose();
            _saveCts = null;

            _notes!.Set(_noteKey!, Note);
        }

        private bool CanStore => _notes is not null && !string.IsNullOrWhiteSpace(_noteKey);
    }
}
