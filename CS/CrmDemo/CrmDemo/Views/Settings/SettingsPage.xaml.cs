using CrmDemo.ViewModels.Settings;
using DevExpress.Maui.Editors;

namespace CrmDemo.Views;

public partial class SettingsPage : ContentPage {
    private SettingsViewModel viewModel;

    public SettingsPage(SettingsViewModel viewModel) {
        this.viewModel = viewModel;
        InitializeComponent();
        BindingContext = viewModel;
    }
#if ANDROID
    protected override void OnAppearing() {
        base.OnAppearing();
        viewModel.Refresh();
    }
#endif
    private void Color_Changed(object sender, EventArgs e) {
        if (sender is not ChoiceChipGroup chipGroup || chipGroup.SelectedIndex < 0)
            return;
        var context = viewModel.Items[chipGroup.SelectedIndex];
        viewModel.ChangeColor(context);
    }
}