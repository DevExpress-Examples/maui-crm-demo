
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
            OnPropertyChanged(nameof(SelectedOrder));
        }
    }
    public ObservableCollection<Customer> Customers {
        get => customers;
        set {
            customers = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<Employee> Employees {
        get => employees;
        set {
            employees = value;
            OnPropertyChanged();
        }
    }
    public IEnumerable States {
        get => Enum.GetValues<OrderState>();
    }
    public ObservableCollection<Product> Products {
        get => products;
        set {
            products = value;
            OnPropertyChanged();
        }
    }
    public OrdersViewModel(UserSessionService sessionService) : base(sessionService) {
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
    private ObservableCollection<Employee> employees;
    private ObservableCollection<Product> products;
    private ObservableCollection<Customer> customers;
    private string customerFullName;
    private string employeeFullName;

    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query) {
        object parameter;
        if (query.TryGetValue("OrderId", out parameter)) {
            pendingNavigationOrderId = (int)parameter;
        }
        if (query.TryGetValue("CustomerFullName", out parameter)) {
            customerFullName = (string)parameter;
        }
        if (query.TryGetValue("EmployeeFullName", out parameter)) {
            employeeFullName = (string)parameter;
        }
    }

    protected override void OnApplyData(List<Order> list) {
        if (!string.IsNullOrEmpty(customerFullName)) {
            Items = new ObservableCollection<Order>(list.Where(x => x.Customer?.FullName == customerFullName));
            return;
        }
        if (!string.IsNullOrEmpty(employeeFullName)) {
            Items = new ObservableCollection<Order>(list.Where(x => x.Employee?.FullName == employeeFullName));
            return;
        }

        base.OnApplyData(list);
    }
}
