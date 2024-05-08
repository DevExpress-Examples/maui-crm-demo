using System.Globalization;
using DevExpress.Maui.Core;
using CrmDemo.DataModel.Models;
using CrmDemo.ViewModels.Customers;
using DevExpress.Maui.CollectionView;

namespace CrmDemo.Views.Customers;

public partial class CustomersPage : ContentPage {

    public CustomersPage(CustomersViewModel viewModel) {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing() {
        base.OnAppearing();
        viewModel.LoadDataAsync();
    }
    protected override void OnNavigatedTo(NavigatedToEventArgs args) {
        base.OnNavigatedTo(args);
        ApplyPendingNavigation();
    }

    private CustomersViewModel viewModel;

    private void ApplyPendingNavigation() {
        if (viewModel.pendingNavigationCustomerId != null) {
            int customerId = viewModel.pendingNavigationCustomerId.Value;
            viewModel.pendingNavigationCustomerId = null;
            Customer customer = viewModel.Items.FirstOrDefault(c => c.Id == customerId, null);
            if (customer != null) {
                collectionView.Commands.ShowDetailForm.Execute(customer);
            }
        }
    }

    private void OnCreateDetailFormViewModel(object sender, CreateDetailFormViewModelEventArgs e) {
        if (e.DetailFormType == DetailFormType.View) {
            e.Result = new CustomerDetailFormViewModel(e.Item);
        }
    }
    private void CollectionViewItemTap(object sender, CollectionViewGestureEventArgs e) {
        if (e.ItemHandle < 0) 
            return;
        collectionView.Commands.ShowDetailForm.Execute(e.Item);
    }
}

public class ItemRepresentationToIconConverter : IValueConverter {
    public ImageSource DefaultTemplateIcon { get; set; }
    public ImageSource AdvancedTemplateIcon { get; set; }
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if ((bool)value) {
            return AdvancedTemplateIcon;
        }
        return DefaultTemplateIcon;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

public class ItemRepresentationToTemplateConverter : IValueConverter {
    public DataTemplate DefaultTemplate { get; set; }
    public DataTemplate AdvancedTemplate { get; set; }
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if ((bool)value) {
            return AdvancedTemplate;
        }
        return DefaultTemplate;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
