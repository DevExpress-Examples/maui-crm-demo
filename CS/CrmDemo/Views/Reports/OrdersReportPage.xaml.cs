using DevExpress.Maui.DataGrid;

using CrmDemo.ViewModels.Reports;

namespace CrmDemo.Views.Reports;

public partial class OrdersReportPage : ContentPage {
    public OrdersReportPage() {
        InitializeComponent();

    }

    private void OnAllButtonClicked(object sender, EventArgs e) {
        ((OrdersReportViewModel)BindingContext).SwitchOnAllStates();
    }
    private async void OnExportToExcelButtonClicked(object sender, EventArgs e) {
        string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, "Orders.xlsx");
        await dataGrid.ExportToXlsxAsync(targetFile);
        await Share.Default.RequestAsync(new ShareFileRequest {
            Title = "Share XSLX file",
            File = new ShareFile(targetFile)
        });
    }
}
