using BibleTalk.ViewModels;
namespace BibleTalk.Views;
public partial class DiscoverPage : ContentPage
{
    public DiscoverPage(DiscoverViewModel viewModel) { InitializeComponent(); BindingContext = viewModel; }
}
