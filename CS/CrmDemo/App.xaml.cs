using Microsoft.EntityFrameworkCore;

using CrmDemo.Helpers;
using CrmDemo.DataLayer;

namespace CrmDemo;

public partial class App : Application {
    public const string DbFileName = "devexpress_crm_database.db";
    public static string DbFullPath {
        get => Path.Combine(FileSystem.AppDataDirectory, DbFileName);
    }
    public App() {
        InitializeComponent();
        EnsureDbFile();
        MainPage = new AppShell();
        UpdateDbAsync();
    }

    private void EnsureDbFile() {
        FileHelper.EnsureAssetInAppDataAsync(DbFileName).Wait();
    }
    private void UpdateOrders(CrmContext crmContext) {
        var lastOrder =
            crmContext.Orders.Skip(crmContext.Orders.Count() - 50)
            .ToList()
            .MaxBy(x => x.OrderDate);
        if (lastOrder != null) {
            var days = (DateTime.Today - TimeSpan.FromDays(1) - lastOrder.OrderDate.Date).TotalDays;
            if (days > 0) {
                crmContext.Orders.Load();
                crmContext.Orders.Local.ToList().ForEach(x => {
                    x.OrderDate = x.OrderDate.AddDays(days);
                });
            }
        }
    }
    private void UpdateMeetings(CrmContext crmContext) {
        crmContext.Meetings.Load();
        var meetings = crmContext.Meetings.Local.ToList();
        var firstMeeting = meetings.MinBy(x => x.StartTime);
        if (firstMeeting == null)
            return;
        var days = (DateTime.Today - firstMeeting.StartTime.Date).TotalDays;
        if (days > 0) {
            meetings.ForEach(x => {
                x.StartTime = x.StartTime.AddDays(days);
                x.EndTime = x.EndTime.AddDays(days);
            });
        }
    }
    private Task UpdateDbAsync() {
        return Task.Run(() => UpdateDb());
    }
    private void UpdateDb() {
        using (CrmContext crmContext = new CrmContext()) {
            UpdateOrders(crmContext);
            UpdateMeetings(crmContext);
            crmContext.SaveChanges();
        }
    }
}