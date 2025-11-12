using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PreDiabetes.Models;
using PreDiabetes.Pages.PreDiabetes;
using PreDiabetes.Resources.Languages;
using System.Collections.ObjectModel;
using System.Globalization;

namespace PreDiabetes.ViewModels;

public partial class PreDiabetesViewModel : ObservableObject
{
    private readonly IPreDiabetesCalculatorService _calculator;

    [ObservableProperty] private ObservableCollection<string> ageGroups = new();
    [ObservableProperty] private ObservableCollection<string> genderOptions = new();
    [ObservableProperty] private ObservableCollection<string> manWaistOptions = new();
    [ObservableProperty] private ObservableCollection<string> womanWaistOptions = new();

    [ObservableProperty] private string selectedAgeGroup;
    [ObservableProperty] private string selectedGender;
    [ObservableProperty] private string selectedManWaist;
    [ObservableProperty] private string selectedWomanWaist;

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

    [ObservableProperty] private string vegetaisOptionYes = "Todos os dias";
    [ObservableProperty] private string vegetaisOptionNo = "Por vezes";

    public PreDiabetesViewModel(IPreDiabetesCalculatorService calculator)
    {
        _calculator = calculator;
        LoadAllLocalizedLists();
    }

    void LoadAllLocalizedLists()
    {
        var twoLetter = CultureInfo.CurrentUICulture?.TwoLetterISOLanguageName
            ?? CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

        CultureInfo culture;
        try { culture = new CultureInfo(twoLetter); }
        catch { culture = CultureInfo.CurrentUICulture ?? CultureInfo.CurrentCulture; }

        string? TryGet(string key)
        {
            try { return AppResources.ResourceManager.GetString(key, culture); }
            catch { return null; }
        }

        var female = AppResources.ResourceManager.GetString("TituloFeminino", culture) ?? AppResources.TituloFeminino ?? "Feminino";
        var male = AppResources.ResourceManager.GetString("TituloMasculino", culture) ?? AppResources.TituloMasculino ?? "Masculino";
        GenderOptions.Clear();
        GenderOptions.Add(female);
        GenderOptions.Add(male);
        SelectedGender = GenderOptions.Count > 0 ? GenderOptions[0] : null;

        var fallbackPt_Ages = new[] { "Menos de 35 anos", "35-44 anos", "45-54 anos", "55-64 anos", "65 anos ou mais" };
        var fallbackEn_Ages = new[] { "Under 35 years", "35-44 years", "45-54 years", "55-64 years", "65 years or older" };
        var rawAges = TryGet("Simulacao_GrupoIdades_Itens");
        var ages = !string.IsNullOrWhiteSpace(rawAges)
            ? rawAges.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray()
            : (twoLetter.Equals("en", System.StringComparison.OrdinalIgnoreCase) ? fallbackEn_Ages : fallbackPt_Ages);

        AgeGroups.Clear();
        foreach (var a in ages) AgeGroups.Add(a);
        SelectedAgeGroup = AgeGroups.Count > 0 ? AgeGroups[0] : null;

        var fallbackPt_ManWaist = new[] { "Menos de 94 cm", "94 - 102 cm", "Mais de 102 cm" };
        var fallbackEn_ManWaist = new[] { "Under 94 cm", "94 - 102 cm", "Over 102 cm" };
        var rawMan = TryGet("Simulacao_AncaHomens_Itens");
        var manWaist = !string.IsNullOrWhiteSpace(rawMan)
            ? rawMan.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray()
            : (twoLetter.Equals("en", System.StringComparison.OrdinalIgnoreCase) ? fallbackEn_ManWaist : fallbackPt_ManWaist);

        ManWaistOptions.Clear();
        foreach (var v in manWaist) ManWaistOptions.Add(v);
        SelectedManWaist = ManWaistOptions.Count > 0 ? ManWaistOptions[0] : null;

        var fallbackPt_WomanWaist = new[] { "Menos de 80 cm", "80 - 88 cm", "Mais de 88 cm" };
        var fallbackEn_WomanWaist = new[] { "Under 80 cm", "80 - 88 cm", "Over 88 cm" };
        var rawWoman = TryGet("Simulacao_AncaMulheres_Itens");
        var womanWaist = !string.IsNullOrWhiteSpace(rawWoman)
            ? rawWoman.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray()
            : (twoLetter.Equals("en", System.StringComparison.OrdinalIgnoreCase) ? fallbackEn_WomanWaist : fallbackPt_WomanWaist);

        WomanWaistOptions.Clear();
        foreach (var v in womanWaist) WomanWaistOptions.Add(v);
        SelectedWomanWaist = WomanWaistOptions.Count > 0 ? WomanWaistOptions[0] : null;

        var vegYes = TryGet("Simulacao_Vegetais_Opcao1");
        var vegNo = TryGet("Simulacao_Vegetais_Opcao2");
        VegetaisOptionYes = !string.IsNullOrWhiteSpace(vegYes)
            ? vegYes
            : (twoLetter.Equals("en", System.StringComparison.OrdinalIgnoreCase) ? "Every day" : "Todos os dias");
        VegetaisOptionNo = !string.IsNullOrWhiteSpace(vegNo)
            ? vegNo
            : (twoLetter.Equals("en", System.StringComparison.OrdinalIgnoreCase) ? "Sometimes" : "Por vezes");
    }

