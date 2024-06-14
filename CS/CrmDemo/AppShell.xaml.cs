using CrmDemo.DataLayer;
using CrmDemo.Views;
using CrmDemo.Views.Customers;
using CrmDemo.Views.Dashboards;
using CrmDemo.Views.Reports;

namespace CrmDemo;

public partial class AppShell : Shell {
    public AppShell() {
        InitializeComponent();
        BindingContext = this;
        
        Routing.RegisterRoute("customers", typeof(CustomersPage));
        Routing.RegisterRoute("orders", typeof(OrdersPage));
        Routing.RegisterRoute("meetings", typeof(MeetingsPage));
        
        Routing.RegisterRoute("relatedOrders", typeof(OrdersPage));
        Routing.RegisterRoute("invoicePdfPreview", typeof(InvoicePdfPreviewPage));
        
        Routing.RegisterRoute("ordersReport", typeof(OrdersReportPage));
        Routing.RegisterRoute("salesByEmployeeReport", typeof(SalesByEmployeeReportPage));
        
        Routing.RegisterRoute("ordersByEmployeeDashboard", typeof(OrdersByEmployeePage));
        Routing.RegisterRoute("ordersByMonthDashboard", typeof(OrdersByMonthPage));
        Routing.RegisterRoute("ordersByStateDashboard", typeof(OrdersByStatePage));
        Routing.RegisterRoute("orderStateEvolutionDashboard", typeof(OrderStateEvolutionPage));
        Routing.RegisterRoute("ordersByCountryDashboard", typeof(OrdersByCityPage));
        Routing.RegisterRoute("priceSoldItemsRelationDashboard", typeof(PriceSoldItemsRelationViewPage));

    }

#if ANDROID
    private bool isNavigation;
    private bool IsNavigationViaFlyout(ShellNavigationState target) {
        return target.Location.OriginalString.StartsWith("//");
    }
    protected override async void OnNavigating(ShellNavigatingEventArgs args) {
        base.OnNavigating(args);
        if (!isNavigation && IsNavigationViaFlyout(args.Target)) {
            isNavigation = true;
            try {
                args.Cancel();
                await Task.Delay(250);
                await GoToAsync(args.Target, false);
            } finally {
                isNavigation = false;
            }
        }
    }
#endif
    protected override void OnNavigated(ShellNavigatedEventArgs args) {
        base.OnNavigated(args);
        LoadDataAsync();
    }

    private Task LoadDataAsync() {
        return Task.Run(() => LoadData());
    }
    private void LoadData() {
        using (CrmContext crmContext = new CrmContext()) {
            customersShellItem.Content = crmContext.Customers.Count();
            meetingsShellItem.Content = crmContext.Meetings.Count();
            employeesShellItem.Content = crmContext.Employees.Count();
            ordersShellItem.Content = crmContext.Orders.Count();
            productsShellItem.Content = crmContext.Products.Count();
        }
    }
    private void OnCloseClicked(object sender, EventArgs e) {
        Current.FlyoutIsPresented = false;
    }
}
