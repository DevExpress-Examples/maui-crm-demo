using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;

using CrmDemo.DataLayer;
using CrmDemo.DataModel.Models;
using CrmDemo.ViewModels.Common;

namespace CrmDemo.ViewModels.Customers;

public class CustomersViewModel : CrmViewModelBase<Customer>, IQueryAttributable {
    private ObservableCollection<Employee> employees;
    private ObservableCollection<string> companies;
    private ObservableCollection<FilterItem> predefinedFilters;
    private string filter;
    private bool isAdvancedItemRepresentation;
    private BindingList<FilterItem> pendingSelectedFilters;

    public ObservableCollection<Employee> Employees {
        get => employees;
        set {
            employees = value;
            RaisePropertiesChanged(nameof(Employees));
        }
    }
    public ObservableCollection<string> Companies {
        get => companies;
        set {
            companies = value;
            RaisePropertiesChanged(nameof(Companies));
        }
    }
    public ObservableCollection<FilterItem> PredefinedFilters {
        get => predefinedFilters;
        set {
            predefinedFilters = value;
            RaisePropertiesChanged(nameof(PredefinedFilters));
        }
    }
    public BindingList<FilterItem> SelectedFilters { get; set; }
    public string Filter {
        get => filter;
        set {
            filter = value;
            RaisePropertiesChanged(nameof(Filter));
        }
    }
    public ICommand NavigateToRelatedOrdersCommand { get; }
    public ICommand SwitchItemViewCommand { get; set; }
    public bool IsAdvancedItemRepresentation {
        get => isAdvancedItemRepresentation;
        set {
            isAdvancedItemRepresentation = value;
            RaisePropertiesChanged(nameof(IsAdvancedItemRepresentation));
        }
    }
    public CustomersViewModel(UserSessionService sessionService) : base(sessionService) {
        NavigateToRelatedOrdersCommand = new Command<Customer>(NavigateToRelatedOrders);
        SwitchItemViewCommand = new Command(SwitchItemView);
        PopulatePredefinedFilters();
        SelectedFilters = new BindingList<FilterItem>();
        SelectedFilters.ListChanged += SelectedFiltersChanged;
    }
    public async void NavigateToRelatedOrders(Customer currentCustomer) {
        var navigationParameter = new Dictionary<string, object> { { "ParentCustomerId", currentCustomer.Id } };
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

    private void PopulatePredefinedFilters() {
        PredefinedFilters = new ObservableCollection<FilterItem>() {
            new FilterItem(){ DisplayText= "Assigned to Me", Filter = $"[Employee.FullName] = '{SessionService.CurrentUserFullName}'" },
            new FilterItem(){ DisplayText= "New", Filter = "IsThisWeek([RegistrationDate])" },
            new FilterItem(){ DisplayText= "Orders > $200k", Filter = "[OrdersAmount] > 200000" },
        };
        if (pendingSelectedFilters != null && pendingSelectedFilters.Count > 0) {
            foreach (var item in pendingSelectedFilters) {
                PredefinedFilters.Insert(0, item);
                SelectedFilters.Add(item);
            }
        }
        pendingSelectedFilters = null;
    }
    private void SwitchItemView(object obj) {
        IsAdvancedItemRepresentation = !IsAdvancedItemRepresentation;
    }
    private void SelectedFiltersChanged(object sender, ListChangedEventArgs e) {
        if (SelectedFilters.Count > 0) {
            Filter = string.Join(" And ", SelectedFilters.Select(f => f.Filter));
        } else {
            Filter = string.Empty;
        }
    }

    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query) {
        object parameter;
        if (query.TryGetValue("ParentEmployeeId", out parameter)) {
            FilterItem item = new FilterItem() { Filter = $"[Employee.Id] = '{parameter}'", DisplayText = $"Employee.Id = {parameter}" };
            if (PredefinedFilters is null) {
                pendingSelectedFilters = new() {
                    item,
                };
            } else {
                SelectedFilters.Add(item);
            }
        } else if (query.TryGetValue("CustomerId", out parameter)) {
            pendingNavigationCustomerId = (int)parameter;
        }
    }
}