using DevExpress.Maui.Core;

using CrmDemo.ViewModels.Products;

namespace CrmDemo.Views.Products;

public partial class ProductsPage : ContentPage {
    public ProductsPage(ProductsViewModel viewModel) {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing() {
        base.OnAppearing();
        viewModel.LoadDataAsync();
    }

    private ProductsViewModel viewModel;
    private void DXCollectionView_CreateDetailFormViewModel(object sender, CreateDetailFormViewModelEventArgs e) {
        if (e.DetailFormType == DetailFormType.View) {
            e.Result = new DetailFormViewModel(e.Item, new ProductDetailsViewModel());
        }
    }
}