using System.Globalization;

using DevExpress.DataAccess.ObjectBinding;
using DevExpress.XtraReports.UI;
using DevExpress.Maui.Controls;
using DevExpress.Maui.Core;
using DevExpress.Maui.DataGrid;
using DevExpress.Maui.CollectionView;

using CrmDemo.Helpers;
using CrmDemo.DataModel.Models;
using CrmDemo.ViewModels.Orders;

namespace CrmDemo.Views;

public partial class OrdersPage : ContentPage {
    private readonly OrdersViewModel viewModel;

    public OrdersPage(OrdersViewModel viewModel) {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = viewModel;
        viewModel.LoadDataAsync();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args) {
        base.OnNavigatedTo(args);
        ApplyPendingNavigation();
    }
    protected override void OnDisappearing() {
        base.OnDisappearing();
        detailInfoBottomSheet.Close();
    }

    private void ApplyPendingNavigation() {
        if (viewModel.pendingNavigationOrderId != null) {
            int orderId = viewModel.pendingNavigationOrderId.Value;
            viewModel.pendingNavigationOrderId = null;
            Order order = viewModel.Items.FirstOrDefault(c => c.Id == orderId, null);
            if (order != null) {
                collectionView.SelectedItem = order;
                detailInfoBottomSheet.HalfExpandedRatio = 0.5;
                detailInfoBottomSheet.State = BottomSheetState.HalfExpanded;
            }
        }
    }
    private TextColumn CreateColumn(string fieldName) {
        return new TextColumn() { FieldName = fieldName, AllowExport = true };
    }
    private GridColumnSummary CreateColumnSummary(string fieldName, DataSummaryType type, string displayFormat) {
        return new GridColumnSummary() { FieldName = fieldName, Type = type, DisplayFormat = displayFormat };
    }
    private DataGridView CreateDataGridView() {
        DataGridView result = new DataGridView();
        result.FilterString = viewModel.Filter;
        result.Columns.Add(CreateColumn("Id"));
        result.Columns.Add(CreateColumn("OrderDate"));
        result.Columns.Add(CreateColumn("TotalAmount"));
        result.Columns.Add(CreateColumn("Customer.FullName"));
        result.Columns.Add(CreateColumn("State"));
        result.TotalSummaries.Add(CreateColumnSummary("Id", DataSummaryType.Count, "Count: {0}"));
        result.TotalSummaries.Add(CreateColumnSummary("TotalAmount", DataSummaryType.Sum, "Total: {0}"));
        result.ItemsSource = viewModel.Items;
        result.Initialize();
        return result;
    }
    private async void ExcelExportClicked(object sender, EventArgs e) {
        sharePopup.IsOpen = false;
        DataGridView dataGrid = CreateDataGridView();
        string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, "Orders.xlsx");
        await dataGrid.ExportToXlsxAsync(targetFile);
        await Share.Default.RequestAsync(new ShareFileRequest {
            Title = "Share XSLX file",
            File = new ShareFile(targetFile)
        });
    }
    private async void PdfExportClicked(object sender, EventArgs e) {
        sharePopup.IsOpen = false;
        DataGridView dataGrid = CreateDataGridView();
        string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, "Orders.pdf");
        await dataGrid.ExportToPdfAsync(targetFile);
        await Share.Default.RequestAsync(new ShareFileRequest {
            Title = "Share PDF file",
            File = new ShareFile(targetFile)
        });
    }
    private void ExportClicked(object sender, EventArgs e) {
        sharePopup.Show();
    }
    private void dataGrid_SelectionChanged(object sender, DevExpress.Maui.DataGrid.SelectionChangedEventArgs e) {
    }
    private void dataGrid_Tap(object sender, CollectionViewGestureEventArgs e) {
        if (detailInfoBottomSheet.HalfExpandedRatio == 0.12 && detailInfoBottomSheet.State == BottomSheetState.HalfExpanded)
            detailInfoBottomSheet.Animate("detailInfoBottomSheet", x => detailInfoBottomSheet.HalfExpandedRatio = x, 0.12, 0.5);
        else
            detailInfoBottomSheet.HalfExpandedRatio = 0.5;
        detailInfoBottomSheet.State = BottomSheetState.HalfExpanded;
    }
    private void dataGrid_Scrolled(object sender, DXCollectionViewScrolledEventArgs e) {
        if (detailInfoBottomSheet.HalfExpandedRatio == 0.5)
            detailInfoBottomSheet.Animate("detailInfoBottomSheet", x => detailInfoBottomSheet.HalfExpandedRatio = x, 0.5, 0.12);
    }
    private async void OrderToPdfClick(object sender, EventArgs e) {
        DXButton exportButtom = (DXButton)sender;
        object originalButtonContent = exportButtom.Content;
        exportButtom.Content = new ActivityIndicator() { IsRunning = true };
        string ouputFilePath = string.Empty;

        await Task.Run(async () => {
            string reportTemplateFileName = await FileHelper.EnsureAssetInAppDataAsync("invoice_report_layout.repx");
            XtraReport OrderReport = XtraReport.FromXmlFile(reportTemplateFileName);
            ObjectDataSource objectDataSource = new ObjectDataSource();
            objectDataSource.DataSource = collectionView.SelectedItem;
            OrderReport.DataSource = objectDataSource;
            ouputFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, "InvoiceReport.pdf");
            OrderReport.ExportToPdf(ouputFilePath);
        });

        exportButtom.Content = originalButtonContent;
        await detailInfoBottomSheet.CloseAsync();
        var navigationParameter = new Dictionary<string, object> { { "documentFullPath", ouputFilePath } };
        await Shell.Current.GoToAsync("invoicePdfPreview", navigationParameter);
    }
    private async void OnDeleteButtonTap(object sender, DXTapEventArgs e) {
        await detailInfoBottomSheet.CloseAsync();
        var res = await DisplayAlert("Confirm delete", "Are you sure you want to delete this item?", "Delete", "Cancel");
        if (res) {
            res = viewModel.DeleteCurrentOrder();
            if (res) {
                await DisplayAlert("Success", "Order successfully deleted", "OK");
            } else {
                await DisplayAlert("Error", "Cannot delete order", "OK");
            }
        }
    }
    private async void OnEditButtonTap(object sender, DXTapEventArgs e) {
        await detailInfoBottomSheet.CloseAsync();
        if (viewModel.SelectedOrder != null) {
            collectionView.Commands.ShowDetailEditForm.Execute(viewModel.SelectedOrder);
        }
    }
    private void OnDataGridPullToRefresh(object sender, EventArgs e) {
        viewModel.LoadDataAsync();
    }
}

public class IsFilterEmptyConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is FilterValueInfo filterInfo && filterInfo != null) {
            return filterInfo.Count == 0;
        } else if (value is int selectedFilterItems) {
            return selectedFilterItems == 0;
        }
        return true;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}