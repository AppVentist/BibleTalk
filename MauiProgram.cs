using BibleTalk.Services;
using BibleTalk.ViewModels;
using BibleTalk.Views;
using Microsoft.Extensions.Logging;

namespace BibleTalk
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Services registrieren
            builder.Services.AddSingleton(new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(5)
            });

            builder.Services.AddSingleton<IContentService, ContentService>();

            // Singleton, damit geladene Bibelbücher app-weit im Cache bleiben
            builder.Services.AddSingleton<IBibleService, BibleService>();

            // Loest "passages" in Bibeltext auf - geteilt von QuestionsPage
            // und LessonPage, damit der Cache nicht zweimal existiert.
            builder.Services.AddSingleton<IBibleTextResolver, BibleTextResolver>();

            // Notizen der Juengerschaft (Preferences)
            builder.Services.AddSingleton<INoteService, NoteService>();

            // Erscheinungsbild Hell/Dunkel/System (Preferences)
            builder.Services.AddSingleton<IThemeService, ThemeService>();

            // MVVM: Views und ViewModels registrieren
            //builder.Services.AddTransient<FeedbackPage>();
            //builder.Services.AddTransient<FeedbackViewModel>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<DiscoverPage>();
            builder.Services.AddTransient<DiscoverViewModel>();
            builder.Services.AddTransient<SeriesPage>();
            builder.Services.AddTransient<SeriesViewModel>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<TopicPage>();
            builder.Services.AddTransient<TopicViewModel>();
            builder.Services.AddTransient<QuestionsPage>();
            builder.Services.AddTransient<QuestionsViewModel>();
            builder.Services.AddTransient<LessonPage>();
            builder.Services.AddTransient<LessonViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

#if ANDROID

#endif

