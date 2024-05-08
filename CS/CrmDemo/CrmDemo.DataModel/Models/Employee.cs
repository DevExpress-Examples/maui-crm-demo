namespace CrmDemo.DataModel.Models;

public class Employee : Person {
    public string Position { get; set; }
    public virtual ICollection<Order> AssociatedOrders { get; set; }
    public virtual ICollection<Customer> AssociatedCustomers { get; set; }
    public virtual ICollection<Meeting> Meetings { get; set; }
}
