using System.Globalization;

using DevExpress.CodeParser;
using DevExpress.Maui.Charts;

namespace CrmDemo.Views.Dashboards;

public partial class OrderStateEvolutionPage : ContentPage {
    public OrderStateEvolutionPage() {
        InitializeComponent();
    }
}

public class StackedBarColorConverter : IValueConverter {
    private Color pendingColor = Color.FromArgb("#6085BE");
    private Color shippingColor = Color.FromArgb("#E1AA58");
    private Color paidColor = Color.FromArgb("#CA5252");
    private Color processedColor = Color.FromArgb("#6AAC78");

    public StackedBarColorConverter() {
    }
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        object result = Colors.Green;
        SeriesTemplateData data = value as SeriesTemplateData;
        if (data != null) {
            switch ((string)data.SeriesDataMemberValue) {
                case "Pending":
                    result = pendingColor;
                    break;
                case "Shipping":
                    result = shippingColor;
                    break;
                case "Paid":
                    result = paidColor;
                    break;
                case "Processed":
                    result = processedColor;
                    break;
            }
        }
        return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        return value;
    }
}
