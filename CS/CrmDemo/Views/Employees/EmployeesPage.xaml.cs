using DevExpress.Maui.Core;

using CrmDemo.DataModel.Models;
using CrmDemo.ViewModels.Employees;

namespace CrmDemo.Views;

public partial class EmployeesPage : ContentPage {
    private EmployeesViewModel viewModel;

    public EmployeesPage(EmployeesViewModel viewModel) {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = viewModel;
#if IOS
        viewModel.LoadDataAsync();
#endif
    }

    protected override void OnAppearing() {
        base.OnAppearing();
#if ANDROID
        viewModel.LoadDataAsync();
#endif
    }

    private void OnCreateDetailFormViewModel(object sender, CreateDetailFormViewModelEventArgs e) {
        if (e.DetailFormType == DetailFormType.View) {
            var employee = (Employee)e.Item;
            e.Result = new DetailFormViewModel(employee, new EmployeeDetailViewModel(employee));
        }
    }
    private void OnCollectionViewPullToRefresh(object sender, EventArgs e) {
        viewModel.LoadDataAsync();
    }
}