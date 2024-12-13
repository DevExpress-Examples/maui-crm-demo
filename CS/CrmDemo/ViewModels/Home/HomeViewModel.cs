using Microsoft.EntityFrameworkCore;
using DevExpress.Maui.Mvvm;
using CrmDemo.DataLayer;
using CrmDemo.DataModel.Models;
using CrmDemo.ViewModels.Common;

namespace CrmDemo.ViewModels.Home;

public class CheckListItemInfo {
    public CheckListItem CheckListItem { get; init; }
    public Customer Customer { get; init; }
}

public class HomeViewModel : DXObservableObject {
    

    private readonly UserSessionService userSessionService;
    private int newOrdersCount;

    private int assignedTasksCount;
    private int completedAssignedTasksCount;
    private List<CheckListItemInfo> tasks;

    private IEnumerable<Order> newOrders;
    private int meetingsAllCount;
    private IEnumerable<Meeting> meetingsToday;

    

    public int NewOrdersCount {
        get => newOrdersCount;
        set {
            newOrdersCount = value;
            OnPropertyChanged(nameof(NewOrdersCount));
        }
    }
    public IEnumerable<Order> NewOrders {
        get => newOrders;
        set {
            newOrders = value;
            OnPropertyChanged(nameof(NewOrders));
        }
    }

    public int AssignedTasksCount {
        get => assignedTasksCount;
        set {
            assignedTasksCount = value;
            OnPropertyChanged(nameof(AssignedTasksCount));
        }
    }
    public int CompletedAssignedTasksCount {
        get => completedAssignedTasksCount;
        set {
            completedAssignedTasksCount = value;
            OnPropertyChanged(nameof(CompletedAssignedTasksCount));
        }
    }
    public List<CheckListItemInfo> Tasks {
        get => tasks;
        set {
            tasks = value;
            OnPropertyChanged(nameof(Tasks));
        }
    }

    public int MeetingsAllCount {
        get => meetingsAllCount;
        set {
            meetingsAllCount = value;
            OnPropertyChanged(nameof(MeetingsAllCount));
        }
    }
    public IEnumerable<Meeting> MeetingsToday {
        get => meetingsToday;
        set {
            meetingsToday = value;
            OnPropertyChanged(nameof(MeetingsToday));
        }
    }

    public HomeViewModel(UserSessionService userSessionService) {
        this.userSessionService = userSessionService;
    }
    public void LoadData() {
        using (CrmContext crmContext = new CrmContext()) {
            NewOrders = crmContext.Orders
                .Where(it => it.State == OrderState.Paid)
                .Include(order => order.Employee)
                .Include(order => order.Items)
                .ThenInclude(item => item.Product).ToList();
            NewOrdersCount = NewOrders.Count();

            List<Customer> customers = crmContext.Customers.Where(x => x.Employee.Id == userSessionService.CurrentUserId).ToList();
            List<CheckListItemInfo> checkList =
                customers.SelectMany(x => x.AssociatedCheckList.Select(y => new CheckListItemInfo() { Customer = x, CheckListItem = y })).ToList();
            AssignedTasksCount = checkList.Count;
            CompletedAssignedTasksCount = checkList.Where(x => x.CheckListItem.IsChecked).Count();
            Tasks = checkList.Where(x => !x.CheckListItem.IsChecked).ToList();

            Employee currentUser = crmContext.Employees.Find(userSessionService.CurrentUserId);
            MeetingsAllCount = currentUser.Meetings.Count;
            MeetingsToday = currentUser.Meetings.Where(x => x.StartTime.Date == DateTime.Today).ToList();
        }
    }
    public Task LoadDataAsync() {
        return Task.Run(() => LoadData());
    }
}
