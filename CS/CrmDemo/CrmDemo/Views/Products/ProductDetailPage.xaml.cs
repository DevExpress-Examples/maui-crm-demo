using DevExpress.Maui.Core;
using DevExpress.Maui.Core.Internal;

namespace CrmDemo.Views.Products;

public partial class ProductDetailPage : ContentPage {
    public ProductDetailPage() {
        InitializeComponent();
    }

    protected override void OnAppearing() {
        base.OnAppearing();
        col.Palette = new[] {
            ThemeManager.Theme.Scheme.Primary,
            ThemeManager.Theme.Scheme.Primary.OverrideAlpha(0.9),
            ThemeManager.Theme.Scheme.Primary.OverrideAlpha(0.8),
            ThemeManager.Theme.Scheme.Primary.OverrideAlpha(0.7),
            ThemeManager.Theme.Scheme.Primary.OverrideAlpha(0.6),
            ThemeManager.Theme.Scheme.Primary.OverrideAlpha(0.5),
        };
    }
}