using CrmDemo.ViewModels.Customers;

namespace CrmDemo.Views.Customers;

public partial class ImportCustomersPage : ContentPage {
    public ImportCustomersPage() {
        InitializeComponent();
        var viewModel = new ImportCustomersViewModel(
            async () => await Navigation.PopAsync(),
            async (s) => await DisplayAlert("Error", s, "Ok")
        );
        BindingContext = viewModel;
    }
}