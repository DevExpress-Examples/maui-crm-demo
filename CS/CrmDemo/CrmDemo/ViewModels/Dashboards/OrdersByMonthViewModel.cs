using System.Collections.ObjectModel;
using CrmDemo.DataLayer;
using Microsoft.EntityFrameworkCore;

namespace CrmDemo.ViewModels.Dashboards;

public class OrdersByMonthViewModel {
    public ObservableCollection<DateCountValue> PageviewStats { get; set; }
    public ObservableCollection<string> OrderStatusFilters { get; set; }

    public OrdersByMonthViewModel() {
        var context = new CrmContext();
        context.Orders.Load();
        var orders = context.Orders.Local.ToList();
        var stats = orders.GroupBy(x => FloorByWeek(x.OrderDate))
            .Select(x => new DateCountValue(x.Key, (int)(x.Sum(y => y.TotalAmount) / 1000)))
            .ToList();
        PageviewStats = new ObservableCollection<DateCountValue>(stats);

        OrderStatusFilters = new ObservableCollection<string> { "All", "Paid", "Pending", "Shipping", "Processed" };
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