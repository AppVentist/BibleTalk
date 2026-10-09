using BibleTalk.Services;
using BibleTalk.ViewModels;

namespace BibleTalk.Views
{
    public partial class TopicPage : ContentPage
    {
        private readonly TopicViewModel _viewModel;

        // Solange false, werden Positionswechsel der CarouselView ignoriert.
        // Nötig, weil die CarouselView beim Wiederanzeigen der Seite
        // ungefragt auf Position 0 zurückspringt.
        private bool _acceptPositionChanges;

        public TopicPage(TopicViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            RestorePosition();
        }

        protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
        {
            _acceptPositionChanges = false;
            base.OnNavigatedFrom(args);
        }

        // Stellt das zuletzt angezeigte Thema wieder her
        private void RestorePosition()
        {
            _acceptPositionChanges = false;

            if (_viewModel.Topics.Count == 0)
                return;

            var index = Math.Clamp(_viewModel.SelectedIndex, 0, _viewModel.Topics.Count - 1);

            // Erst nach dem Layout-Durchlauf setzen, sonst überschreibt
            // die CarouselView den Wert wieder mit 0
            Dispatcher.Dispatch(() =>
            {
                if (TopicsCarousel.Position != index)
                    TopicsCarousel.Position = index;

                _viewModel.CurrentTopic = _viewModel.Topics[index];
                _acceptPositionChanges = true;
            });
        }

        private void OnTopicPositionChanged(object? sender, PositionChangedEventArgs e)
        {
            if (_acceptPositionChanges)
                _viewModel.SelectedIndex = e.CurrentPosition;
        }

        // Zurück-Button im Header. Bewusst als Code-behind-Handler:
        // In einem DataTemplate schluckt AsyncRelayCommand Ausnahmen
        // aus GoToAsync stillschweigend – der Button wirkt dann tot.
        private async void OnBackClicked(object? sender, EventArgs e)
        {
            try
            {
                if (Shell.Current is not null)
                {
                    await Shell.Current.GoToAsync("..");
                    return;
                }

                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Zurück-Navigation fehlgeschlagen: {ex}");

                // Fallback über den klassischen Navigation-Stack
                try
                {
                    await Navigation.PopAsync();
                }
                catch (Exception fallbackEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Fallback fehlgeschlagen: {fallbackEx}");
                }
            }
        }

        // Share-Button im Header
        private async void OnShareClicked(object? sender, EventArgs e)
        {
            await AppLinks.ShareAppAsync();
        }
    }
}