    public bool ShowManWaist => SelectedGender == GenderOptions.LastOrDefault();
    public bool ShowWomanWaist => SelectedGender == GenderOptions.FirstOrDefault();

    partial void OnWeightKgTextChanged(string value) => ValidateWeightAndHeight();
    partial void OnHeightCmTextChanged(string value) => ValidateWeightAndHeight();

    void ValidateWeightAndHeight()
    {
        ValidationMessage = string.Empty;
        HasValidationMessage = false;
        BmiText = string.Empty;
        IsBmiVisible = false;
        BmiCategory = string.Empty;
        BmiColor = Colors.Transparent;

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

                var severity = bmi switch
                {
                    < 16.0 => BmiSeverity.VeryLow,
                    < 18.5 => BmiSeverity.Low,
                    < 25.0 => BmiSeverity.Normal,
                    < 30.0 => BmiSeverity.High,
                    _ => BmiSeverity.VeryHigh
                };

                BmiCategory = GetLocalizedBmiCategory(severity);
                BmiColor = GetBmiColor(severity);
            }
        }

        HasValidationMessage = !string.IsNullOrEmpty(ValidationMessage);
        CalculateCommand.NotifyCanExecuteChanged();
    }

    private enum BmiSeverity { VeryLow, Low, Normal, High, VeryHigh }
    private string GetLocalizedBmiCategory(BmiSeverity severity)
    {
        var key = severity switch
        {
            BmiSeverity.VeryLow => "Bmi_VeryLow",
            BmiSeverity.Low => "Bmi_Low",
            BmiSeverity.Normal => "Bmi_Normal",
            BmiSeverity.High => "Bmi_High",
            BmiSeverity.VeryHigh => "Bmi_VeryHigh",
            _ => "Bmi_Unknown"
        };

        try
        {
            var localized = AppResources.ResourceManager.GetString(key, CultureInfo.CurrentUICulture);
            if (!string.IsNullOrWhiteSpace(localized)) return localized!;
        }
        catch { }

        var twoLetter = (CultureInfo.CurrentUICulture ?? CultureInfo.CurrentCulture).TwoLetterISOLanguageName;
        var en = twoLetter.Equals("en", System.StringComparison.OrdinalIgnoreCase);

        return severity switch
        {
            BmiSeverity.VeryLow => en ? "Very Low" : "Muito Baixo",
            BmiSeverity.Low => en ? "Low" : "Baixo",
            BmiSeverity.Normal => en ? "Normal" : "Normal",
            BmiSeverity.High => en ? "High" : "Alto",
            BmiSeverity.VeryHigh => en ? "Very High" : "Muito Alto",
            _ => en ? "Unknown" : "Desconhecido"
        };
    }
    private Color GetBmiColor(BmiSeverity severity) => severity switch
    {
        BmiSeverity.VeryLow => Color.FromArgb("#5D6D7E"),
        BmiSeverity.Low => Color.FromArgb("#F39C12"),
        BmiSeverity.Normal => Color.FromArgb("#27AE60"),
        BmiSeverity.High => Color.FromArgb("#F1C40F"),
        BmiSeverity.VeryHigh => Color.FromArgb("#E74C3C"),
        _ => Colors.Transparent
    };

    private bool CanCalculate() => !HasValidationMessage;

    [RelayCommand(CanExecute = nameof(CanCalculate))]
    public async Task Calculate()
    {
        int ageIndex = AgeGroups.IndexOf(SelectedAgeGroup);
        int genderIndex = GenderOptions.IndexOf(SelectedGender);
        int manWaistIndex = ManWaistOptions.IndexOf(SelectedManWaist);
        int womanWaistIndex = WomanWaistOptions.IndexOf(SelectedWomanWaist);

        double weightKg = 0, heightCm = 0;
        if (!string.IsNullOrWhiteSpace(WeightKgText))
            double.TryParse(WeightKgText.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out weightKg);
        if (!string.IsNullOrWhiteSpace(HeightCmText))
            double.TryParse(HeightCmText.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out heightCm);

        var input = new PreDiabetesInput
        {
            AgeIndex = ageIndex,
            GenderIndex = genderIndex,
            ManWaistIndex = manWaistIndex,
            WomanWaistIndex = womanWaistIndex,
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

        var popup = new ResultPopup(result.Points, result.RiskFactor ?? string.Empty, result.Message ?? string.Empty, BmiText, BmiCategory);

        var host = Shell.Current as Page;
        if (host == null && Application.Current?.Windows?.Count > 0)
            host = Application.Current.Windows[0].Page;

        host?.ShowPopup(popup);

        await Task.CompletedTask;
    }
}