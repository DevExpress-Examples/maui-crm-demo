using System.Windows.Input;

namespace CrmDemo.ViewModels.Dashboards;

public class DashboardsViewModel {
    public ICommand GoToOrdersByEmployeeCommand { get; set; }
    public ICommand GoToOrdersByMonthCommand { get; set; }
    public ICommand GoToOrdersByStateCommand { get; set; }
    public ICommand GoToOrderStateEvolutionCommand { get; set; }
    public ICommand GoToOrdersByCountryCommand { get; set; }
    public ICommand GoToSalesTeamProductivityCommand { get; set; }
    public DashboardsViewModel() {
        GoToOrdersByEmployeeCommand = new Command(GoToOrdersByEmployee);
        GoToOrdersByMonthCommand = new Command(GoToOrdersByMonth);
        GoToOrdersByStateCommand = new Command(GoToOrdersByState);
        GoToOrderStateEvolutionCommand = new Command(GoToOrderStateEvolution);
        GoToOrdersByCountryCommand = new Command(GoToOrdersByCountry);
        GoToSalesTeamProductivityCommand = new Command(GoToSalesTeamProductivity);
    }

    private async void GoToOrdersByEmployee() {
        await Shell.Current.GoToAsync("ordersByEmployeeDashboard");
    }
    private void GoToOrdersByMonth() {
        Shell.Current.GoToAsync("ordersByMonthDashboard");
    }
    private void GoToOrdersByState() {
        Shell.Current.GoToAsync("ordersByStateDashboard");
    }

    private void GoToOrderStateEvolution() {
        Shell.Current.GoToAsync("orderStateEvolutionDashboard");
    }
    private void GoToOrdersByCountry() {
        Shell.Current.GoToAsync("ordersByCountryDashboard");
    }
    private void GoToSalesTeamProductivity() {
        Shell.Current.GoToAsync("priceSoldItemsRelationDashboard");
    }
}