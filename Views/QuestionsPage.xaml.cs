using BibleTalk.ViewModels;

namespace BibleTalk.Views
{
    public partial class QuestionsPage : ContentPage
    {
        private readonly QuestionsViewModel _viewModel;

        public QuestionsPage(QuestionsViewModel viewModel)
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
