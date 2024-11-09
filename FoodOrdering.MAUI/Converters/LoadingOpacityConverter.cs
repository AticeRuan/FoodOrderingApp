using System.Globalization;

namespace FoodOrdering.MAUI.Converters;
public class LoadingOpacityConverter : IValueConverter
    {
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
        if (value is bool isLoading)
            {
            return isLoading ? 0.6 : 1.0;
            }
        return 1.0;
        }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
        throw new NotImplementedException();
        }
    }
