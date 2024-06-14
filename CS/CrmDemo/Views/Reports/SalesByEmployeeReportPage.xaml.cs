using DevExpress.Maui.DataGrid;

using CrmDemo.ViewModels.Reports;

namespace CrmDemo.Views.Reports;

public partial class SalesByEmployeeReportPage : ContentPage {
    public SalesByEmployeeReportPage() {
        InitializeComponent();

    }

    private void OnAllButtonClicked(object sender, EventArgs e) {
        ((SalesByEmployeeReportViewModel)BindingContext).SwitchOnAllStates();
    }
    private async void OnExportToExcelButtonClicked(object sender, EventArgs e) {
        string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, "EmployeesSales.xlsx");
        await dataGrid.ExportToXlsxAsync(targetFile);
        await Share.Default.RequestAsync(new ShareFileRequest {
            Title = "Share XSLX file",
            File = new ShareFile(targetFile)
        });
    }
}
