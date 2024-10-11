using DevExpress.Maui.Charts;

using CrmDemo.ViewModels.Dashboards;

namespace CrmDemo.Views.Dashboards;

public partial class OrdersByStatePage : ContentPage {
    private readonly OrdersByStateViewModel viewModel;
    public OrdersByStatePage() {
        InitializeComponent();
        BindingContext = viewModel = new OrdersByStateViewModel();

    }

    private void PieChartView_SelectionChanged(object sender, DevExpress.Maui.Charts.SelectionChangedEventArgs e) {
        if (e.SelectedObjects.Count > 0) {
            viewModel.SelectedTrafficChannel = (TrafficChannel)((DataSourceKey)e.SelectedObjects[0]).DataObject;
        } else {
            viewModel.SelectedTrafficChannel = null;
        }
    }
}