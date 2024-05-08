using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;

using Microsoft.EntityFrameworkCore;

using CrmDemo.DataLayer;
using CrmDemo.DataModel.Models;
using CrmDemo.ViewModels.Common;

namespace CrmDemo.ViewModels.Orders;

public class OrdersViewModel : CrmViewModelBase<Order>, IQueryAttributable {
    

    public Order SelectedOrder {
        get => selectedOrder;
        set {
            selectedOrder = value;
            RaisePropertiesChanged(nameof(SelectedOrder));
        }
    }
    public ObservableCollection<Customer> Customers {
        get => customers;
        set {
            customers = value;
            RaisePropertiesChanged();
        }
    }
    public ObservableCollection<Employee> Employees {
        get => employees;
        set {
            employees = value;
            RaisePropertiesChanged();
        }
    }
    public IEnumerable States {
        get => Enum.GetValues(typeof(OrderState));
    }
    public ObservableCollection<FilterItem> PredefinedFilters {
        get => predefinedFilters;
        set {
            predefinedFilters = value;
            RaisePropertiesChanged(nameof(PredefinedFilters));
        }
    }
    public ObservableCollection<Product> Products {
        get => products;
        set {
            products = value;
            RaisePropertiesChanged();
        }
    }
    public BindingList<FilterItem> SelectedFilters { get; set; }
    public string Filter {
        get => filter;
        set {
            filter = value;
            RaisePropertiesChanged();
        }
    }
    public OrdersViewModel(UserSessionService sessionService) : base(sessionService) {
        PopulatePredefinedFiltersAsync();
        SelectedFilters = new BindingList<FilterItem>();
        SelectedFilters.ListChanged += SelectedFiltersChanged;
        sortComparison = (Order o1, Order o2) => -Comparer.Default.Compare(o1.Id, o2.Id);
    }
    public bool DeleteCurrentOrder() {
        bool result = false;
        Order order = SelectedOrder;
        if (order != null) {
            Items.Remove(order);
            SelectedOrder = null;
            CrmContext.Orders.Remove(order);
            CrmContext.SaveChanges();
            result = true;
        }
        return result;
    }

    protected internal int? pendingNavigationOrderId;
    protected override IQueryable<Order> GetQueryable(CrmContext crmContext) {
        return crmContext.Orders
            .Include(o => o.Employee)
            .Include(o => o.Customer)
            .Include(o => o.Items).ThenInclude(i => i.Product);
    }
    protected override void OnLoadData(CrmContext crmContext) {
        Employees = new ObservableCollection<Employee>(crmContext.Employees);
        Products = new ObservableCollection<Product>(crmContext.Products);
        Customers = new ObservableCollection<Customer>(crmContext.Customers);
    }

    

    private Order selectedOrder;
    private string filter;
    private ObservableCollection<FilterItem> predefinedFilters;
    private ObservableCollection<Employee> employees;
    private ObservableCollection<Product> products;
    private ObservableCollection<Customer> customers;
    private BindingList<FilterItem> pendingSelectedFilters;

    private void AddFilter(object parameter, string filter) {
        var item = new FilterItem() { Filter = $"[{filter}].[Id] == '{parameter}'", DisplayText = $"{filter}.Id = {parameter}" };
        if (PredefinedFilters is null) {
            pendingSelectedFilters = new() {
                item
            };
        } else {
            SelectedFilters.Add(item);
        }
    }
    private void SelectedFiltersChanged(object sender, ListChangedEventArgs e) {
        if (SelectedFilters.Count > 0)
            Filter = string.Join(" AND ", SelectedFilters.Select(f => f.Filter));
        else
            Filter = string.Empty;
    }
    private void PopulatePredefinedFiltersAsync() {
        PredefinedFilters = new ObservableCollection<FilterItem>() {
            new FilterItem(){ DisplayText= "Assigned to Me", Filter = $"[Employee].[FullName] == '{SessionService.CurrentUser.FullName}'" },
            new FilterItem(){ DisplayText= "New", Filter = "IsThisWeek([OrderDate])" },
            new FilterItem(){ DisplayText= "Amount > $3000", Filter = "[TotalAmount] > 3000" },
            new FilterItem(){ DisplayText= "Pending", Filter = "[State] == 'Pending'" },
            new FilterItem(){ DisplayText= "Shipping", Filter = "[State] == 'Shipping'" },
            new FilterItem(){ DisplayText= "Paid", Filter = "[State] == 'Paid'" },
            new FilterItem(){ DisplayText= "Processed", Filter = "[State] == 'Processed'" },
        };
        if (pendingSelectedFilters != null && pendingSelectedFilters.Count > 0) {
            foreach (var item in pendingSelectedFilters) {
                PredefinedFilters.Insert(0, item);
                SelectedFilters.Add(item);
            }
        }
        pendingSelectedFilters = null;
    }

    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query) {
        object parameter;
        if (query.TryGetValue("ParentEmployeeId", out parameter)) {
            AddFilter(parameter, "Employee");
        } else if (query.TryGetValue("ParentCustomerId", out parameter)) {
            AddFilter(parameter, "Customer");
        } else if (query.TryGetValue("OrderId", out parameter)) {
            pendingNavigationOrderId = (int)parameter;
        }
    }
}
