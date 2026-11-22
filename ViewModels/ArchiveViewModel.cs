using BibleTalk.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BibleTalk.ViewModels;

public partial class ArchiveViewModel : ObservableObject, IQueryAttributable
{
    private SeriesItemViewModel[] _all = [];

    [ObservableProperty]
    private IReadOnlyList<SeriesItemViewModel> items = [];

    [ObservableProperty]
    private string searchText = "";

    [ObservableProperty]
    private string emptyMessage = "Keine Quartalshefte im Archiv verfügbar.";

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("SeriesList", out var value) && value is Series[] series)
        {
            _all = series.Where(issue => issue.Kind == SeriesKind.Quarterly)
                .Select(issue => new SeriesItemViewModel(issue))
                .ToArray();
            Filter();
        }
    }

    partial void OnSearchTextChanged(string value) => Filter();

    private void Filter()
    {
        var term = SearchText?.Trim();
        EmptyMessage = string.IsNullOrEmpty(term)
            ? "Keine Quartalshefte im Archiv verfügbar."
            : "Keine passenden Quartalshefte gefunden.";
        Items = string.IsNullOrEmpty(term)
            ? _all
            : _all.Where(item => Matches(item, term)).ToArray();
    }

    private static bool Matches(SeriesItemViewModel item, string term) =>
        item.Title.Contains(term, StringComparison.CurrentCultureIgnoreCase)
        || item.Subtitle?.Contains(term, StringComparison.CurrentCultureIgnoreCase) == true;
}
