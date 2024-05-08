using System.ComponentModel;

using DevExpress.Maui.Core;
using DevExpress.Maui.Editors;
using DevExpress.Maui.DataGrid;

using CrmDemo.DataLayer;
using CrmDemo.DataModel.Models;
using CrmDemo.ViewModels.Customers;

namespace CrmDemo.Views.Customers;

public partial class CustomerDetailPage : ContentPage {
    private DetailFormViewModel viewModel => (DetailFormViewModel)BindingContext;
    private Customer Item => (Customer)viewModel.Item;
    private CrmContext crmContext => ((CustomersViewModel)viewModel.DataControlContext).CrmContext;

    public CustomerDetailPage() {
        InitializeComponent();
        checkListItemsGrid.ValidateAndSave += OnCheckListItemsGridValidateAndSave;
    }

    private void OnDataGridViewCustomCellAppearance(object sender, CustomCellAppearanceEventArgs e) {
        if (e.FieldName == nameof(CheckListItem.Description)) {
            if ((e.Item != null) && ((CheckListItem)e.Item).IsChecked) {
                e.TextDecorations = TextDecorations.Strikethrough;
                e.FontColor = e.FontColor.WithAlpha(0.5f);
            }
        }
    }
    private void DeleteItemClick(object sender, EventArgs e) {
        popup.IsOpen = true;
    }
    private void CancelDeleteClick(object sender, EventArgs e) {
        popup.IsOpen = false;
    }
    private void DeleteConfirmedClick(object sender, EventArgs e) {
        try {
            viewModel.CloseOnDelete = false;
            if (viewModel.Delete()) {
                Customer customer = (Customer)viewModel.Item;
                crmContext.Customers.Remove(customer);
                crmContext.SaveChanges();
                viewModel.Close();
            }
        } catch (Exception ex) {
            DisplayAlert("Error", ex.Message, "OK");
        }
    }
    private void OnAddTaskButtonClicked(object sender, EventArgs e) {
        if (!checkListExpander.IsExpanded) {
            checkListExpander.IsExpanded = true;
        }
        Customer customer = (Customer)viewModel.Item;
        customer.AssociatedCheckList.Add(new CheckListItem() { Description = "New Task" });
        crmContext.SaveChanges();
    }
    private void OnCheckListItemsGridTap(object sender, DataGridGestureEventArgs e) {
        if (e.FieldName == nameof(CheckListItem.IsChecked) && (e.Element == DataGridElement.Row)) {
            CheckListItem checkListItem = (CheckListItem)checkListItemsGrid.GetRowItem(e.RowHandle);
            checkListItem.IsChecked = !checkListItem.IsChecked;
            checkListItemsGrid.RefreshRow(e.RowHandle);
            crmContext.SaveChanges();
        }
    }
    private void OnCheckListItemsGridValidateAndSave(object sender, ValidateItemEventArgs e) {
        CheckListItem checkListItem = (CheckListItem)e.Item;
        crmContext.SaveChanges();
    }
    private void OnDeleteTaskSwipeItemTap(object sender, SwipeItemTapEventArgs e) {
        Customer customer = (Customer)viewModel.Item;
        customer.AssociatedCheckList.Remove((CheckListItem)e.Item);
        crmContext.SaveChanges();
    }

    async void MessageClick(object sender, EventArgs e) {
        if (Sms.Default.IsComposeSupported) {
            string[] recipients = new[] { Item.Phone };

            var message = new SmsMessage(string.Empty, recipients);

            await Sms.Default.ComposeAsync(message);
        }
    }
    void CallClick(object sender, EventArgs e) {
        if (PhoneDialer.Default.IsSupported && !String.IsNullOrEmpty(Item.Phone))
            PhoneDialer.Default.Open(Item.Phone);
    }
    async void MailClick(object sender, EventArgs e) {
        if (Email.Default.IsComposeSupported) {
            string[] recipients = new[] { Item.Email };

            var message = new EmailMessage {
                BodyFormat = EmailBodyFormat.PlainText,
                To = recipients.ToList()
            };

            await Email.Default.ComposeAsync(message);
        }
    }
    async void CopyPhoneClick(object sender, EventArgs e) {
        await Clipboard.Default.SetTextAsync(Item.Phone);
    }
    async void CopyEmailClick(object sender, EventArgs e) {
        await Clipboard.Default.SetTextAsync(Item.Email);
    }
}