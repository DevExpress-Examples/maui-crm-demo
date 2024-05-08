using System.Collections.ObjectModel;

using DevExpress.Maui.Core;

using CrmDemo.DataLayer;
using CrmDemo.DataModel.Models;

namespace CrmDemo.ViewModels.Dashboards;

public class OrdersByStateViewModel : BindableBase {
    private CrmContext crmContext;
    private List<Order> allOrders;
    private TrafficChannel selectedTrafficChannel;
    private ObservableCollection<Order> segmentOrders;

    public ObservableCollection<TrafficChannel> TrafficChannelsData { get; set; }
    public Color[] TrafficColors { get; set; }
    public TrafficChannel SelectedTrafficChannel {
        get => selectedTrafficChannel;
        set {
            selectedTrafficChannel = value;
            UpdateSegmentOrders();
        }
    }
    public ObservableCollection<Order> SegmentOrders {
        get => segmentOrders;
        set {
            segmentOrders = value;
            RaisePropertyChanged(nameof(SegmentOrders));
        }
    }
    public OrdersByStateViewModel() {
        crmContext = new CrmContext();
        allOrders = crmContext.Orders.ToList();
        TrafficChannelsData = new ObservableCollection<TrafficChannel>() {
            new TrafficChannel("Pending", allOrders.Where(o => o.State == OrderState.Pending).ToList()),
            new TrafficChannel("Shipping", allOrders.Where(o => o.State == OrderState.Shipping).ToList()),
            new TrafficChannel("Paid", allOrders.Where(o => o.State == OrderState.Paid).ToList()),
            new TrafficChannel("Processed",  allOrders.Where(o => o.State == OrderState.Processed).ToList())
        };
        TrafficColors = new Color[] {
            Color.FromArgb("#6085BE"),
            Color.FromArgb("#E1AA58"),
            Color.FromArgb("#CA5252"),
            Color.FromArgb("#6AAC78"),
        };
        UpdateSegmentOrders();
    }

    private void UpdateSegmentOrders() {
        SegmentOrders = new ObservableCollection<Order>((selectedTrafficChannel != null) ? selectedTrafficChannel.Orders : allOrders);
    }
}


public class TrafficChannel {
    public string ChannelName { get; }
    public IList<Order> Orders;
    public double Value { get; }

    public TrafficChannel(string channelName, IList<Order> orders) {
        ChannelName = channelName;
        Orders = orders;
        Value = orders.Count;
    }
}