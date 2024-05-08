using System.Globalization;
using DevExpress.Maui.Core;
using DevExpress.Maui.Scheduler;
using DevExpress.Maui.Scheduler.Internal;
using CrmDemo.ViewModels.Meetings;

namespace CrmDemo.Views;

public partial class MeetingsPage : ContentPage {
    public MeetingsPage(MeetingsViewModel viewModel) {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = viewModel;
        Loaded += OnLoaded;
    }
    protected override void OnAppearing() {
        base.OnAppearing();
        inNavigation = false;
    }
    protected override void OnNavigatedTo(NavigatedToEventArgs args) {
        base.OnNavigatedTo(args);
        Dispatcher.Dispatch(async () => await ApplyPendingNavigation());
    }

    private MeetingsViewModel viewModel;
    private bool inNavigation;
    private bool isDataSaving;
    private bool isLoaded;

    private async Task ApplyPendingNavigation() {
        if (viewModel.pendingNavigationMeetingId != null) {
            int meetingId = viewModel.pendingNavigationMeetingId.Value;
            viewModel.pendingNavigationMeetingId = null;
            AppointmentItem appointmentItem = dayView.DataStorage.GetAppointmentItemById(meetingId);
            if (appointmentItem != null) {
                await ShowAppointmentForm(appointmentItem);
            }
        }
    }
    private void SaveChanges() {
        if (!isLoaded)
            return;
        if (isDataSaving)
            return;
        isDataSaving = Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(10), () => {
            ((MeetingsViewModel)BindingContext).SaveChanges();
            isDataSaving = false;
        });
    }
    private void OnLoaded(object sender, EventArgs e) {
        isLoaded = true;
    }
    private async void OnNewClicked(object sender, EventArgs e) {
        await ShowAppointmentForm(null);
    }
    private async void OnDayViewTap(object sender, SchedulerGestureEventArgs e) {
        if (this.inNavigation)
            return;
        var a = e.AppointmentInfo?.Appointment;
        if (a == null)
            return;
        await ShowAppointmentForm(a);
    }
    private async Task ShowAppointmentForm(AppointmentItem a) {
        AppointmentDetailViewModel detailViewModel;
        AppointmentEditViewModel editViewModel;
        if (a != null) {
            detailViewModel = new AppointmentDetailViewModel(a, dataStorage);
            editViewModel = detailViewModel.CreateAppointmentEditViewModel();
        } else {
            var date = dayView.Start.Date;
            detailViewModel = null;
            editViewModel = new AppointmentEditViewModel(date.AddHours(9), date.AddHours(10), false, dataStorage);
        }
        var page = new AppointmentEditPage(detailViewModel, editViewModel);
        inNavigation = true;
        await Navigation.PushAsync(page);
    }
    private void OnSchedulerItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e) {
        var storage = (SchedulerDataStorage)sender;
        if (storage.Parent == null)
            return;
        if (e.ItemType != ItemType.AppointmentItem)
            return;
        SaveChanges();
    }
    private void OnSchedulerItemCollectionChanged(object sender, ItemsCollectionChangedEventArgs e) {
        var storage = (SchedulerDataStorage)sender;
        if (storage.Parent == null)
            return;
        if (e.ItemType != ItemType.AppointmentItem)
            return;
        SaveChanges();
    }
    private void OnCalendarCustomDayCellAppearance(object sender, DevExpress.Maui.Editors.CustomSelectableCellAppearanceEventArgs e) {
        if (e.Date == ((MeetingsViewModel)BindingContext).CalendarSelectedDate.Date) {
            e.EllipseBackgroundColor = ThemeManager.Theme.Scheme.Primary;
        }
        if (((MeetingsViewModel)BindingContext).Meetings.Any(meeting => meeting.StartTime.Date == e.Date)) {
            e.EllipseBackgroundColor = ThemeManager.Theme.Scheme.OnSurface.WithAlpha(0.08f);
        }
    }
}

public class BoolToToolbarIconConverter : IValueConverter {
    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is not bool && targetType != typeof(ImageSource)) {
            return null;
        }

        if ((bool)value) {
            return ImageSource.FromFile("scheduler");
        }
        return ImageSource.FromFile("meetings");
    }
    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}