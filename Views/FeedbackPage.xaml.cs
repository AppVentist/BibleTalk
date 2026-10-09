using BibleTalk.Services;

namespace BibleTalk.Views;

public partial class FeedbackPage : ContentPage
{
    public FeedbackPage()
    {
        InitializeComponent();
    }

    // Öffnet den E-Mail-Client für Feedback
    private async void OnSendFeedback_Clicked(object sender, EventArgs e)
    {
        try
        {
            await AppLinks.ComposeFeedbackAsync();
        }
        catch (Exception ex)
        {
            // z. B. Emulator ohne E-Mail-App
            await DisplayAlertAsync("Hinweis",
                $"E-Mail-App konnte nicht geöffnet werden: {ex.Message}", "OK");
        }
    }
}
