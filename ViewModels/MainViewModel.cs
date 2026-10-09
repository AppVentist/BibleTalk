using System.Collections.ObjectModel;
using BibleTalk.Models;
using BibleTalk.Services;
using BibleTalk.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
namespace BibleTalk.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IContentService _content;
    private readonly List<Series> _all = [];
    private bool _loaded;
    public MainViewModel(IContentService content) => _content = content;
    public ObservableCollection<SeriesItemViewModel> Quarterlies { get; } = [];
    public ObservableCollection<ConversationItemViewModel> Conversations { get; } = [];
    [ObservableProperty] private bool isLoading;
    [ObservableProperty] private bool isRefreshing;
    [ObservableProperty] private ConversationItemViewModel? resume;
    [ObservableProperty] private string? errorMessage;
    public bool HasResume => Resume is not null;
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
    partial void OnResumeChanged(ConversationItemViewModel? value) => OnPropertyChanged(nameof(HasResume));
    partial void OnErrorMessageChanged(string? value) => OnPropertyChanged(nameof(HasError));
    [RelayCommand] private Task LoadAsync() => FetchAsync(false);
    [RelayCommand] private Task RefreshAsync() => FetchAsync(true);
    private async Task FetchAsync(bool force)
    {
        if (IsLoading || IsRefreshing) return;
        if (_loaded && !force) { UpdateResume(); return; }
        IsRefreshing = force; IsLoading = !force; ErrorMessage = null;
        try
        {
            var groups = await Task.WhenAll(
                _content.GetSeriesAsync("quarterlies/index.json", SeriesKind.Quarterly),
                _content.GetSeriesAsync("conversations/index.json", SeriesKind.Conversation),
                _content.GetSeriesAsync("discipleship/index.json", SeriesKind.Discipleship),
                _content.GetSeriesAsync("homegroups/index.json", SeriesKind.Homegroup));
            _all.Clear(); _all.AddRange(groups.SelectMany(x => x).Where(x => x.Topics.Count > 0));
            Quarterlies.Clear();
            foreach (var series in _all.Where(x => x.Kind == SeriesKind.Quarterly)) Quarterlies.Add(new(series));
            Conversations.Clear();
            foreach (var series in _all.Where(x => x.Kind == SeriesKind.Conversation))
                foreach (var topic in series.Topics.Where(x => x.HasContent).Take(3 - Conversations.Count))
                    Conversations.Add(new(series, topic));
            _loaded = true; UpdateResume();
            if (Quarterlies.Count == 0 && Conversations.Count == 0) ErrorMessage = "Inhalte konnten nicht geladen werden. Bitte erneut versuchen.";
        }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex); ErrorMessage = "Inhalte konnten nicht geladen werden. Bitte erneut versuchen."; }
        finally { IsLoading = false; IsRefreshing = false; }
    }
    private void UpdateResume()
    {
        var last = ReadingHistory.Last;
        var series = _all.FirstOrDefault(x => x.Id == last.Id && x.Kind == last.Kind);
        var topic = series?.Topics.FirstOrDefault(x => x.Number == last.Number);
        Resume = series is not null && topic is not null ? new(series, topic) : null;
    }
    [RelayCommand]
    private Task DiscoverAsync() => Shell.Current.GoToAsync(nameof(DiscoverPage), new Dictionary<string, object> { ["SeriesList"] = _all.ToArray() });
}
