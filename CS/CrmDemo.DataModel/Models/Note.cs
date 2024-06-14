namespace CrmDemo.DataModel.Models;

public class Note {
    public virtual Employee Owner { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
}
