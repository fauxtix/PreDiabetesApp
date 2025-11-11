using System.Globalization;

namespace PreDiabetes.Converters
{
    public sealed class NullOrEmptyToBoolConverter : IValueConverter
    {
        public static readonly NullOrEmptyToBoolConverter Instance = new NullOrEmptyToBoolConverter();

        private NullOrEmptyToBoolConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
                return !string.IsNullOrWhiteSpace(s);
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
