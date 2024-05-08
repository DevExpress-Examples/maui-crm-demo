using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;

using DevExpress.Maui.Core;
using CrmDemo.DataLayer;
using CrmDemo.DataModel.Models;
using CrmDemo.ViewModels.Common;

namespace CrmDemo.ViewModels.Meetings;

public class MeetingsViewModel : BindableBase, IQueryAttributable {
    

    public DateTime CalendarSelectedDate {
        get {
            return calendarSelectedDate;
        }
        set {
            calendarSelectedDate = value;
            RaisePropertyChanged();
            SelectedDateMeetings = Meetings?.Where(meeting => meeting.StartTime.Date == calendarSelectedDate.Date);
        }
    }
    public bool IsSimpleView {
        get {
            return isSimpleView;
        }
        set {
            isSimpleView = value;
            RaisePropertyChanged();
        }
    }
    public ICommand SwitchViewCommand { get; set; }
    public IEnumerable<Meeting> SelectedDateMeetings {
        get {
            return selectedDateMeetings;
        }
        set {
            selectedDateMeetings = value;
            RaisePropertyChanged();
        }
    }
    public ObservableCollection<Meeting> Meetings { get; private set; }

    public MeetingsViewModel(UserSessionService sessionService) {
        crmContext = new CrmContext();
        crmContext.Meetings.Load();
        Meetings = crmContext.Meetings.Local.ToObservableCollection();
        SwitchViewCommand = new Command(SwitchView);
    }
    public void SwitchView() {
        IsSimpleView = !IsSimpleView;
    }
    public void SaveChanges() {
        Employee currentEmployee = (Employee)crmContext.Find(typeof(Employee), GetCurrentUserId());
        foreach (Meeting meeting in crmContext.Meetings.Local) {
            if (meeting.Id == 0) {
                meeting.Employees.Add(currentEmployee);
            }
        }
        crmContext.SaveChanges();
    }

    protected internal int? pendingNavigationMeetingId;

    

    private CrmContext crmContext;
    private DateTime calendarSelectedDate;
    private IEnumerable<Meeting> selectedDateMeetings;
    private bool isSimpleView = true;

    private int GetCurrentUserId() {
        int result = 0;
        using (UserSessionService userSessionService = new UserSessionService()) {
            result = userSessionService.CurrentUser.Id;
        }
        return result;
    }

    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query) {
        object parameter;
        if (query.TryGetValue("MeetingId", out parameter)) {
            pendingNavigationMeetingId = (int)parameter;
        }
    }
}