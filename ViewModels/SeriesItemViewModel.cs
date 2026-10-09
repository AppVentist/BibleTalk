using BibleTalk.Models;
using BibleTalk.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BibleTalk.ViewModels
{
    // Wrapper um Series für die drei Zeilen der MainPage.
    // Bringt seinen Öffnen-Command selbst mit – dadurch braucht das
    // DataTemplate kein RelativeSource-Binding.
    public partial class SeriesItemViewModel : ObservableObject
    {
        public SeriesItemViewModel(Series series) => Series = series;

        public Series Series { get; }

        public string Title => Series.Title;
        public string? Subtitle => Series.Subtitle;
        public string CoverImage => Series.CoverImage;

        [RelayCommand]
        private async Task OpenAsync()
        {
            await Shell.Current.GoToAsync(nameof(SeriesPage),
                new Dictionary<string, object>
                {
                    ["Series"] = Series
                });
        }
    }
}