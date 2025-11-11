using System.Globalization;

using System.Reflection;

namespace PreDiabetes.Converters;

public class HexToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string raw || string.IsNullOrWhiteSpace(raw))
            return new SolidColorBrush(Colors.Transparent);

        var s = raw.Trim();

        // support "0xRRGGBB" or "0xAARRGGBB"
        if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            s = "#" + s.Substring(2);

        // try named color from Colors (Red, Blue, etc.)
        var namedColor = TryGetNamedColor(s);
        if (namedColor is not null)
            return new SolidColorBrush(namedColor);

        // remove leading '#' if present for easier handling
        if (s.StartsWith("#"))
            s = s.Substring(1);

        // expand shorthand formats like RGB -> RRGGBB or ARGB -> AARRGGBB
        if (s.Length == 3) // RGB
            s = $"{s[0]}{s[0]}{s[1]}{s[1]}{s[2]}{s[2]}";
        else if (s.Length == 4) // ARGB
            s = $"{s[0]}{s[0]}{s[1]}{s[1]}{s[2]}{s[2]}{s[3]}{s[3]}";

        // now s is expected to be RRGGBB (6) or AARRGGBB (8)
        try
        {
            if (s.Length == 6 || s.Length == 8)
            {
                var withHash = "#" + s;
                var color = Color.FromArgb(withHash);
                return new SolidColorBrush(color);
            }
        }
        catch
        {
            // fallthrough to transparent
        }

        return new SolidColorBrush(Colors.Transparent);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    static Color? TryGetNamedColor(string input)
    {
        // Accept names with or without '#', case-insensitive
        var name = input.TrimStart('#').Trim();
        if (string.IsNullOrEmpty(name))
            return null;

        // Try exact property on Colors (static properties)
        var prop = typeof(Colors).GetProperty(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);
        if (prop?.GetValue(null) is Color c)
            return c;

        return null;
    }
}