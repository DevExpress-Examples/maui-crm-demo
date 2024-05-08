using DevExpress.Maui.CollectionView;

using CrmDemo.ViewModels.Home;
using CrmDemo.DataModel.Models;

namespace CrmDemo.Views;

public partial class HomePage : ContentPage {
    

    public HomePage(HomeViewModel viewModel) {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing() {
        base.OnAppearing();
        viewModel.LoadDataAsync();
    }

    

    private HomeViewModel viewModel;
    private void OnOrdersListItemTap(object sender, CollectionViewGestureEventArgs e) {
        newOrdersBottomSheet.Close();
        int orderId = ((Order)e.Item).Id;
        ShellNavigationQueryParameters navigationParameters = new ShellNavigationQueryParameters { { "OrderId", orderId } };
        Shell.Current.GoToAsync("orders", navigationParameters);
    }
    private void OnTasksListItemTap(object sender, CollectionViewGestureEventArgs e) {
        tasksBottomSheet.Close();
        int customerId = ((CheckListItemInfo)e.Item).Customer.Id;
        ShellNavigationQueryParameters navigationParameters = new ShellNavigationQueryParameters { { "CustomerId", customerId } };
        Shell.Current.GoToAsync("customers", navigationParameters);
    }
    private void OnFundsClicked(object sender, EventArgs e) {
        Shell.Current.GoToAsync("ordersByMonthDashboard");
    }
    private void OnAllMeetingsClicked(object sender, EventArgs e) {
        Shell.Current.GoToAsync("meetings");
    }
    private void OnMeetingsListItemTap(object sender, CollectionViewGestureEventArgs e) {
        int meetingId = ((Meeting)e.Item).Id;
        ShellNavigationQueryParameters navigationParameters = new ShellNavigationQueryParameters { { "MeetingId", meetingId } };
        Shell.Current.GoToAsync("meetings", navigationParameters);
    }
}