using System.Net.Mail;

using DevExpress.Maui.Controls;
using DevExpress.Maui.Core;
using DevExpress.Maui.DataForm;

using CrmDemo.DataModel.Models;
using Microsoft.EntityFrameworkCore;
using CrmDemo.ViewModels.Customers;
using CrmDemo.DataLayer;

namespace CrmDemo.Views.Customers;

public partial class CustomerEditCoreView : ContentView {
    public CustomerEditCoreView() {
        InitializeComponent();
    }
    public async Task<bool> TrySaveChangesAsync() {
        bool result = false;
        if (dataForm.Validate()) {
            dataForm.Commit();
            await viewModel.SaveAsync();
            result = true;
        }
        return result;
    }

    private DetailEditFormViewModel viewModel => (DetailEditFormViewModel)BindingContext;
    private CustomersViewModel customersViewModel => (CustomersViewModel)viewModel.DataControlContext;
    private CrmContext crmContext => customersViewModel.CrmContext;

    private void dataForm_ValidateProperty(object sender, DataFormPropertyValidationEventArgs e) {
        if (e.PropertyName == nameof(Person.Email) && e.NewValue != null) {
            MailAddress res;
            if (!MailAddress.TryCreate((string)e.NewValue, out res)) {
                e.HasError = true;
                e.ErrorText = "Invalid email";
            }
        }
    }
    private void ImageTapped(object sender, EventArgs e) {
        bottomSheet.State = BottomSheetState.HalfExpanded;
    }
    private async void OnSaveItemClicked(object sender, EventArgs e) {
        viewModel.CloseOnSave = false;
        if (await TrySaveChangesAsync()) {
            Customer customer = (Customer)viewModel.Item;
            if (crmContext.Entry(customer).State == EntityState.Detached) {
                customer.RegistrationDate = DateTime.Now.Date;
                crmContext.Customers.Add(customer);
            }
            crmContext.SaveChanges();
            viewModel.Close();
        }
    }
    private async void DeletePhotoClicked(object sender, EventArgs args) {
        await Dispatcher.DispatchAsync(() => {
            bottomSheet.State = BottomSheetState.Hidden;
            if (viewModel.Item is Person person) {
                person.Avatar = null;
            }
        });
    }
    private async void SelectPhotoClicked(object sender, EventArgs args) {
        await bottomSheet.CloseAsync();
        var photo = await MediaPicker.PickPhotoAsync();
        await ProcessResult(photo);
        
    }
    private async void TakePhotoClicked(object sender, EventArgs args) {
        await bottomSheet.CloseAsync();
        if (!MediaPicker.Default.IsCaptureSupported)
            return;

        var photo = await MediaPicker.Default.CapturePhotoAsync();
        await ProcessResult(photo);
        
    }
    private async Task ProcessResult(FileResult result) {
        if (result == null)
            return;

        ImageSource imageSource = null;
        if (System.IO.Path.IsPathRooted(result.FullPath))
            imageSource = ImageSource.FromFile(result.FullPath);
        else {
            var stream = await result.OpenReadAsync();
            imageSource = ImageSource.FromStream(() => stream);
        }
        var editorPage = new ImageEditView(imageSource);
        await Navigation.PushAsync(editorPage);

        var cropResult = await editorPage.WaitForResultAsync();
        if (cropResult != null && viewModel.Item is Person person) {
            person.Avatar = new CrmImage() { ThumbnailImage = new ImageData() { Data = cropResult } };
            person.Avatar.ClearCache();
        }

        editorPage.Handler.DisconnectHandler();
        OnSaveItemClicked(null, null);
    }
}
