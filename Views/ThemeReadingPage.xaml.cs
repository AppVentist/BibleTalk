using BibleTalk.ViewModels;

namespace BibleTalk.Views;

public partial class ThemeReadingPage : ContentPage
{
    private readonly QuestionsViewModel _viewModel;

    public ThemeReadingPage(QuestionsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.FlushNotes();
    }
}
