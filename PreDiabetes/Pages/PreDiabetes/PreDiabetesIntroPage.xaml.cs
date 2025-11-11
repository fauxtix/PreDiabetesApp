using PreDiabetes.ViewModels;

namespace PreDiabetes.Pages;

public partial class PreDiabetesIntroPage : ContentPage
{
    public PreDiabetesIntroPage(PreDiabetesIntroViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
    private async void SimulationBtn_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(PreDiabetesPage), true);
    }
}