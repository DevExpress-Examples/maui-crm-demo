using System.Globalization;
using CrmDemo.DataModel.Models;

namespace CrmDemo.Helpers;

public class InverseBoolConverter : IValueConverter, IMarkupExtension {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return !(bool)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        return !(bool)value;
    }

    public object ProvideValue(IServiceProvider serviceProvider) {
        return this;
    }
}

public class StateToColorConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        AppTheme currentTheme = Application.Current.RequestedTheme;
        if (value is null) {
            return null;
        }
        OrderState state = (OrderState)value;
        var alpha = (float)parameter;
        Color res;
        if (currentTheme is AppTheme.Light) {
            res = state switch {
                OrderState.Pending => Color.FromArgb("#60BA07"),
                OrderState.Shipping => Color.FromArgb("#55ABDC"),
                OrderState.Paid => Color.FromArgb("#3CAB76"),
                OrderState.Processed => Color.FromArgb("#D46E60"),
                _ => Color.FromArgb("#60BA07")
            };
        } else {
            res = state switch {
                OrderState.Pending => Color.FromArgb("#A0CD73"),
                OrderState.Shipping => Color.FromArgb("#8FC3E0"),
                OrderState.Paid => Color.FromArgb("#67C596"),
                OrderState.Processed => Color.FromArgb("#D88074"),
                _ => Color.FromArgb("#A0CD73")
            };
        }

        return res.WithAlpha(alpha);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

class TasksConverter : IMultiValueConverter {
    
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
        if (values is null || values.Length < 2) {
            return "Tasks not found";
        }
        string localizedString = "of";

        return $"{values[0]} {localizedString} {values[1]}";
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}