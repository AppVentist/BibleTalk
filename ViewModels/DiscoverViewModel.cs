using System.Collections.ObjectModel;
using BibleTalk.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;

namespace BibleTalk.ViewModels;

public record DiscoverEntry(string Title, string Subtitle, string CoverImage, ICommand OpenCommand)
{
    public bool HasSubtitle => !string.IsNullOrWhiteSpace(Subtitle);
    public bool HasCover => !string.IsNullOrWhiteSpace(CoverImage);
}

public partial class DiscoverViewModel : ObservableObject, IQueryAttributable
{
    private const string Alltag = "Alltag";
    private const string Glaube = "Glaube";
    private const string Bibel = "Bibel";

    private readonly List<ConversationItemViewModel> _all = [];
    public ObservableCollection<DiscoverEntry> Items { get; } = [];
    [ObservableProperty] private string searchText = "";
    private string _category = "";

    public string Heading => !string.IsNullOrWhiteSpace(SearchText) ? "Suchergebnisse"
        : _category.Length > 0 ? _category : "Themen suchen";
    public string Introduction => !string.IsNullOrWhiteSpace(SearchText)
        ? "Ergebnisse aus allen drei Themenbereichen"
        : _category switch
        {
            Alltag => "Was beschäftigt mich gerade?",
            Glaube => "Was möchte ich besser verstehen?",
            Bibel => "Welchen Bibeltext möchte ich studieren?",
            _ => "Alltag, Glaube und Bibel"
        };

    public string EmptyMessage => _category.Length == 0 && string.IsNullOrWhiteSpace(SearchText)
        ? "Gib ein Thema oder eine Bibelstelle ein."
        : "Keine passenden Themen gefunden.";

    partial void OnSearchTextChanged(string value) => RefreshResults();

    private void RefreshResults()
    {
        OnPropertyChanged(nameof(Heading));
        OnPropertyChanged(nameof(Introduction));
        OnPropertyChanged(nameof(EmptyMessage));
        Filter();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("SeriesList", out var value) || value is not Series[] series) return;
        query.TryGetValue("Category", out var category);
        _category = category is Alltag or Glaube or Bibel ? (string)category : "";
        _all.Clear();
        foreach (var entry in series)
            foreach (var topic in entry.Topics.Where(t => t.HasContent))
                _all.Add(new(entry, topic));
        if (string.IsNullOrEmpty(SearchText))
            RefreshResults();
        else
            SearchText = "";
    }

    private void Filter()
    {
        Items.Clear();
        var search = Normalize(SearchText?.Trim() ?? "");
        if (search.Length > 0)
        {
            foreach (var item in _all.Where(item => Matches(item, search)))
                Items.Add(new(item.Title, item.Details, "", item.ReadCommand));
        }
        else if (_category.Length > 0)
        {
            var topics = _all.Where(item => item.Category == _category);
            var showSeries = _category == Bibel;
            foreach (var series in topics.Where(item => showSeries && item.Series.Kind == SeriesKind.Homegroup)
                         .Select(item => item.Series).Distinct())
            {
                var entry = new SeriesItemViewModel(series);
                Items.Add(new(entry.Title, entry.Subtitle ?? "", entry.CoverImage, entry.OpenCommand));
            }

            foreach (var item in topics.Where(item => !showSeries || item.Series.Kind != SeriesKind.Homegroup))
            {
                var subtitle = _category == Glaube ? "" : item.Topic.Description ?? item.Details;
                Items.Add(new(item.Title, subtitle, "", item.ReadCommand));
            }
        }
    }

    private static bool Matches(ConversationItemViewModel item, string search)
    {
        var topic = item.Topic;
        var terms = new[] { item.Title, item.Details, item.Category, item.Series.Title, topic.MemoryVerse }
            .Concat(topic.Questions.SelectMany(question => question.ValidPassages
                .Append(question.BibleReference).Append(question.Label)))
            .Concat(topic.Sections.SelectMany(section => section.ValidPassages));
        return terms.Any(term => term is not null &&
            Normalize(term).Contains(search, StringComparison.CurrentCultureIgnoreCase));
    }

    private static string Normalize(string value) => value.Replace('–', '-').Replace('—', '-');
}
