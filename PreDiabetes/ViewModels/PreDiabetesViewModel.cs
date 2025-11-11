using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PreDiabetes.Models;
using PreDiabetes.Pages.PreDiabetes;
using System.Globalization;

namespace PreDiabetes.ViewModels;

public partial class PreDiabetesViewModel : ObservableObject
{
    private readonly IPreDiabetesCalculatorService _calculator;

    public PreDiabetesViewModel(IPreDiabetesCalculatorService calculator)
    {
        _calculator = calculator;
        AgeIndex = 0;
        GenderIndex = 0;
        ManWaistIndex = 0;
        WomanWaistIndex = 0;
        VegetaisTodosOsDias = true;
        AtividadeFisica = true;
    }

    [ObservableProperty] private int ageIndex;
    [ObservableProperty] private int genderIndex;
    [ObservableProperty] private int manWaistIndex;
    [ObservableProperty] private int womanWaistIndex;
    [ObservableProperty] private bool isSmoker;
    [ObservableProperty] private bool hipertensaoMedication;
    [ObservableProperty] private bool glucoseInTests;
    [ObservableProperty] private bool parenteDiabetes;
    [ObservableProperty] private bool atividadeFisica;
    [ObservableProperty] private bool vegetaisTodosOsDias;
    [ObservableProperty] private string weightKgText = string.Empty;
    [ObservableProperty] private string heightCmText = string.Empty;

    [ObservableProperty] private string bmiText = string.Empty;
    [ObservableProperty] private bool isBmiVisible;
    [ObservableProperty] private string bmiCategory = string.Empty;
    [ObservableProperty] private Color bmiColor = Colors.Transparent;

    [ObservableProperty] private string validationMessage = string.Empty;
    [ObservableProperty] private bool hasValidationMessage;

    public bool ShowManWaist => GenderIndex == 1;
    public bool ShowWomanWaist => GenderIndex == 0;

    partial void OnGenderIndexChanged(int value)
    {
        OnPropertyChanged(nameof(ShowManWaist));
        OnPropertyChanged(nameof(ShowWomanWaist));
    }

    partial void OnWeightKgTextChanged(string value) => ValidateWeightAndHeight();
    partial void OnHeightCmTextChanged(string value) => ValidateWeightAndHeight();

    void ValidateWeightAndHeight()
    {
        ValidationMessage = string.Empty;
        HasValidationMessage = false;
        BmiText = string.Empty;
        IsBmiVisible = false;
        BmiCategory = string.Empty;

        if (string.IsNullOrWhiteSpace(WeightKgText) && string.IsNullOrWhiteSpace(HeightCmText))
        {
            CalculateCommand.NotifyCanExecuteChanged();
            return;
        }

        var culture = CultureInfo.CurrentCulture;
        double weight = 0, height = 0;
        if (!string.IsNullOrWhiteSpace(WeightKgText) && !double.TryParse(WeightKgText.Trim(), NumberStyles.Number, culture, out weight))
        {
            ValidationMessage = "Peso inválido. Use apenas números (ex.: 75).";
        }
        else if (!string.IsNullOrWhiteSpace(HeightCmText) && !double.TryParse(HeightCmText.Trim(), NumberStyles.Number, culture, out height))
        {
            ValidationMessage = "Altura inválida. Use apenas números (ex.: 175).";
        }
        else
        {
            if (weight > 0 && (weight < 30 || weight > 300))
                ValidationMessage = "Peso fora dos valores expectáveis (30–300 kg).";
            if (height > 0 && (height < 100 || height > 250))
                ValidationMessage = string.IsNullOrEmpty(ValidationMessage)
                    ? "Altura fora dos valores expectáveis (100–250 cm)."
                    : ValidationMessage + " Também verifique a altura (100–250 cm).";

            if (string.IsNullOrWhiteSpace(ValidationMessage) && weight > 0 && height > 0)
            {
                var h = height / 100.0;
                var bmi = weight / (h * h);
                BmiText = bmi.ToString("0.0", CultureInfo.CurrentCulture);
                IsBmiVisible = true;

                BmiCategory = bmi switch
                {
                    < 16.0 => "Muito Baixo",
                    < 18.5 => "Baixo",
                    < 25.0 => "Normal",
                    < 30.0 => "Alto",
                    _ => "Muito Alto"
                };

                BmiColor = BmiCategory switch
                {
                    "Muito Baixo" => Color.FromArgb("#5D6D7E"),
                    "Baixo" => Color.FromArgb("#F39C12"),
                    "Normal" => Color.FromArgb("#27AE60"),
                    "Alto" => Color.FromArgb("#F1C40F"),
                    "Muito Alto" => Color.FromArgb("#E74C3C"),
                    _ => Colors.Transparent
                };
            }
            else
            {
                BmiText = string.Empty;
                IsBmiVisible = false;
                BmiCategory = string.Empty;
                BmiColor = Colors.Transparent;
            }
        }

        HasValidationMessage = !string.IsNullOrEmpty(ValidationMessage);

        CalculateCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(BmiText));
        OnPropertyChanged(nameof(IsBmiVisible));
        OnPropertyChanged(nameof(BmiCategory));
    }

    private bool CanCalculate()
    {
        return !HasValidationMessage;
    }

    [RelayCommand(CanExecute = nameof(CanCalculate))]
    private Task Calculate()
    {
        double weightKg = 0;
        double heightCm = 0;
        if (!string.IsNullOrWhiteSpace(WeightKgText))
            double.TryParse(WeightKgText.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out weightKg);
        if (!string.IsNullOrWhiteSpace(HeightCmText))
            double.TryParse(HeightCmText.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out heightCm);

        var input = new PreDiabetesInput
        {
            AgeIndex = AgeIndex,
            GenderIndex = GenderIndex,
            ManWaistIndex = ManWaistIndex,
            WomanWaistIndex = WomanWaistIndex,
            IsSmoker = IsSmoker,
            HipertensaoMedication = HipertensaoMedication,
            GlucoseInTests = GlucoseInTests,
            ParenteDiabetes = ParenteDiabetes,
            AtividadeFisica = AtividadeFisica,
            VegetaisTodosOsDias = VegetaisTodosOsDias,
            WeightKg = weightKg,
            HeightCm = heightCm
        };

        var result = _calculator.Calculate(input);

        var popup = new ResultPopup(result.Points, result.RiskFactor ??
            string.Empty, result.Message ??
            string.Empty,
            BmiText, BmiCategory);

        var host = Shell.Current as Page;
        if (host == null && Application.Current?.Windows?.Count > 0)
            host = Application.Current.Windows[0].Page;

        host?.ShowPopup(popup);

        return Task.CompletedTask;
    }
}
