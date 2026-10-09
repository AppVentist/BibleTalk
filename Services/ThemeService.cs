namespace BibleTalk.Services
{
    // Erscheinungsbild der App (Hell / Dunkel / wie System).
    // Die Auswahl liegt in Preferences, damit sie einen Neustart überlebt.
    public interface IThemeService
    {
        // "System", "Light" oder "Dark"
        string Current { get; }

        void Apply(string theme);

        // Beim App-Start aufrufen, um die gespeicherte Auswahl zu setzen.
        void ApplySaved();
    }

    public class ThemeService : IThemeService
    {
        public const string System = "System";
        public const string Light = "Light";
        public const string Dark = "Dark";

        private const string PreferenceKey = "app_theme";

        public string Current =>
            Preferences.Default.Get(PreferenceKey, System);

        public void Apply(string theme)
        {
            Preferences.Default.Set(PreferenceKey, theme);
            SetUserAppTheme(theme);
        }

        public void ApplySaved() => SetUserAppTheme(Current);

        private static void SetUserAppTheme(string theme)
        {
            if (Application.Current is null)
                return;

            // Unspecified = dem Betriebssystem folgen
            Application.Current.UserAppTheme = theme switch
            {
                Light => AppTheme.Light,
                Dark => AppTheme.Dark,
                _ => AppTheme.Unspecified
            };
        }
    }
}
