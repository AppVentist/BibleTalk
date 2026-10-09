using BibleTalk.ViewModels;

namespace BibleTalk.Views
{
    public partial class LessonPage : ContentPage
    {
        private readonly LessonViewModel _viewModel;

        public LessonPage(LessonViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Letzte Zeichen sichern, bevor die Seite verschwindet
            _viewModel.FlushNotes();
        }
    }
}
