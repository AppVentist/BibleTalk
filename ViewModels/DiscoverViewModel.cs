using System.Collections.ObjectModel;
using BibleTalk.Models;
using CommunityToolkit.Mvvm.ComponentModel;
namespace BibleTalk.ViewModels;
public partial class DiscoverViewModel : ObservableObject, IQueryAttributable
{
    private readonly List<ConversationItemViewModel> _all = [];
    public ObservableCollection<ConversationItemViewModel> Items { get; } = [];
    public IReadOnlyList<string> Categories { get; } = ["Alle Themen", "Alltag", "Glaubensfragen", "Bibel entdecken", "Standardausgabe"];
    [ObservableProperty] private string searchText = "";
    [ObservableProperty] private string selectedCategory = "Alle Themen";
    [ObservableProperty] private bool isEmpty;
    partial void OnSearchTextChanged(string value) => Filter();
    partial void OnSelectedCategoryChanged(string value) => Filter();
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("SeriesList", out var value) || value is not Series[] series) return;
        _all.Clear();
        foreach (var entry in series) foreach (var topic in entry.Topics.Where(t => t.HasContent)) _all.Add(new(entry, topic));
        Filter();
    }
    private void Filter()
    {
        Items.Clear();
        foreach (var item in _all.Where(x => (SelectedCategory == "Alle Themen" || x.Category == SelectedCategory)
            && $"{x.Title} {x.Details} {x.Category}".Contains(SearchText?.Trim() ?? "", StringComparison.CurrentCultureIgnoreCase))) Items.Add(item);
        IsEmpty = Items.Count == 0;
    }
}
