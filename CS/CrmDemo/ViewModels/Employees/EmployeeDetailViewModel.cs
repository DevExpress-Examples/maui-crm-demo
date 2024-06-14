using System.Collections.ObjectModel;
using DevExpress.Maui.Core;
using CrmDemo.DataLayer;
using CrmDemo.DataModel.Models;
using CrmDemo.ViewModels.Dashboards;

namespace CrmDemo.ViewModels.Employees;

public class EmployeeDetailViewModel : BindableBase {
    private readonly int employeeId;
    private ObservableCollection<DateCountValue> processedOrdersStats;

    public ObservableCollection<DateCountValue> ProcessedOrdersStats {
        get => processedOrdersStats;
        set {
            processedOrdersStats = value;
            RaisePropertiesChanged(nameof(ProcessedOrdersStats));
        }
    }
    public EmployeeDetailViewModel(Employee employee) {
        this.employeeId = employee.Id;
        LoadDataAsync();
    }
    private Task LoadDataAsync() {
        return Task.Run(() => LoadData());
    }
    private void LoadData() {
        using (CrmContext context = new CrmContext()) {
            var today = DateTime.Today;
            var thisMonth = new DateTime(today.Year, today.Month, 1);
            var date = thisMonth.AddMonths(-4);
            var orders = context.Orders
                .Where(x => x.Employee.Id == employeeId && x.OrderDate >= date)
                .ToList();
            var ordersByMonth = orders.GroupBy(x => (x.OrderDate.Month, x.OrderDate.Year))
                .Select(x => (x.Key, x.Count()))
                .ToList();
            var processedOrdersStats = ordersByMonth
                .Select(x => new DateCountValue(new DateTime(x.Key.Year, x.Key.Month, 1), x.Item2))
                .ToList();
            ;
            ProcessedOrdersStats = new ObservableCollection<DateCountValue>(processedOrdersStats);
        }
    }
}