using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;

using CrmDemo.DataLayer;
using CrmDemo.DataModel.Models;
using CrmDemo.ViewModels.Common;

namespace CrmDemo.ViewModels.Customers;

public class CustomersViewModel : CrmViewModelBase<Customer>, IQueryAttributable {
    private ObservableCollection<Employee> employees;
    private ObservableCollection<string> companies;
    private bool isAdvancedItemRepresentation;

    public ObservableCollection<Employee> Employees {
        get => employees;
        set {
            employees = value;
            OnPropertyChanged(nameof(Employees));
        }
    }
    public ObservableCollection<string> Companies {
        get => companies;
        set {
            companies = value;
            OnPropertyChanged(nameof(Companies));
        }
    }
    public ICommand NavigateToRelatedOrdersCommand { get; }
    public ICommand SwitchItemViewCommand { get; set; }
    public bool IsAdvancedItemRepresentation {
        get => isAdvancedItemRepresentation;
        set {
            isAdvancedItemRepresentation = value;
            OnPropertyChanged(nameof(IsAdvancedItemRepresentation));
        }
    }
    public CustomersViewModel(UserSessionService sessionService) : base(sessionService) {
        NavigateToRelatedOrdersCommand = new Command<Customer>(NavigateToRelatedOrders);
        SwitchItemViewCommand = new Command(SwitchItemView);
    }
    public async void NavigateToRelatedOrders(Customer currentCustomer) {
        var navigationParameter = new Dictionary<string, object> { { "CustomerFullName", currentCustomer.FullName } };
        await Shell.Current.GoToAsync("relatedOrders", navigationParameter);
    }

    protected internal int? pendingNavigationCustomerId;
    protected override IQueryable<Customer> GetQueryable(CrmContext crmContext) {
        return crmContext.Customers
            .Include(c => c.Employee)
            .Include(c => c.Avatar).ThenInclude(a => a.FullImage)
            .Include(c => c.Avatar).ThenInclude(a => a.ThumbnailImage);
    }
    protected override void OnLoadData(CrmContext crmContext) {
        base.OnLoadData(crmContext);
        Companies = new ObservableCollection<string>(crmContext.Customers.ToList().Select(c => c.Company).Distinct());
        Employees = new ObservableCollection<Employee>(crmContext.Employees.Include(e => e.Avatar).ToList());
    }

    private void SwitchItemView(object obj) {
        IsAdvancedItemRepresentation = !IsAdvancedItemRepresentation;
    }

    private string employeeFullName;
    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query) {
        object parameter;
        if (query.TryGetValue("CustomerId", out parameter)) {
            pendingNavigationCustomerId = (int)parameter;
        }
        if (query.TryGetValue("EmployeeFullName", out parameter)) {
            employeeFullName = (string)parameter;
        }
    }

    protected override void OnApplyData(List<Customer> list) {
        if (!string.IsNullOrEmpty(employeeFullName)) {
            Items = new ObservableCollection<Customer>(list.Where(x => x.Employee?.FullName == employeeFullName));
            return;
        }

        base.OnApplyData(list);
    }
}