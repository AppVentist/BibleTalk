using BibleTalk.Models;
using BibleTalk.Views;
using CommunityToolkit.Mvvm.Input;
namespace BibleTalk.ViewModels;

public partial class ConversationItemViewModel
{
    public ConversationItemViewModel(Series series, Topic topic) { Series = series; Topic = topic; }
    public Series Series { get; }
    public Topic Topic { get; }
    public string Title => Topic.Title;
    public string CoverImage => Series.CoverImage;
    public string Category => Topic.Category ?? (Series.Kind switch {
        SeriesKind.Discipleship => "Glaubensfragen", SeriesKind.Homegroup => "Bibel entdecken", _ => "Standardausgabe" });
    public string Details => string.IsNullOrWhiteSpace(Topic.ConversationReference)
        ? $"{Series.Title} · {Series.TopicWord} {Topic.Number}"
        : $"{Topic.ConversationReference} · ca. {Topic.DurationMinutes} Min.";
    public string ResumeDetails => $"Zuletzt geöffnet · {Series.TopicWord} {Topic.Number}";
    public string Symbol => Category switch { "Alltag" => "☀", "Glaubensfragen" => "♡", _ => "◇" };
    [RelayCommand]
    private Task OpenAsync() => Shell.Current.GoToAsync(nameof(TopicPage), new Dictionary<string, object> {
        ["Series"] = Series, ["Topic"] = Topic });
}
