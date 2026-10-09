using BibleTalk.Models;
using BibleTalk.Views;
using BibleTalk.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BibleTalk.ViewModels
{
    // Wrapper um Topic: bündelt Themen-Daten und den Command für die
    // Fragen-Seite. Dadurch kommt das CarouselView-Template ohne
    // RelativeSource-Bindings aus (SourceGen-Inflator verträgt die nicht).
    public partial class TopicItemViewModel : ObservableObject
    {
        private readonly Series _series;
        public TopicItemViewModel(Topic topic, Series series)
        {
            Topic = topic;
            _series = series;
            NumberLabel = $"{series.TopicWord} {topic.Number}"; // "Thema 3" / "Einheit 3" / "Abend 3"
        }

        // "Thema 3" vs. "Einheit 3" vs. "Abend 3" – abhängig vom Bereich
        public string NumberLabel { get; }

        // Das gekapselte Model – wird beim Navigieren weitergereicht
        public Topic Topic { get; }

        public int Number => Topic.Number;
        public string Title => Topic.Title;
        public string? MemoryVerse => Topic.MemoryVerse;
        public bool HasMemoryVerse => Topic.HasMemoryVerse;
        public string Description => Topic.Description;
        public bool HasDescription => Topic.HasDescription;
        public string? StartingQuestion => Topic.StartingQuestion;
        public bool HasStartingQuestion => Topic.HasStartingQuestion;
        public bool HasQuestions => Topic.HasQuestions;

        // Gibt es überhaupt etwas zu öffnen?
        public bool HasContent => Topic.HasContent;

        // Beschriftung des Buttons - je nach Bereich
        public string ActionLabel => _series.Kind switch
        {
            SeriesKind.Discipleship => "Einheit öffnen",
            SeriesKind.Homegroup => "Abend öffnen",
            _ => "Gesprächsfragen"
        };

        // Von der SeriesPage: Themenliste -> Themendetail
        [RelayCommand]
        private async Task OpenTopicAsync()
        {
            await Shell.Current.GoToAsync(nameof(TopicPage),
                new Dictionary<string, object>
                {
                    ["Topic"] = Topic,
                    ["Series"] = _series
                });
        }
        
        // Von der TopicPage: Button unter dem Thema.
        //
        // Das ist die EINZIGE Weiche zwischen den Darstellungsformen.
        // Kommt ein weiterer Bereich dazu, wird hier eine Zeile ergaenzt -
        // nirgends sonst.
        [RelayCommand]
        private async Task OpenQuestionsAsync()
        {
            ReadingHistory.Save(_series, Topic);
            // Jüngerschaft nur dann auf die LessonPage schicken, wenn die
            // Einheit auch wirklich "sections" mitbringt. Bestehende Dateien
            // im Frage-Format laufen so unveraendert weiter.
            var route = (_series.Kind, Topic.HasSections) switch
            {
                (SeriesKind.Discipleship, true) => nameof(LessonPage),
                _ => nameof(QuestionsPage)
            };

            await Shell.Current.GoToAsync(route,
                new Dictionary<string, object>
                {
                    ["Topic"] = Topic,
                    ["Series"] = _series
                });
        }
    }
}
