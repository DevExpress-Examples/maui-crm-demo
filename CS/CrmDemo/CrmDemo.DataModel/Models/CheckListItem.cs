namespace CrmDemo.DataModel.Models;

public class CheckListItem {
    public int Id { get; set; }
    public string Description { get; set; }
    public bool IsChecked { get; set; }
    public virtual Customer Customer { get; set; }
}
