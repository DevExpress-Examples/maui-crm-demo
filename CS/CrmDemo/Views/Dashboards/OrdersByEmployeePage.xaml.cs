using DevExpress.Maui.Charts;
using DevExpress.Maui.Core;
using DevExpress.Maui.Core.Internal;
using CrmDemo.ViewModels.Dashboards;

namespace CrmDemo.Views.Dashboards;

public partial class OrdersByEmployeePage : ContentPage {
    private readonly OrdersByEmployeeViewModel viewModel;

    public OrdersByEmployeePage() {
        InitializeComponent();
        BindingContext = viewModel = new OrdersByEmployeeViewModel();
    }
    protected override void OnAppearing() {
        base.OnAppearing();
        col.Palette = new[] {
            ThemeManager.Theme.Scheme.Primary,
            ThemeManager.Theme.Scheme.Primary.OverrideAlpha(0.9),
            ThemeManager.Theme.Scheme.Primary.OverrideAlpha(0.8),
            ThemeManager.Theme.Scheme.Primary.OverrideAlpha(0.7),
            ThemeManager.Theme.Scheme.Primary.OverrideAlpha(0.6),
            ThemeManager.Theme.Scheme.Primary.OverrideAlpha(0.5),
        };
    }

    private void ChartView_SelectionChanged(object sender, DevExpress.Maui.Charts.SelectionChangedEventArgs e) {
        if (e.SelectedObjects.Count > 0) {
            viewModel.SelectedComponentString = ((EmployeeProcessedOrdersData)((DataSourceKey)e.SelectedObjects[0]).DataObject).Employee.FullName;
            viewModel.SelectedEmployeeId = ((EmployeeProcessedOrdersData)((DataSourceKey)e.SelectedObjects[0]).DataObject).Employee.Id;
        } else {
            viewModel.SelectedComponentString = OrdersByEmployeeViewModel.EmptySelectionString;
            viewModel.SelectedEmployeeId = -1;
        }
    }
    private void OnPeriodButtonTap(object sender, DevExpress.Maui.Core.DXTapEventArgs e) {
        datePicker.State = DevExpress.Maui.Controls.BottomSheetState.HalfExpanded;
    }
}
