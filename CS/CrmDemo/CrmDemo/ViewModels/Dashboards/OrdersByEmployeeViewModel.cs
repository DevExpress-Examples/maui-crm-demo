using System.Text;
using System.Globalization;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

using DevExpress.Maui.Core;

using CrmDemo.DataLayer;
using CrmDemo.DataModel.Models;

namespace CrmDemo.ViewModels.Dashboards;

public class OrdersByEmployeeViewModel : BindableBase {
    private static readonly CompositeFormat compositeFormat = CompositeFormat.Parse("{0:M} - {1:M}");
    private CrmContext crmContext;
    private int processedOrdersCount;
    private Employee selectedEmployee;
    private DateTime startDate = DateTime.Now.Date.AddMonths(-1);
    private DateTime endDate = DateTime.Now.Date;
    private int totalOrdersCount;
    private int pendingOrdersCount;
    private int shippingOrdersCount;
    private int paidOrdersCount;
    private float selectionCompletedOrdersProgress;

    public string EmptySelectionString = "All Employees";
    public string PeriodText => String.Format(CultureInfo.CurrentCulture, compositeFormat, StartDate, EndDate);
    public Employee SelectedEmployee {
        get => selectedEmployee;
        set {
            selectedEmployee = value;
            RaisePropertyChanged(nameof(SelectedComponentString));
            Update();
        }
    }
    public string SelectedComponentString {
        get => (selectedEmployee != null) ? $"{selectedEmployee.FirstName} {selectedEmployee.LastName}" : EmptySelectionString;
    }
    public int PendingOrdersCount {
        get => pendingOrdersCount;
        set {
            pendingOrdersCount = value;
            RaisePropertyChanged();
        }
    }
    public int ShippingOrdersCount {
        get => shippingOrdersCount;
        set {
            shippingOrdersCount = value;
            RaisePropertyChanged();
        }
    }
    public int PaidOrdersCount {
        get => paidOrdersCount;
        set {
            paidOrdersCount = value;
            RaisePropertyChanged();
        }
    }
    public int ProcessedOrdersCount {
        get => processedOrdersCount;
        set {
            processedOrdersCount = value;
            RaisePropertyChanged();
        }
    }
    public int TotalOrdersCount {
        get => totalOrdersCount;
        set {
            totalOrdersCount = value;
            RaisePropertyChanged();
        }
    }
    public float SelectionCompletedOrdersProgress {
        get => selectionCompletedOrdersProgress;
        set {
            selectionCompletedOrdersProgress = value;
            RaisePropertyChanged();
        }
    }

    public DateTime StartDate {
        get => startDate;
        set {
            if (startDate != value) {
                startDate = value;
                Update();
                RaisePropertiesChanged(nameof(StartDate), nameof(PeriodText));
            }
        }
    }
    public DateTime EndDate {
        get => endDate;
        set {
            if (endDate != value) {
                endDate = value;
                Update();
                RaisePropertiesChanged(nameof(EndDate), nameof(PeriodText));
            }
        }
    }
    public ObservableCollection<EmployeeProcessedOrdersData> EmployeesProcessedOrdersCollection { get; set; }

    public OrdersByEmployeeViewModel() {
        crmContext = new CrmContext();
        Update();
    }

    private void Update() {
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

public record EmployeeProcessedOrdersData(Employee Employee, string EmployeeFullName, int OrdersProcessed);
