using Microsoft.EntityFrameworkCore;

using CrmDemo.DataModel.Models;

namespace CrmDemo.DataLayer;

public class CrmContext : DbContext {
    private static int crmContextId;

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Meeting> Meetings { get; set; }
    public DbSet<CheckListItem> CheckListItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<CrmImage> ImageObjects { get; set; }

    public int Id { get; set; }

    public CrmContext() {
        SQLitePCL.Batteries_V2.Init();
        Database.EnsureCreated();
        crmContextId++;
        Id = crmContextId;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, App.DbFileName);
        optionsBuilder.UseLazyLoadingProxies().UseSqlite($"Filename={dbPath}");
        base.OnConfiguring(optionsBuilder);
    }
}