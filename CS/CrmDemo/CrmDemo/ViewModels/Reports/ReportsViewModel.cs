using System.Windows.Input;

namespace CrmDemo.ViewModels.Reports;

public class ReportsViewModel {
    public ICommand GoToOrdersReportCommand { get; set; }
    public ICommand GoToSalesByEmployeeReportCommand { get; set; }
    public ReportsViewModel() {
        GoToOrdersReportCommand = new Command(GoToOrdersReport);
        GoToSalesByEmployeeReportCommand = new Command(GoToSalesByEmployeeReport);
    }

    private void GoToOrdersReport() {
        Shell.Current.GoToAsync("ordersReport");
    }
    private void GoToSalesByEmployeeReport() {
        Shell.Current.GoToAsync("salesByEmployeeReport");
    }
}
