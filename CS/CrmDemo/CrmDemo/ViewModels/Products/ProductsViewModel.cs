using CrmDemo.DataLayer;
using CrmDemo.DataModel.Models;
using CrmDemo.ViewModels.Common;

namespace CrmDemo.ViewModels.Products;

public class ProductsViewModel : CrmViewModelBase<Product> {
    public ProductsViewModel(UserSessionService sessionService) : base(sessionService) {
    }

    protected override IQueryable<Product> GetQueryable(CrmContext crmContext) {
        return crmContext.Products;
    }
}