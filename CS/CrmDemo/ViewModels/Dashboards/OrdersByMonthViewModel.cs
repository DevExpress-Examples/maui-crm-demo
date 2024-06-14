using System.Collections.ObjectModel;
using CrmDemo.DataLayer;
using DevExpress.Maui.Core;
using Microsoft.EntityFrameworkCore;

namespace CrmDemo.ViewModels.Dashboards;

public class OrdersByMonthViewModel : BindableBase {
    private ObservableCollection<DateCountValue> pageViewStats;

    public ObservableCollection<DateCountValue> PageViewStats {
        get => pageViewStats;
        set {
            pageViewStats = value;
            RaisePropertiesChanged(nameof(PageViewStats));
        }
    }
    public ObservableCollection<string> OrderStatusFilters { get; set; }
    public OrdersByMonthViewModel() {
        OrderStatusFilters = new ObservableCollection<string> { "All", "Paid", "Pending", "Shipping", "Processed" };
        LoadDataAsync();
    }

    private void LoadData() {
        using (CrmContext context = new CrmContext()) {
            var orders = context.Orders.Include(o => o.Items).ThenInclude(i => i.Product).ToList();
            var stats = orders.GroupBy(x => FloorByWeek(x.OrderDate))
                .Select(x => new DateCountValue(x.Key, (int)(x.Sum(y => y.TotalAmount) / 1000)))
                .ToList();
            PageViewStats = new ObservableCollection<DateCountValue>(stats);
        }
    }
    private Task LoadDataAsync() {
        return Task.Run(() => LoadData());
    }

    static DateTime FloorByWeek(DateTime date) {
        return date.AddDays(-(int)date.DayOfWeek);
    }
}

public class DateCountValue {
    public DateTime Date { get; set; }
    public int Count { get; set; }
    public DateCountValue(DateTime date, int count) {
        Date = date;
        Count = count;
    }
}