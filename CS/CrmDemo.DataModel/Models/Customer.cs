using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmDemo.DataModel.Models;

public class Customer : Person {
    public string Company { get; set; }
    public DateTime RegistrationDate { get; set; }
    public virtual Employee Employee { get; set; }
    public virtual ObservableCollection<CheckListItem> AssociatedCheckList { get; set; }
    public virtual ICollection<Order> Orders { get; set; }
    [NotMapped]
    public decimal OrdersAmount { get => Orders == null ? 0 : Orders.Sum(o => o.TotalAmount); }
    [NotMapped]
    public bool HasUnpaidOrders {
        get => (Orders != null) && Orders.Any(o => !o.IsPaid);
    }
}
