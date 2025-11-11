using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Globalization;

namespace PreDiabetes.ViewModels;

public partial class PreDiabetesIntroViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;

    public PreDiabetesIntroViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;

        var current = CultureInfo.CurrentUICulture;
        var culture = new CultureInfo(current.TwoLetterISOLanguageName);

        if (culture.TwoLetterISOLanguageName.Equals("en", StringComparison.OrdinalIgnoreCase))
        {
            Title = "Introduction";
            Items = new ObservableCollection<string>
            {
                "Age group — <35, 35-44, 45-54, 55-64, 65+",
                "Smoking habit",
                "Blood pressure medication",
                "Family history of diabetes",
                "Physical activity (≥30 min/day)",
                "Vegetable/fruit consumption",
                "Waist circumference (men/women)"
            };
        }
        else 
        {
            Title = "Introdução";
            Items = new ObservableCollection<string>
            {
                "Grupo de idades — Menos de 35, 35-44, 45-54, 55-64, 65+",
                "Hábito de fumar",
                "Medicação para a tensão arterial",
                "Parentes com diabetes",
                "Atividade física (≥30 min/dia)",
                "Consumo de vegetais/fruta",
                "Circunferência da cintura (homens/mulheres)"
            };
        }
    }

    public string Title { get; }
    public ObservableCollection<string> Items { get; }

    [RelayCommand]
    private async Task StartSimulationAsync()
    {
        await _navigationService.NavigateToAsync<PreDiabetesPage>();
    }
}
