using DevExpress.Maui.Core;

using CrmDemo.ViewModels.Products;

namespace CrmDemo.Views.Products;

public partial class ProductsPage : ContentPage {
    private ProductsViewModel viewModel;

    public ProductsPage(ProductsViewModel viewModel) {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = viewModel;
        viewModel.LoadDataAsync();
    }

    private void DXCollectionView_CreateDetailFormViewModel(object sender, CreateDetailFormViewModelEventArgs e) {
        if (e.DetailFormType == DetailFormType.View) {
            e.Result = new DetailFormViewModel(e.Item, new ProductDetailsViewModel());
        }
    }
    private void OnCollectionViewPullToRefresh(object sender, EventArgs e) {
        viewModel.LoadDataAsync();
    }
}