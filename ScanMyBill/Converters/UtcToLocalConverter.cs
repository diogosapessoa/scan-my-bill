using System.Globalization;

namespace ScanMyBill.Converters
{
    public sealed class UtcToLocalConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                // Garante que o .NET sabe que a data de origem é UTC
                var utcDateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

                // Converte para o horário local do dispositivo
                return utcDateTime.ToLocalTime();
            }

            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
