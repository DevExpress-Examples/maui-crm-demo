using System.Collections.ObjectModel;

using DevExpress.Maui.Core;
using CrmDemo.DataLayer;

namespace CrmDemo.ViewModels.Common;

public abstract class CrmViewModelBase<TEntity> : BindableBase where TEntity : class {
    public ObservableCollection<TEntity> Items {
        get => items;
        protected set {
            items = value;
            RaisePropertyChanged(nameof(Items));
        }
    }
    public bool IsDataLoading {
        get => isDataLoading;
        set {
            isDataLoading = value;
            RaisePropertyChanged(nameof(IsDataLoading));
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
        Items = new ObservableCollection<TEntity>(list);
        OnLoadData(crmContext);

        IsDataLoading = false;
    }
    protected abstract IQueryable<TEntity> GetQueryable(CrmContext crmContext);
    protected virtual void OnLoadData(CrmContext crmContext) { }

    private CrmContext crmContext;
    private ObservableCollection<TEntity> items;
    private bool isDataLoading;
}
