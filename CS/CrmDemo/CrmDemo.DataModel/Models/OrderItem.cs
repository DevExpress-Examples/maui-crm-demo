namespace CrmDemo.DataModel.Models;

public class OrderItem {
    public int Id { get; set; }
    public int Quantity { get; set; }
    public virtual Order Order { get; set; }
    public virtual Product Product { get; set; }
    public decimal Amount {
        get { return (Product != null) ? (Quantity * Product.UnitPrice) : 0; }
    }
}
