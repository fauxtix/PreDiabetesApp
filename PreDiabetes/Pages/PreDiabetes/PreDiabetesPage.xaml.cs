using PreDiabetes.ViewModels;

namespace PreDiabetes.Pages;

public partial class PreDiabetesPage : ContentPage
{
    public PreDiabetesPage(PreDiabetesViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
