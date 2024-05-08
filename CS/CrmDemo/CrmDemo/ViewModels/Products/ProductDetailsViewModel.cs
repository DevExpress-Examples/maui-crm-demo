using System.Collections.ObjectModel;
using DevExpress.Maui.Core;

namespace CrmDemo.ViewModels.Products;

public class ProductDetailsViewModel : BindableBase {
    public ObservableCollection<DateSales> Sales { get; set; }
    public ProductDetailsViewModel() {
        Sales = new ObservableCollection<DateSales>() {
            new DateSales(){ Date = DateTime.Now.AddMonths(-4), SalesAmount = 129.3m },
            new DateSales(){ Date = DateTime.Now.AddMonths(-3), SalesAmount = 130.1m },
            new DateSales(){ Date = DateTime.Now.AddMonths(-2), SalesAmount = 150.9m },
            new DateSales(){ Date = DateTime.Now.AddMonths(-1), SalesAmount = 140.3m },
            new DateSales(){ Date = DateTime.Now, SalesAmount = 180.6m }
        };
    }
}

public class DateSales {
    public DateTime Date { get; set; }
    public decimal SalesAmount { get; set; }
}