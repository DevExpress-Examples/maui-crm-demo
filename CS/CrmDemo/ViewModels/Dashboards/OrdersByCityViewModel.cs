using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

using DevExpress.Maui.Core;
using CrmDemo.DataLayer;
using CrmDemo.DataModel.Models;

namespace CrmDemo.ViewModels.Dashboards;

public class CityRevenue {
    public string DateString { get; private set; }
    public string Country { get; private set; }
    public decimal Revenue { get; set; }

    public CityRevenue(string dateString, string country) {
        DateString = dateString;
        Country = country;
    }
    public void AddSum(decimal sum) {
        Revenue = Revenue + sum;
    }
}

public class OrdersByCityViewModel : BindableBase {
    private ObservableCollection<CityRevenue> dataItems;
    public ObservableCollection<CityRevenue> DataItems {
        get => dataItems;
        set {
            dataItems = value;
            RaisePropertiesChanged(nameof(DataItems));
        }
    }
    public OrdersByCityViewModel() {
        LoadDataAsync();
    }

    private void LoadData() {
        using (CrmContext crmContext = new CrmContext()) {
            Dictionary<string, CityRevenue> data = new Dictionary<string, CityRevenue>();
            DateTime now = DateTime.Now;
            DateTime startDate = new DateTime(now.Year, now.Month, 1).AddMonths(-2);
            List<Order> orders = crmContext.Orders.Include(o => o.Items).Where(o => o.OrderDate >= startDate).ToList();
            foreach (Order order in orders) {
                string dateString = order.OrderDate.ToString("MMM yyyy");
                string city = order.Customer?.Address?.City ?? "";
                string dataItemKey = dateString + "-" + city;
                CityRevenue dataItem = null;
                if (!data.TryGetValue(dataItemKey, out dataItem)) {
                    dataItem = new CityRevenue(dateString, city);
                    data.Add(dataItemKey, dataItem);
                }
                dataItem.AddSum(order.TotalAmount / 1000);
            }
            DataItems = new ObservableCollection<CityRevenue>(data.Values.ToList());
        }
    }
    private Task LoadDataAsync() {
        return Task.Run(() => LoadData());
    }
}
