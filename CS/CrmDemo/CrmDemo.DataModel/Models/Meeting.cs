using System.Collections.ObjectModel;

namespace CrmDemo.DataModel.Models;

public class Meeting {
    private ObservableCollection<Employee> employees;

    public int Id { get; set; }
    public string Subject { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool AllDay { get; set; }
    public virtual ObservableCollection<Employee> Employees {
        get {
            if (employees == null) {
                employees = new ObservableCollection<Employee>();
            }
            return employees;
        }
        set { employees = value; }
    }
}
