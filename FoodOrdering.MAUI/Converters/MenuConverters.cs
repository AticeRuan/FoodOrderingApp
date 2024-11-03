using System.Globalization;

namespace FoodOrdering.MAUI.Converters;

public class DictionaryKeyConverter : IValueConverter
    {
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
        if (value is KeyValuePair<string, List<FoodOrdering.Shared.Models.FoodMenuItem>> kvp)
            {
            return kvp.Key;
            }
        return string.Empty;
        }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
        throw new NotImplementedException();
        }
    }

public class DictionaryValueConverter : IValueConverter
    {
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
        if (value is KeyValuePair<string, List<FoodOrdering.Shared.Models.FoodMenuItem>> kvp)
            {
            return kvp.Value;
            }
        return new List<FoodOrdering.Shared.Models.FoodMenuItem>();
        }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
        throw new NotImplementedException();
        }
    }