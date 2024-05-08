using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmDemo.DataModel.Models;

public enum OrderState {
    Pending,
    Shipping,
    Paid,
    Processed
}

public class Order {
    private ObservableCollection<OrderItem> items;

    public int Id { get; set; }
    public virtual Customer Customer { get; set; }
    public virtual Employee Employee { get; set; }
    public OrderState State { get; set; }
    public string Comment { get; set; }
    public DateTime OrderDate { get; set; }
    public virtual ObservableCollection<OrderItem> Items {
        get {
            if (items == null) {
                items = new ObservableCollection<OrderItem>();
            }
            return items;
        }
        set { items = value; }
    }
    [NotMapped]
    public decimal ItemsCount { get => Items.Count; }
    [NotMapped]
    public decimal TotalAmount { get => Items.Sum(d => d.Amount); }
    [NotMapped]
    public bool IsPaid { get => (State != OrderState.Paid) && (State != OrderState.Processed); }
}
