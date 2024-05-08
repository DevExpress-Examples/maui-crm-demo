using System.Windows.Input;

using CrmDemo.DataLayer;
using CrmDemo.DataModel.Models;
using CrmDemo.ViewModels.Common;

namespace CrmDemo.ViewModels.Employees;

public class EmployeesViewModel : CrmViewModelBase<Employee> {
    public ICommand NavigateToRelatedOrdersCommand { get; }
    public ICommand NavigateToRelatedCustomersCommand { get; }
    public EmployeesViewModel(UserSessionService sessionService) : base(sessionService) {
        NavigateToRelatedOrdersCommand = new Command<Employee>(NavigateToRelatedOrders);
        NavigateToRelatedCustomersCommand = new Command<Employee>(NavigateToRelatedCustomers);
    }

    protected override IQueryable<Employee> GetQueryable(CrmContext crmContext) {
        return crmContext.Employees;
    }

    private async void NavigateToRelatedOrders(Employee currentEmployee) {
        var navigationParameter = new Dictionary<string, object> { { "ParentEmployeeId", currentEmployee.Id } };
        await Shell.Current.GoToAsync("relatedOrders", navigationParameter);
    }
    private async void NavigateToRelatedCustomers(Employee currentEmployee) {
        var navigationParameter = new Dictionary<string, object> { { "ParentEmployeeId", currentEmployee.Id } };
        await Shell.Current.GoToAsync("customers", navigationParameter);
    }
}