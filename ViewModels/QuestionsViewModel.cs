using System.Collections.ObjectModel;
using BibleTalk.Models;
using BibleTalk.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BibleTalk.ViewModels
{
    // Fokus-Modus: Empfängt das Thema über die Shell-Navigation und zeigt
    // die Fragen EINZELN nacheinander an - mit Fortschrittsbalken,
    // Vor-/Zurück-Steuerung und Wischgeste (CarouselView).
    public partial class QuestionsViewModel : ObservableObject, IQueryAttributable
    {
        private readonly IBibleTextResolver _bibleTextResolver;
        private readonly INoteService _notes;

        // Wird für die Notiz-Schlüssel gebraucht
        private Series? _series;

        public QuestionsViewModel(IBibleTextResolver bibleTextResolver, INoteService notes)
        {
            _bibleTextResolver = bibleTextResolver;
            _notes = notes;
        }

        [ObservableProperty]
        private Topic? topic;

        // Flache Liste ALLER Einzelfragen des Themas
        public ObservableCollection<QuestionStepViewModel> Steps { get; } = [];

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ProgressLabel))]
        [NotifyPropertyChangedFor(nameof(Progress))]
        [NotifyPropertyChangedFor(nameof(IsLastStep))]
        [NotifyPropertyChangedFor(nameof(NextButtonText))]
        [NotifyCanExecuteChangedFor(nameof(PreviousCommand))]
        private int currentIndex;

        public int TotalSteps => Steps.Count;

        public bool HasSteps => Steps.Count > 0;

        public bool IsEmpty => Steps.Count == 0;

        // "3 / 12"
        public string ProgressLabel =>
            TotalSteps == 0 ? string.Empty : $"{CurrentIndex + 1} / {TotalSteps}";

        // 0.0 - 1.0 für die ProgressBar
        public double Progress =>
            TotalSteps == 0 ? 0 : (CurrentIndex + 1) / (double)TotalSteps;

        public bool IsLastStep => CurrentIndex >= TotalSteps - 1;

        public string NextButtonText => IsLastStep ? "Fertig" : "Nächste Frage";

        [RelayCommand(CanExecute = nameof(CanGoPrevious))]
        private void Previous()
        {
            if (CanGoPrevious())
                CurrentIndex--;
        }

        private bool CanGoPrevious() => CurrentIndex > 0;

        // Auf der letzten Frage wird aus "Weiter" ein "Fertig",
        // das zurück zum Thema führt
        [RelayCommand]
        private async Task NextAsync()
        {
            if (IsLastStep)
                await Shell.Current.GoToAsync("..");
            else
                CurrentIndex++;
        }

        [RelayCommand]
        private async Task CloseAsync() => await Shell.Current.GoToAsync("..");

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (!query.TryGetValue("Topic", out var t) || t is not Topic topicValue)
                return;

            // Die Serie wird für die Notiz-Schlüssel gebraucht. Fehlt sie,
            // laufen die Fragen weiter - nur ohne Notizfeld.
            query.TryGetValue("Series", out var s);
            var seriesValue = s as Series;

            // Beim erneuten Aufruf mit demselben Thema die Position beibehalten
            if (ReferenceEquals(Topic, topicValue) && Steps.Count > 0)
                return;

            Topic = topicValue;
            _series = seriesValue;
            BuildSteps(topicValue);

            // Die Bibeltexte kommen von der Platte und brauchen einen Moment.
            // Bewusst nicht abgewartet: die Fragen sollen sofort stehen.
            LoadBibleTexts();
        }

        // Bewusst async void mit eigenem catch: ein "_ = Task" wuerde eine
        // Ausnahme unbeobachtet lassen.
        private async void LoadBibleTexts()
        {
            try
            {
                await _bibleTextResolver.LoadAsync(Steps);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Bibeltexte der Fragen: {ex}");
            }
        }

        // Von der QuestionsPage beim Verlassen aufgerufen: schreibt Notizen,
        // die noch in der Speicher-Verzoegerung haengen.
        public void FlushNotes()
        {
            foreach (var step in Steps)
                step.FlushNote();
        }

        // Blöcke aus der JSON in einzelne Schritte auflösen.
        // Reihenfolge je Block: Gesprächsfragen, Anwendungsfragen, Zusatzfragen.
        private void BuildSteps(Topic topicValue)
        {
            Steps.Clear();

            var sectionNumber = 0;

            // Läuft durch ALLE Fragen des Themas durch und wird nie
            // je Block zurückgesetzt - so bleibt die Nummerierung fortlaufend.
            var questionNumber = 0;

            foreach (var question in topicValue.Questions)
            {
                sectionNumber++;
                var label = string.IsNullOrWhiteSpace(question.Label)
                    ? $"Abschnitt {sectionNumber}"
                    : question.Label!;

                // Ohne "id" in der JSON auf die Position ausweichen.
                // Funktioniert, ist aber gegen Umsortieren nicht sicher.
                var blockId = string.IsNullOrWhiteSpace(question.Id)
                    ? $"pos{sectionNumber}"
                    : question.Id!;

                // Nur speichern, wenn die Serie bekannt ist - sonst gäbe es
                // keinen eindeutigen Schlüssel und die Notiz landete unter
                // einem Namen, der später mit einem anderen Heft kollidiert.
                var canStoreNotes = _series is not null && question.ShowNote;

                void Add(IEnumerable<string>? texts, QuestionKind kind)
                {
                    if (texts is null)
                        return;

                    // Zählt INNERHALB von Block und Fragentyp - dadurch bleibt
                    // der Schlüssel stabil, wenn an anderer Stelle im Thema
                    // eine Frage ergänzt oder entfernt wird.
                    var indexInKind = 0;

                    foreach (var text in texts)
                    {
                        if (string.IsNullOrWhiteSpace(text))
                            continue;

                        indexInKind++;

                        var noteKey = canStoreNotes
                            ? _notes.BuildKey(_series!, topicValue,
                                  $"{blockId}_{QuestionStepViewModel.KindKey(kind)}{indexInKind}")
                            : null;

                        Steps.Add(new QuestionStepViewModel(
                            ++questionNumber,
                            sectionNumber,
                            label,
                            question.BibleReference,
                            question.BibleText,
                            question.ValidPassages,
                            kind,
                            text.Trim(),
                            _notes,
                            noteKey,
                            question.NotePrompt,
                            canStoreNotes));
                    }
                }

                Add(question.BibleQuestion, QuestionKind.Bible);
                Add(question.ApplicationQuestion, QuestionKind.Application);
                Add(question.AddedQuestions, QuestionKind.Added);
            }

            CurrentIndex = 0;
            OnPropertyChanged(nameof(TotalSteps));
            OnPropertyChanged(nameof(HasSteps));
            OnPropertyChanged(nameof(IsEmpty));
            OnPropertyChanged(nameof(ProgressLabel));
            OnPropertyChanged(nameof(Progress));
            OnPropertyChanged(nameof(IsLastStep));
            OnPropertyChanged(nameof(NextButtonText));
        }
    }
}
