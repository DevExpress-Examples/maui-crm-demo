using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace CrmDemo.DataModel.Models;

public abstract class Person : INotifyPropertyChanged {
    public int Id { get; set; }
    [Required(ErrorMessage = "First Name cannot be empty")]
    public string FirstName { get; set; }
    [Required(ErrorMessage = "Last Name cannot be empty")]
    public string LastName { get; set; }
    public virtual CrmImage Avatar {
        get => avatar;
        set {
            if (avatar != value) {
                this.avatar = value;
                OnPropertyChanged();
            }
        }
    }
    public virtual Address Address { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    [NotMapped]
    public string FullName { get => $"{FirstName} {LastName}"; }
    [NotMapped]
    public string City => Address?.City;

    private CrmImage avatar;

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "") {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}

public class Address {
    public int Id { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string Street { get; set; }
    public string ZipCode { get; set; }
}
