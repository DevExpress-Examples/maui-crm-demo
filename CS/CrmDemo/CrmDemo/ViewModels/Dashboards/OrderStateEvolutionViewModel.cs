using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.EntityFrameworkCore;

using DevExpress.Maui.Core;

using CrmDemo.DataLayer;
using CrmDemo.DataModel.Models;

namespace CrmDemo.ViewModels.Dashboards;

public class StateRevenue {
    public string DateString { get; private set; }
    public string State { get; private set; }
    public decimal Revenue { get; private set; }
    public Color Color { get => Colors.Red; }

    public StateRevenue(string dateString, string state) {
        DateString = dateString;
        State = state;
    }
    public void AddSum(decimal sum) {
        Revenue = Revenue + sum;
    }
}

public class Segment {
    public string Name { get; set; }
    public Color Color { get; set; }
}

public class OrdersStateEvolutionViewModel : BindableBase {
    private ObservableCollection<StateRevenue> dataItems;
    private CrmContext crmContext;

    public ObservableCollection<StateRevenue> DataItems {
        get => dataItems;
        set {
            dataItems = value;
            RaisePropertyChanged();
        }
    }

    public OrdersStateEvolutionViewModel() {
        crmContext = new CrmContext();
        LoadData();
    }
    private void LoadData() {
        Dictionary<string, StateRevenue> data = new Dictionary<string, StateRevenue>();
        DateTime now = DateTime.Now;
        DateTime startDate = new DateTime(now.Year, now.Month, 1).AddMonths(-2);
        List<Order> orders = crmContext.Orders.Include(o => o.Items).Where(o => o.OrderDate >= startDate).ToList();
        foreach (Order order in orders) {
            string dateString = order.OrderDate.ToString("MMM yyyy");
            string state = order.State.ToString();
            string dataItemKey = dateString + "-" + state;
            StateRevenue dataItem = null;
            if (!data.TryGetValue(dataItemKey, out dataItem)) {
                dataItem = new StateRevenue(dateString, state);
                data.Add(dataItemKey, dataItem);
            }
            dataItem.AddSum(order.TotalAmount);
        }
        DataItems = new ObservableCollection<StateRevenue>(data.Values.ToList());
    }
}
