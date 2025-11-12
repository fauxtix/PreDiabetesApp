using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PreDiabetes.Resources.Languages;
using System.Globalization;

namespace PreDiabetes.ViewModels;

public partial class ResultPopupViewModel : ObservableObject
{
    readonly string _shareText;

    public event Action? RequestClose;

    public string PartilharButtonText => $"🔗 {AppResources.TituloPartilhar}";
    public string CopiarButtonText => $"📋 {AppResources.TituloCopiar}";
    public string FecharButtonText => $"✖ {AppResources.TituloFechar}";

    [ObservableProperty] int points;
    [ObservableProperty] string pointsText = string.Empty;
    [ObservableProperty] string riskText = string.Empty;
    [ObservableProperty] string message = string.Empty;
    [ObservableProperty] bool isInsufficient;
    [ObservableProperty] bool copyEnabled;
    [ObservableProperty] string iconText = "📊";
    [ObservableProperty] string riskBadgeHex = "#00000000";
    [ObservableProperty] string iconCircleHex = "#00000000";
    [ObservableProperty] string statusText = string.Empty;
    [ObservableProperty] bool isStatusVisible;

    [ObservableProperty] string bmiColorHex = "#00000000";
    [ObservableProperty] Color bmiColor = Colors.Transparent;
    [ObservableProperty] string bmiIcon = string.Empty;

    [ObservableProperty] string bmiText = string.Empty;
    [ObservableProperty] string bmiCategory = string.Empty;

    public ResultPopupViewModel(int points, string? riskFactor, string? message, string? bmiText = null, string? bmiCategory = null)
    {
        Points = points;
        PointsText = $"{AppResources.TituloPontos}: {points}";

        // BMI props
        BmiText = bmiText ?? string.Empty;
        BmiCategory = bmiCategory ?? string.Empty;
        (BmiColorHex, BmiIcon) = MapBmiCategoryToVisuals(BmiCategory);

        BmiColor = !string.IsNullOrWhiteSpace(BmiColorHex)
            ? Color.FromArgb(BmiColorHex)
            : Colors.Transparent;

        if (points <= 0)
        {
            IsInsufficient = true;
            RiskText = (CultureInfo.CurrentUICulture?.TwoLetterISOLanguageName == "en")
                ? "Insufficient data"
                : "Dados insuficientes";

            Message = (CultureInfo.CurrentUICulture?.TwoLetterISOLanguageName == "en")
                ? "Insufficient data to calculate risk. Verify inputs and try again."
                : "Dados insuficientes para calcular o risco. Verifique as entradas e tente novamente.";

            CopyEnabled = false;
            RiskBadgeHex = "#95A5A6";
            IconText = "❔";
            IconCircleHex = "#F0F0F0";
        }
        else
        {
            IsInsufficient = false;
            RiskText = riskFactor ?? string.Empty;
            Message = message ?? string.Empty;
            CopyEnabled = true;

            var r = (riskFactor ?? string.Empty).Trim().ToLowerInvariant();

            // Match Portuguese or English synonyms
            if (r.Contains("alto") || r.Contains("high") || r.Contains("elevado") || r.Contains("muito alto") || r.Contains("very high"))
            {
                RiskBadgeHex = "#E74C3C";
                IconText = "⚠️";
                IconCircleHex = "#FFF1F0";
            }
            else if (r.Contains("moderado") || r.Contains("medium") || r.Contains("moderate"))
            {
                RiskBadgeHex = "#F39C12";
                IconText = "ℹ️";
                IconCircleHex = "#FFF8ED";
            }
            else if (r.Contains("baixo") || r.Contains("low") || r.Contains("leve") || r.Contains("slight"))
            {
                RiskBadgeHex = "#27AE60";
                IconText = "✅";
                IconCircleHex = "#EEF9F1";
            }
            else
            {
                // Slightly elevated (en/pt) or any other value
                RiskBadgeHex = "#F39C12";
                IconText = "ℹ️";
                IconCircleHex = "#FFF8ED";
            }
        }

        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"{AppResources.TituloPontos}: {Points}");
        sb.AppendLine($"{AppResources.TituloRisco}: {RiskText}");
        if (!string.IsNullOrWhiteSpace(BmiText))
            sb.AppendLine($"\n{AppResources.TituloIMC}: {BmiText} ({BmiCategory})");
        sb.AppendLine();
        sb.AppendLine(Message ?? string.Empty);

        _shareText = sb.ToString();

        StatusText = string.Empty;
        IsStatusVisible = false;
    }
    static (string Hex, string Icon) MapBmiCategoryToVisuals(string category)
    {
        return (category ?? string.Empty).Trim() switch
        {
            // Portuguese
            "Muito Baixo" or "Very Low" => ("#5D6D7E", "❗"),
            "Baixo" or "Low" => ("#F39C12", "ℹ️"),
            "Normal" => ("#27AE60", "✅"),
            "Alto" or "High" => ("#F1C40F", "⚠️"),
            "Muito Alto" or "Very High" => ("#E74C3C", "⛔"),
            // Moderate (English and Portuguese)
            "Moderado" or "Moderate" => ("#F39C12", "ℹ️"),
            // Slightly elevated
            "Ligeiramente elevado" or "Slightly elevated" => ("#F39C12", "ℹ️"),
            _ => ("#00000000", string.Empty)
        };
    }

    [RelayCommand]
    async Task Copy()
    {
        if (!CopyEnabled) return;

        try
        {
            await Clipboard.SetTextAsync(_shareText);
            await ShowTransientStatusAsync(AppResources.TituloCopiadoClipboard);
        }
        catch
        {
            await ShowTransientStatusAsync(AppResources.TituloFalhaAoCopiar);
        }
    }

    [RelayCommand]
    void Close()
    {
        RequestClose?.Invoke();
    }

    [RelayCommand]
    async Task Partilhar()
    {
        if (!CopyEnabled) return;

        try
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = _shareText,
                Title = AppResources.TituloResultado
            });
            await ShowTransientStatusAsync(AppResources.TituloPartilhado);
        }
        catch
        {
            await ShowTransientStatusAsync(AppResources.TituloFalhaAoPartilhar);
        }
    }

    async Task ShowTransientStatusAsync(string text)
    {
        StatusText = text;
        IsStatusVisible = true;
        await Task.Delay(1400);
        IsStatusVisible = false;
        StatusText = string.Empty;
    }
}
