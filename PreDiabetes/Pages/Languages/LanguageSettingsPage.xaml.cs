using PreDiabetes.ViewModels.Languages;

namespace PreDiabetes.Pages.Languages;


public partial class LanguageSettingsPage : ContentPage
{
    public LanguageSettingsPage(LanguageSettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}