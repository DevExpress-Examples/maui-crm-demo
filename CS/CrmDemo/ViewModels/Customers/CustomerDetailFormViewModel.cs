using DevExpress.Maui.Core;

using CrmDemo.DataLayer;
using CrmDemo.DataModel.Models;

namespace CrmDemo.ViewModels.Customers;

public class CustomerDetailFormViewModel : DetailFormViewModel {
    private CustomersViewModel customersViewModel => (CustomersViewModel)DataControlContext;
    private CrmContext crmContext => customersViewModel.CrmContext;

    public Employee CustomerEmployee {
        get => ((Customer)Item).Employee;
        set {
            ((Customer)Item).Employee = value;
            crmContext.SaveChanges();
            OnPropertyChanged(nameof(CustomerEmployee));
        }
    }
    public CustomerDetailFormViewModel(object item, object context = null)
        : base(item, context) {
    }
}
