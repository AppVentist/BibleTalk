namespace BibleTalk.Services
{
    // Einzige Quelle für Store-Links, Feedback-Adresse und Share-Texte.
    // Vorher lagen dieselben URLs in AppShell, FeedbackPage, TopicPage und
    // SeriesPage verstreut – beim Veröffentlichen musste man sie in
    // vier Dateien nachziehen.
    public static class AppLinks
    {
        // ---- Beim Veröffentlichen anpassen -------------------------------
        public const string AndroidPackageId = "de.appventist.bibletalk";
        public const string IosAppId = "idDEINEAPPID";
        public const string FeedbackAddress = "viktor.ott@adventisten.de";
        // ------------------------------------------------------------------

        public const string FeedbackSubject = "Feedback zur BibleTalk-App";

        private const string ShareTitle = "BibleTalk teilen";

        private const string ShareText =
            "Entdecke die Bibel durch gewinnbringende Fragen zur Bibel und deren Anwendung!";

        public static string AndroidStoreUrl =>
            $"https://play.google.com/store/apps/details?id={AndroidPackageId}";

        public static string IosStoreUrl =>
            $"https://apps.apple.com/app/{IosAppId}";

        // Auf iOS der App Store, sonst Google Play
        public static string StoreUrl =>
            DeviceInfo.Platform == DevicePlatform.iOS ? IosStoreUrl : AndroidStoreUrl;

        // Fallback, wenn kein E-Mail-Client als Compose-Ziel registriert ist
        public static string FeedbackMailtoUrl =>
            $"mailto:{FeedbackAddress}?subject={Uri.EscapeDataString(FeedbackSubject)}";

        // Teilt den Store-Link. Fängt selbst ab, wenn kein Share-Ziel
        // existiert (typisch auf Emulatoren) – der Aufrufer braucht
        // deshalb kein eigenes try/catch.
        public static async Task ShareAppAsync()
        {
            try
            {
                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Title = ShareTitle,
                    Text = ShareText,
                    Uri = StoreUrl
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Teilen fehlgeschlagen: {ex}");
            }
        }

        // Öffnet den E-Mail-Client. Ausnahmen werden bewusst NICHT
        // geschluckt, damit die FeedbackPage einen Hinweis anzeigen kann.
        public static async Task ComposeFeedbackAsync()
        {
            if (Email.Default.IsComposeSupported)
            {
                await Email.Default.ComposeAsync(new EmailMessage
                {
                    Subject = FeedbackSubject,
                    To = [FeedbackAddress]
                });
                return;
            }

            await Launcher.Default.OpenAsync(FeedbackMailtoUrl);
        }
    }
}
