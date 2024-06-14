using CrmDemo.DataLayer;
using CrmDemo.DataModel.Models;

namespace CrmDemo.ViewModels.Common;

public class UserSessionService {
    private static int currentUserId;
    private static string currentUserFullName;
    private static void LoadData() {
        if (currentUserId == 0) {
            using (CrmContext crmContext = new CrmContext()) {
                Employee currentUser = crmContext.Employees.First();
                currentUserId = currentUser.Id;
                currentUserFullName = currentUser.FullName;
            }
        }
    }

    public int CurrentUserId {
        get {
            LoadData();
            return currentUserId;
        }
    }
    public string CurrentUserFullName {
        get {
            LoadData();
            return currentUserFullName;
        }
    }
    public UserSessionService() {
    }
}
