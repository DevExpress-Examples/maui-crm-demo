using Microsoft.EntityFrameworkCore;

using DevExpress.Maui.Core;

using CrmDemo.DataModel.Models;
using CrmDemo.DataLayer;

namespace CrmDemo.ViewModels.Reports;

public class EmployeeSalesData {
    public EmployeeSalesData(string employeeFullName, IList<Order> orders) {
        EmployeeFullName = employeeFullName;
        OrdersCount = orders.Count;
        OrderItemsCount = orders.Sum(o => o.Items.Count);
        TotalAmount = orders.Sum(o => o.TotalAmount);
    }
    public string EmployeeFullName { get; set; }
    public int OrdersCount { get; set; }
    public int OrderItemsCount { get; set; }
    public decimal TotalAmount { get; set; }
}

public class SalesByEmployeeReportViewModel : BindableBase {
    public bool Pending {
        get => pending;
        set {
            pending = value;
            LoadDataAsync();
        }
    }
    public bool Shipping {
        get => shipping;
        set {
            shipping = value;
            LoadDataAsync();
        }
    }
    public bool Paid {
        get => paid;
        set {
            paid = value;
            LoadDataAsync();
        }
    }
    public bool Processed {
        get => processed;
        set {
            processed = value;
            LoadDataAsync();
        }
    }
    public DateTime? FromFilterDate {
        get => fromFilterDate;
        set {
            fromFilterDate = value;
            LoadDataAsync();
        }
    }
    public DateTime? ToFilterDate {
        get => toFilterDate;
        set {
            toFilterDate = value;
            LoadDataAsync();
        }
    }
    public IList<EmployeeSalesData> SalesData {
        get => salesData;
        set {
            salesData = value;
            RaisePropertyChanged(nameof(SalesData));
        }
    }
    public SalesByEmployeeReportViewModel() {
        pending = false;
        shipping = false;
        paid = false;
        processed = true;
        LoadDataAsync();
    }
    public void SwitchOnAllStates() {
        pending = true;
        shipping = true;
        paid = true;
        processed = true;
        LoadDataAsync();
        RaisePropertiesChanged("Pending", "Shipping", "Paid", "Processed");
    }

    private bool pending;
    private bool shipping;
    private bool paid;
    private bool processed;
    private DateTime? fromFilterDate;
    private DateTime? toFilterDate;
    public IList<EmployeeSalesData> salesData;
    private void LoadData() {
        using (CrmContext crmContext = new CrmContext()) {
            IQueryable<Order> sourceOrders = crmContext.Orders;
            
            if (!pending) {
                sourceOrders = sourceOrders.Where(o => o.State != OrderState.Pending);
            }
            if (!shipping) {
                sourceOrders = sourceOrders.Where(o => o.State != OrderState.Shipping);
            }
            if (!paid) {
                sourceOrders = sourceOrders.Where(o => o.State != OrderState.Paid);
            }
            if (!processed) {
                sourceOrders = sourceOrders.Where(o => o.State != OrderState.Processed);
            }
            
            if (fromFilterDate != null) {
                sourceOrders = sourceOrders.Where(o => o.OrderDate >= fromFilterDate.Value);
            }
            if (toFilterDate != null) {
                sourceOrders = sourceOrders.Where(o => o.OrderDate < toFilterDate.Value.AddDays(1));
            }
            
            sourceOrders = sourceOrders.Include(o => o.Employee).Include(o => o.Items).ThenInclude(i => i.Product);

            
            SalesData = sourceOrders.GroupBy(o => o.Employee,
                 (employee, orders) => new EmployeeSalesData(employee.FullName, orders.ToList())
            ).ToList();
        }
    }
    public Task LoadDataAsync() {
        return Task.Run(() => LoadData());
    }
}
