using BibleTalk.Services;
using BibleTalk.ViewModels;

namespace BibleTalk.Views
{
    public partial class SeriesPage : ContentPage
    {
        public SeriesPage(SeriesViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        // Zurück-Button im Header. Bewusst als Code-behind-Handler:
        // AsyncRelayCommand schluckt Ausnahmen aus GoToAsync stillschweigend –
        // der Button wirkt dann tot.
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
