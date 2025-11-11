using System.Globalization;

namespace PreDiabetes.Converters;

public class BmiCategoryToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var category = (value as string)?.Trim() ?? string.Empty;

        // Return Color (not Brush) so it can be used with Label.TextColor
        return category switch
        {
            "Muito Baixo" => Color.FromArgb("#5D6D7E"),
            "Baixo" => Color.FromArgb("#F39C12"),
            "Normal" => Color.FromArgb("#27AE60"),
            "Alto" => Color.FromArgb("#F1C40F"),
            "Muito Alto" => Color.FromArgb("#E74C3C"),
            _ => Colors.Transparent
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
