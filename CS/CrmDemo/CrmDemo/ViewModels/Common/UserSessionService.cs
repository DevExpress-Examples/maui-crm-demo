using CrmDemo.DataLayer;
using CrmDemo.DataModel.Models;

namespace CrmDemo.ViewModels.Common;

public class UserSessionService : IDisposable {
    

    public Employee CurrentUser { get; private set; }
    public int CurrentUserId { get; private set; }

    public UserSessionService() {
        crmContext = new CrmContext();
        CurrentUser = crmContext.Employees.First();
        CurrentUserId = CurrentUser.Id;
    }
    public void Dispose() {
        if (crmContext != null) {
            crmContext.Dispose();
            crmContext = null;
        }
    }

    

    private CrmContext crmContext;
}
