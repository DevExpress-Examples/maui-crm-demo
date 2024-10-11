using Microsoft.EntityFrameworkCore;

using DevExpress.Maui.Core;

using CrmDemo.DataLayer;
using CrmDemo.ViewModels.Orders;
using CrmDemo.DataModel.Models;

namespace CrmDemo.Views;

public partial class OrderEditPage : ContentPage {
    private DetailEditFormViewModel viewModel => (DetailEditFormViewModel)BindingContext;
    private OrdersViewModel ordersViewModel => (OrdersViewModel)viewModel.DataControlContext;
    private CrmContext crmContext => ordersViewModel.CrmContext;

    public OrderEditPage() {
        InitializeComponent();

    }
    
#if IOS
    protected override void OnNavigatingFrom(NavigatingFromEventArgs args) {
        base.OnNavigatingFrom(args);
        productComboBox.IsEnabled = false;
        T1220900_Item.IsEnabled = false;
    }
    protected override void OnNavigatedTo(NavigatedToEventArgs args) {
        base.OnNavigatedTo(args);
        productComboBox.IsEnabled = true;
        T1220900_Item.IsEnabled = true;
    }
#endif
    

    private async Task<bool> TrySaveChangesAsync() {
        bool result = false;
        if (dataForm.Validate()) {
            dataForm.Commit();
            await viewModel.SaveAsync();
            result = true;
        }
        return result;
    }
    private void OnAddOrderItemClicked(object sender, EventArgs e) {
        if ((productComboBox.SelectedItem != null) && (quantityEditor.Value > 0)) {
            Order order = (Order)viewModel.Item;
            Product product = (Product)productComboBox.SelectedItem;
            OrderItem orderItem = new OrderItem();
            orderItem.Product = product;
            orderItem.Quantity = (int)quantityEditor.Value;
            order.Items.Add(orderItem);
        }
    }
    private async void OnSaveToolbarItemClicked(object sender, EventArgs e) {
        viewModel.CloseOnSave = false;
        if (await TrySaveChangesAsync()) {
            Order order = (Order)viewModel.Item;
            if (crmContext.Entry(order).State == EntityState.Detached) {
                order.OrderDate = DateTime.Now;
                crmContext.Orders.Add(order);
            }
            crmContext.SaveChanges();
            viewModel.Close();
        }
    }
}