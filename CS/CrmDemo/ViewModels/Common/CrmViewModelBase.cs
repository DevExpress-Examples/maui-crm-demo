using System.Collections.ObjectModel;
using DevExpress.Maui.Mvvm;
using CrmDemo.DataLayer;
using DevExpress.Data.Filtering;

namespace CrmDemo.ViewModels.Common;

public abstract class CrmViewModelBase<TEntity> : DXObservableObject where TEntity : class {
    private CrmContext crmContext;
    private ObservableCollection<TEntity> items;
    private bool isDataLoading;

    public CriteriaOperator AssignedToMeFilterExpression => CriteriaOperator.Parse($"[Employee.FullName] = '{SessionService.CurrentUserFullName}'");
    public ObservableCollection<TEntity> Items {
        get => items;
        protected set {
            items = value;
            OnPropertyChanged(nameof(Items));
        }
    }
    public bool IsDataLoading {
        get => isDataLoading;
        set {
            isDataLoading = value;
            OnPropertyChanged(nameof(IsDataLoading));
        }
    }
    public CrmViewModelBase(UserSessionService sessionService) {
        SessionService = sessionService;
    }
    public Task LoadDataAsync() {
        return Task.Run(() => LoadData());
    }

    protected internal CrmContext CrmContext { get => crmContext; }
    protected UserSessionService SessionService;
    protected Comparison<TEntity> sortComparison;
    protected void LoadData() {
        IsDataLoading = true;

        if (crmContext != null) {
            crmContext.Dispose();
        }
        crmContext = new CrmContext();

        List<TEntity> list = GetQueryable(crmContext).ToList();
        if (sortComparison != null) {
            list.Sort(sortComparison);
        }
        OnApplyData(list);
        OnLoadData(crmContext);

        IsDataLoading = false;
    }
    protected virtual void OnApplyData(List<TEntity> list) {
        Items = new ObservableCollection<TEntity>(list);
    }
    protected abstract IQueryable<TEntity> GetQueryable(CrmContext crmContext);
    protected virtual void OnLoadData(CrmContext crmContext) { }
}
