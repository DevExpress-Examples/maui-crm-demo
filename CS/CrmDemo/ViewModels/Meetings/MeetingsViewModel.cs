using System.Collections.ObjectModel;
using System.Windows.Input;
using DevExpress.Maui.Mvvm;
using CrmDemo.DataLayer;
using CrmDemo.DataModel.Models;
using CrmDemo.ViewModels.Common;

namespace CrmDemo.ViewModels.Meetings;

public class MeetingsViewModel : DXObservableObject, IQueryAttributable {
    

    private UserSessionService userSessionService;
    private CrmContext crmContext;
    private DateTime calendarSelectedDate;
    private ObservableCollection<Meeting> meetings;
    private IEnumerable<Meeting> selectedDateMeetings;
    private bool isSimpleView = true;

    

    public DateTime CalendarSelectedDate {
        get {
            return calendarSelectedDate;
        }
        set {
            calendarSelectedDate = value;
            OnPropertyChanged();
            SelectedDateMeetings = Meetings?.Where(meeting => meeting.StartTime.Date == calendarSelectedDate.Date);
        }
    }
    public bool IsSimpleView {
        get {
            return isSimpleView;
        }
        set {
            isSimpleView = value;
            OnPropertyChanged();
        }
    }
    public ICommand SwitchViewCommand { get; set; }
    public IEnumerable<Meeting> SelectedDateMeetings {
        get {
            return selectedDateMeetings;
        }
        set {
            selectedDateMeetings = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<Meeting> Meetings {
        get => meetings;
        set {
            meetings = value;
            OnPropertyChanged(nameof(Meetings));
        }
    }

    public MeetingsViewModel(UserSessionService userSessionService) {
        this.userSessionService = userSessionService;
        SwitchViewCommand = new Command(SwitchView);
    }
    public Task LoadDataAsync() {
        return Task.Run(() => LoadData());
    }
    public void SwitchView() {
        IsSimpleView = !IsSimpleView;
    }
    public void SaveChanges() {
        Employee currentEmployee = (Employee)crmContext.Find<Employee>(userSessionService.CurrentUserId);
        foreach (Meeting meeting in crmContext.Meetings.Local) {
            if (meeting.Id == 0) {
                meeting.Employees.Add(currentEmployee);
            }
        }
        crmContext.SaveChanges();
    }

    protected internal int? pendingNavigationMeetingId;

    private void LoadData() {
        crmContext = new CrmContext();
        Meetings = new ObservableCollection<Meeting>(crmContext.Meetings.ToList());
    }
    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query) {
        object parameter;
        if (query.TryGetValue("MeetingId", out parameter)) {
            pendingNavigationMeetingId = (int)parameter;
        }
    }
}