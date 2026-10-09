using System.Collections.ObjectModel;
using BibleTalk.Models;
using BibleTalk.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BibleTalk.ViewModels
{
    // Jüngerschaft: eine Einheit als durchgehender Lesetext mit Notizfeldern.
    //
    // Bewusst ANDERS als die QuestionsPage: dort ist eine Frage pro Bildschirm
    // richtig, weil die Gruppe eine Frage nach der anderen bespricht. Hier
    // schreibt der Einzelne mit und blättert zurück - also eine ScrollView
    // über alle Abschnitte statt eines Karussells.
    public partial class LessonViewModel : ObservableObject, IQueryAttributable
    {
        private readonly IBibleTextResolver _bibleTextResolver;
        private readonly INoteService _notes;

        private Topic? _topic;

        public LessonViewModel(IBibleTextResolver bibleTextResolver, INoteService notes)
        {
            _bibleTextResolver = bibleTextResolver;
            _notes = notes;
        }

        public ObservableCollection<LessonSectionViewModel> Sections { get; } = [];

        // "Einheit 3"
        [ObservableProperty]
        private string numberLabel = string.Empty;

        [ObservableProperty]
        private string title = string.Empty;

        [ObservableProperty]
        private string memoryVerse = string.Empty;

        public bool HasMemoryVerse => !string.IsNullOrWhiteSpace(MemoryVerse);

        public bool HasSections => Sections.Count > 0;
        public bool IsEmpty => Sections.Count == 0;

        [RelayCommand]
        private async Task CloseAsync() => await Shell.Current.GoToAsync("..");

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (!query.TryGetValue("Topic", out var t) || t is not Topic topic)
                return;
            if (!query.TryGetValue("Series", out var s) || s is not Series series)
                return;

            // Beim Zurückkehren auf dieselbe Einheit nicht neu aufbauen -
            // sonst sind die Notizfelder kurz leer und die Scrollposition weg.
            if (ReferenceEquals(_topic, topic) && Sections.Count > 0)
                return;

            _topic = topic;
            NumberLabel = $"{series.TopicWord} {topic.Number}";
            Title = topic.Title;
            MemoryVerse = topic.MemoryVerse ?? string.Empty;
            OnPropertyChanged(nameof(HasMemoryVerse));

            Sections.Clear();

            var index = 0;
            foreach (var section in topic.Sections)
            {
                index++;

                // Ohne "id" in der JSON auf die Position ausweichen.
                // Funktioniert, ist aber gegen Umsortieren nicht sicher.
                var sectionId = string.IsNullOrWhiteSpace(section.Id)
                    ? $"pos{index}"
                    : section.Id!;

                Sections.Add(new LessonSectionViewModel(
                    index,
                    section,
                    _notes.BuildKey(series, topic, sectionId),
                    _notes));
            }

            OnPropertyChanged(nameof(HasSections));
            OnPropertyChanged(nameof(IsEmpty));

            // Nicht abgewartet: der Text soll sofort stehen,
            // die Bibelstellen tröpfeln nach.
            LoadBibleTexts();
        }

        // Bewusst async void mit eigenem catch: ein "_ = Task" wuerde eine
        // Ausnahme unbeobachtet lassen.
        private async void LoadBibleTexts()
        {
            try
            {
                await _bibleTextResolver.LoadAsync(Sections);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Bibeltexte der Einheit: {ex}");
            }
        }

        // Von der LessonPage beim Verlassen aufgerufen: schreibt Notizen,
        // die noch in der Speicher-Verzoegerung haengen.
        public void FlushNotes()
        {
            foreach (var section in Sections)
                section.FlushNote();
        }
    }
}
