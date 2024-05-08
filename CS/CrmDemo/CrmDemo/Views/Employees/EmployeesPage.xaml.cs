using DevExpress.Maui.Core;

using CrmDemo.DataModel.Models;
using CrmDemo.ViewModels.Employees;

namespace CrmDemo.Views;

public partial class EmployeesPage : ContentPage {
    public EmployeesPage(EmployeesViewModel viewModel) {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing() {
        base.OnAppearing();
        viewModel.LoadDataAsync();
    }

    private EmployeesViewModel viewModel;
    private void OnCreateDetailFormViewModel(object sender, CreateDetailFormViewModelEventArgs e) {
        if (e.DetailFormType == DetailFormType.View) {
            var employee = (Employee)e.Item;
            e.Result = new DetailFormViewModel(employee, new EmployeeDetailViewModel(employee));
        }
    }
}