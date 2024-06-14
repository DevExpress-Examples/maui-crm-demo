using Microsoft.EntityFrameworkCore;

using DevExpress.Maui.Core;

using CrmDemo.DataLayer;
using CrmDemo.DataModel.Models;

namespace CrmDemo.ViewModels.Reports;

public class OrdersReportViewModel : BindableBase {
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
    public IList<Order> Orders {
        get => orders;
        set {
            orders = value;
            RaisePropertyChanged(nameof(Orders));
        }
    }
    public OrdersReportViewModel() {
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
    private IList<Order> orders;
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
            
            sourceOrders = sourceOrders
                .OrderBy(o => o.OrderDate)
                .Include(o => o.Employee)
                .Include(o => o.Customer)
                .Include(o => o.Items).ThenInclude(i => i.Product);

            Orders = sourceOrders.ToList();
        }
    }
    public Task LoadDataAsync() {
        return Task.Run(() => LoadData());
    }
}
