using BibleTalk.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BibleTalk.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly IThemeService _theme;

        // Gebunden an RadioButtonGroup.SelectedValue - Werte entsprechen
        // den Value-Attributen der RadioButtons ("System", "Light", "Dark").
        [ObservableProperty]
        private string selectedTheme;

        public SettingsViewModel(IThemeService theme)
        {
            _theme = theme;
            selectedTheme = theme.Current;
        }

        // Sofort anwenden, kein "Speichern"-Button nötig.
        partial void OnSelectedThemeChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
                _theme.Apply(value);
        }
    }
}
