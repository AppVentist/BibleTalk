using BibleTalk.Models;
using BibleTalk.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BibleTalk.ViewModels
{
    // Empfängt das gewählte Heft über die Shell-Navigation
    public partial class SeriesViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private Series? series;

        public ObservableCollection<TopicItemViewModel> Topics { get; } = [];

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (!query.TryGetValue("Series", out var value) || value is not Series s)
                return;

            // Beim Zurücknavigieren ruft Shell erneut mit denselben Parametern auf, daher prüfen wir, ob die Serie bereits gesetzt ist
            if (ReferenceEquals(Series, s) && Topics.Count > 0)
                return;

            Series = s;
            Topics.Clear();
            foreach (var topic in s.Topics)
                Topics.Add(new TopicItemViewModel(topic, s));
        }
    }
}
