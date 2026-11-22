using BibleTalk.ViewModels;

namespace BibleTalk.Views;

public partial class ArchivePage : ContentPage
{
    public ArchivePage(ArchiveViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
