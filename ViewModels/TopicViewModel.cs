using System.Collections.ObjectModel;
using BibleTalk.Models;
using BibleTalk.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BibleTalk.ViewModels
{
    // Empfängt Thema + Heft über die Shell-Navigation und stellt ALLE Themen
    // des Hefts als wischbares CarouselView bereit. Das angetippte Thema
    // bestimmt nur die Startposition.
    public partial class TopicViewModel : ObservableObject, IQueryAttributable
    {
        private Series? _series;

        public ObservableCollection<TopicItemViewModel> Topics { get; } = [];

        // Cover des Hefts – für alle Themen gleich, deshalb auf Seitenebene
        [ObservableProperty]
        private string? coverImage;

        [ObservableProperty]
        private TopicItemViewModel? currentTopic;

        // Merkt sich das zuletzt angezeigte Thema über Navigationswechsel hinweg
        public int SelectedIndex { get; set; }

        partial void OnCurrentTopicChanged(TopicItemViewModel? value)
        {
            if (value is not null)
            {
                SelectedIndex = Topics.IndexOf(value);
                if (_series is not null) ReadingHistory.Save(_series, value.Topic);
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (!query.TryGetValue("Series", out var s) || s is not Series series)
                return;

            // Beim Zurücknavigieren von der QuestionsPage ruft Shell diese Methode
            // erneut mit den ursprünglichen Parametern auf. Dann darf die Liste
            // NICHT neu aufgebaut werden, sonst springt das Karussell zurück.
            if (ReferenceEquals(_series, series) && Topics.Count > 0)
                return;

            _series = series;
            CoverImage = series.CoverImage;

            Topics.Clear();
            foreach (var topic in series.Topics)
                Topics.Add(new TopicItemViewModel(topic, series));

            // Auf das angetippte Thema springen, sonst auf das erste
            query.TryGetValue("Topic", out var t);
            CurrentTopic = t is Topic selected
                ? Topics.FirstOrDefault(i => ReferenceEquals(i.Topic, selected)) ?? Topics.FirstOrDefault()
                : Topics.FirstOrDefault();
        }
    }
}
