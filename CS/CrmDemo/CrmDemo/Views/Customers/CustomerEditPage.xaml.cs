namespace CrmDemo.Views.Customers;

public partial class CustomerEditPage : ContentPage {
    readonly ImportCustomersPage importCustomersPage;

    public CustomerEditPage() {
        InitializeComponent();
        importCustomersPage = new ImportCustomersPage();
    }

    private async void OnImportCustomersClicked(object sender, EventArgs e) {
        await Navigation.PushAsync(importCustomersPage);
    }
}