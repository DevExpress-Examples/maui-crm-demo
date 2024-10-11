using DevExpress.XtraReports.UI;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.ConnectionParameters;
using CrmDemo.ViewModels.Reports;

namespace CrmDemo.Views;

public partial class ReportsPage : ContentPage {
    private ReportsViewModel ViewModel => (ReportsViewModel)BindingContext;

    public ReportsPage() {
        InitializeComponent();
    }

    public async Task<string> CopyWorkingFilesToAppData(string fileName) {
        using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(fileName);
        string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);
        if (File.Exists(targetFile)) {
            File.Delete(targetFile);
        }
        using FileStream outputStream = File.OpenWrite(targetFile);
        fileStream.CopyTo(outputStream);
        return targetFile;
    }
    private async Task ExportReportToPdf(string reportFileName, string outputFileName) {
        string reportTemplateFileName = await CopyWorkingFilesToAppData(reportFileName);
        XtraReport customerOrdersReport = XtraReport.FromXmlFile(reportTemplateFileName);
        SQLiteConnectionParameters connectionParameters = new SQLiteConnectionParameters() { FileName = App.DbFullPath };
        ((SqlDataSource)customerOrdersReport.DataSource).ConnectionParameters = connectionParameters;

        string ouputFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, outputFileName);
        customerOrdersReport.ExportToPdf(ouputFilePath);
        Dispatcher.Dispatch(async () => {
            await bottomSheet.CloseAsync();
            await Share.Default.RequestAsync(new ShareFileRequest {
                Title = "Share the report",
                File = new ShareFile(ouputFilePath)
            });
        });
    }
    private async void OnOrdersDetailsByCustomerButtonClicked(object sender, EventArgs e) {
        bottomSheet.Show();
        await Task.Run(() => ExportReportToPdf("orders_details_by_customer.repx", "OrdersDetailsByCustomer.pdf"));
    }
    private async void OnOrdersAgingByCustomerButtonClicked(object sender, EventArgs e) {
        bottomSheet.Show();
        await Task.Run(() => ExportReportToPdf("orders_aging_by_customer.repx", "OrdersAgingByCustomer.pdf"));
    }
}
