using System.Text;
using System.Globalization;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using DevExpress.Maui.Mvvm;
using CrmDemo.DataLayer;
using CrmDemo.DataModel.Models;

namespace CrmDemo.ViewModels.Dashboards;

public class OrdersByEmployeeViewModel : DXObservableObject {
    private static readonly CompositeFormat compositeFormat = CompositeFormat.Parse("{0:M} - {1:M}");
    private int processedOrdersCount;
    private int selectedEmployeeId;
    private DateTime startDate = DateTime.Now.Date.AddMonths(-1);
    private DateTime endDate = DateTime.Now.Date;
    private int totalOrdersCount;
    private int pendingOrdersCount;
    private int shippingOrdersCount;
    private int paidOrdersCount;
    private float selectionCompletedOrdersProgress;

    public const string EmptySelectionString = "All Employees";
    public string PeriodText => String.Format(CultureInfo.CurrentCulture, compositeFormat, StartDate, EndDate);
    public int SelectedEmployeeId {
        get => selectedEmployeeId;
        set {
            selectedEmployeeId = value;
            OnPropertyChanged(nameof(SelectedComponentString));
            Update();
        }
    }
    public string SelectedComponentString { get; set; } = EmptySelectionString;
    public int PendingOrdersCount {
        get => pendingOrdersCount;
        set {
            pendingOrdersCount = value;
            OnPropertyChanged();
        }
    }
    public int ShippingOrdersCount {
        get => shippingOrdersCount;
        set {
            shippingOrdersCount = value;
            OnPropertyChanged();
        }
    }
    public int PaidOrdersCount {
        get => paidOrdersCount;
        set {
            paidOrdersCount = value;
            OnPropertyChanged();
        }
    }
    public int ProcessedOrdersCount {
        get => processedOrdersCount;
        set {
            processedOrdersCount = value;
            OnPropertyChanged();
        }
    }
    public int TotalOrdersCount {
        get => totalOrdersCount;
        set {
            totalOrdersCount = value;
            OnPropertyChanged();
        }
    }
    public float SelectionCompletedOrdersProgress {
        get => selectionCompletedOrdersProgress;
        set {
            selectionCompletedOrdersProgress = value;
            OnPropertyChanged();
        }
    }

    public DateTime StartDate {
        get => startDate;
        set {
            if (startDate != value) {
                startDate = value;
                Update();
                OnPropertyChanged(nameof(StartDate));
                OnPropertyChanged(nameof(PeriodText));
            }
        }
    }
    public DateTime EndDate {
        get => endDate;
        set {
            if (endDate != value) {
                endDate = value;
                Update();
                OnPropertyChanged(nameof(EndDate));
                OnPropertyChanged(nameof(PeriodText));
            }
        }
    }
    public ObservableCollection<EmployeeProcessedOrdersData> EmployeesProcessedOrdersCollection { get; set; }

    public OrdersByEmployeeViewModel() {
        Update();
    }

    private void Update() {
        using (CrmContext crmContext = new CrmContext()) {
            var employees = crmContext.Employees;
            var orders = crmContext.Orders;
            employees.Load();
            orders.Load();
            var minDate = orders.OrderBy(it => it.OrderDate);
            var maxDate = orders.OrderBy(it => it.OrderDate);
            var topEmployees = employees.OrderBy(it => it.AssociatedOrders.Count(x => x.OrderDate >= StartDate && x.OrderDate <= EndDate));
            var data = topEmployees.Select(it =>
                new EmployeeProcessedOrdersData(
                    it,
                    $"{it.FirstName} {it.LastName}",
                    it.AssociatedOrders.Count(x => x.OrderDate >= StartDate && x.OrderDate <= EndDate)
                )
            );
            EmployeesProcessedOrdersCollection = new ObservableCollection<EmployeeProcessedOrdersData>(data);
            var selectedEmployee = employees.FirstOrDefault(it => it.Id == SelectedEmployeeId);
            if (selectedEmployee == null) {
                PendingOrdersCount = employees.Sum(it => it.AssociatedOrders.Count(x => x.OrderDate >= StartDate && x.OrderDate <= EndDate && x.State == OrderState.Pending));
                ShippingOrdersCount = employees.Sum(it => it.AssociatedOrders.Count(x => x.OrderDate >= StartDate && x.OrderDate <= EndDate && x.State == OrderState.Shipping));
                PaidOrdersCount = employees.Sum(it => it.AssociatedOrders.Count(x => x.OrderDate >= StartDate && x.OrderDate <= EndDate && x.State == OrderState.Paid));
                ProcessedOrdersCount = employees.Sum(it => it.AssociatedOrders.Count(x => x.OrderDate >= StartDate && x.OrderDate <= EndDate && x.State == OrderState.Processed));
                TotalOrdersCount = employees.Sum(it => it.AssociatedOrders.Count(x => x.OrderDate >= StartDate && x.OrderDate <= EndDate));
            } else {
                PendingOrdersCount = selectedEmployee.AssociatedOrders.Count(x => x.OrderDate >= StartDate && x.OrderDate <= EndDate && x.State == OrderState.Pending);
                ShippingOrdersCount = selectedEmployee.AssociatedOrders.Count(x => x.OrderDate >= StartDate && x.OrderDate <= EndDate && x.State == OrderState.Shipping);
                PaidOrdersCount = selectedEmployee.AssociatedOrders.Count(x => x.OrderDate >= StartDate && x.OrderDate <= EndDate && x.State == OrderState.Paid);
                ProcessedOrdersCount = selectedEmployee.AssociatedOrders.Count(x => x.OrderDate >= StartDate && x.OrderDate <= EndDate && x.State == OrderState.Processed);
                TotalOrdersCount = selectedEmployee.AssociatedOrders.Count(x => x.OrderDate >= StartDate && x.OrderDate <= EndDate);
            }
            SelectionCompletedOrdersProgress = (float)ProcessedOrdersCount / (float)TotalOrdersCount;
        }
    }
}

public record EmployeeProcessedOrdersData(Employee Employee, string EmployeeFullName, int OrdersProcessed);
