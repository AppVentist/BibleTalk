using BibleTalk.Services;
using BibleTalk.Views;

namespace BibleTalk
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Routen für die Navigation registrieren
            Routing.RegisterRoute(nameof(SeriesPage), typeof(SeriesPage));
            Routing.RegisterRoute(nameof(TopicPage), typeof(TopicPage));
            Routing.RegisterRoute(nameof(QuestionsPage), typeof(QuestionsPage));
            Routing.RegisterRoute(nameof(LessonPage), typeof(LessonPage));
            Routing.RegisterRoute(nameof(DiscoverPage), typeof(DiscoverPage));
        }

        // Tippen auf das Logo: zurück zur Heft-Übersicht (MainPage als Wurzel)
        private async void OnLogoTapped(object sender, TappedEventArgs e)
        {
            await Current.GoToAsync("//MainPage");
        }

        // Event Handler ruft die Share-Funktion auf, um die App zu teilen
        private async void OnShareClicked(object sender, EventArgs e)
        {
            await AppLinks.ShareAppAsync();
        }

        // Event Handler ruft Email auf, um Feedback zu geben
        private async void OnFeedbackClicked(object sender, EventArgs e)
        {
            try
            {
                await AppLinks.ComposeFeedbackAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"E-Mail konnte nicht geöffnet werden: {ex}");
            }
        }
    }
}
