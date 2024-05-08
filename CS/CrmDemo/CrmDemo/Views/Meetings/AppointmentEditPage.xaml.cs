using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using MApplication = Microsoft.Maui.Controls.Application;

using DevExpress.Maui.Scheduler;
using DevExpress.Maui.Scheduler.Internal;

namespace CrmDemo.Views;

public partial class AppointmentEditPage : ContentPage, IDialogService {
    const string SaveIconName = "dxsch_check";
    const string LightThemePostfix = "_light";
    const string DarkThemePostfix = "_dark";
    const string FileResolution = ".png";

    

    public AppointmentEditPage(AppointmentDetailViewModel detailViewModel, AppointmentEditViewModel viewModel) {
        this.detailViewModel = detailViewModel;
        BindingContext = this.viewModel = viewModel;
        if (detailViewModel != null)
            detailViewModel.DialogService = this;
        viewModel.DialogService = this;
        InitializeComponent();
        UpdateToolbarItems();
        if (detailViewModel == null) {
            deleteButton.IsVisible = false;
            deleteButtonSeparator.IsVisible = false;
        }
    }
    protected override void OnAppearing() {
        base.OnAppearing();
        MApplication.Current.RequestedThemeChanged += CurrentOnRequestedThemeChanged;
        ApplySafeInsets();
        SizeChanged += OnSizeChanged;
    }
    protected override void OnDisappearing() {
        base.OnDisappearing();
        SizeChanged -= OnSizeChanged;
        MApplication.Current.RequestedThemeChanged -= CurrentOnRequestedThemeChanged;
    }

    

    private readonly AppointmentDetailViewModel detailViewModel;
    private readonly AppointmentEditViewModel viewModel;
    private bool IsLightTheme => MApplication.Current.RequestedTheme == AppTheme.Light;

    private void CurrentOnRequestedThemeChanged(object sender, AppThemeChangedEventArgs e) {
        UpdateToolbarItems();
    }
    private void OnSizeChanged(object sender, EventArgs e) {
        ApplySafeInsets();
    }
    private async void OnSaveTapped(object sender, EventArgs e) {
        if (await viewModel.SaveChanges()) {
            await Navigation.PopAsync();
        }
    }
    private void OnCaptionTapped(object sender, EventArgs e) {
        this.eventNameEntry.Focus();
    }
    private void OnAllDayTapped(object sender, EventArgs e) {
        this.allDaySwitch.IsToggled = !this.allDaySwitch.IsToggled;
    }
    private void UpdateToolbarItems() {
        string actualPostfix = String.Empty;
        actualPostfix = IsLightTheme ? LightThemePostfix : DarkThemePostfix;
        this.saveToolbarItem.IconImageSource = new FileImageSource { File = SaveIconName + actualPostfix + FileResolution };
    }
    private void ApplySafeInsets() {
        Thickness safeInsets = On<Microsoft.Maui.Controls.PlatformConfiguration.iOS>().SafeAreaInsets();
        this.RecreateStyleWithHorizontalInsets("FormItemStyle", "FormItemStyleBase", typeof(Grid), safeInsets);
        this.RecreateStyleWithHorizontalInsets("FormDateTimeItemStyle", "FormDateTimeItemStyleBase", typeof(StackLayout), safeInsets);
        this.RecreateStyleWithHorizontalInsets("Wrapper", "WrapperBase", typeof(Frame), safeInsets);
        this.root.Margin = new Thickness(0, safeInsets.Top, 0, safeInsets.Bottom);
    }
    private async void OnDeleteClicked(object sender, EventArgs e) {
        if (detailViewModel == null)
            return;
        if (await detailViewModel.RemoveAppointment())
            await Navigation.PopAsync();
    }

    Task<bool> IDialogService.DisplayAlertMessage(string title, string message, string accept, string cancel) => DisplayAlert(title, message, accept, cancel);
    Task<string> IDialogService.DisplaySelectItemDialog(string title, string cancel, params string[] options) => DisplayActionSheet(title, cancel, null, options);
}