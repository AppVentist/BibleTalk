using BibleTalk.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BibleTalk
{
    public partial class App : Application
    {
        public App(IThemeService themeService)
        {
            InitializeComponent();

            // Gespeichertes Erscheinungsbild (Hell/Dunkel/System) setzen,
            // bevor die erste Seite gezeichnet wird.
            themeService.ApplySaved();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}