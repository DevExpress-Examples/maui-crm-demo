﻿using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

using DevExpress.Maui.Core;
using CrmDemo.DataLayer;
using CrmDemo.DataModel.Models;

namespace CrmDemo.ViewModels.Dashboards;

public class ProductSalesInfo {
    public string ProductName { get; set; }
    public int SoldItems { get; set; }
    public decimal Price { get; set; }
    public decimal Revenue { get; private set; }
    public ProductSalesInfo(string productName, decimal price) {
        ProductName = productName;
        Price = price;
    }
    public void AddSoldItems(int soldItems) {
        SoldItems = SoldItems + soldItems;
        Revenue = SoldItems * Price;
    }
}

public class PriceSoldItemsRelationViewModel : BindableBase {
    private ObservableCollection<ProductSalesInfo> dataItems;

    public ObservableCollection<ProductSalesInfo> DataItems {
        get => dataItems;
        set {
            dataItems = value;
            RaisePropertyChanged(nameof(DataItems));
        }
    }
    public PriceSoldItemsRelationViewModel() {
        LoadDataAsync();
    }

    private void LoadData() {
        using (CrmContext crmContext = new CrmContext()) {
            Dictionary<string, ProductSalesInfo> data = new Dictionary<string, ProductSalesInfo>();
            List<OrderItem> orderItems = crmContext.OrderItems.Include(i => i.Product).ToList();
            List<ProductSalesInfo> dataItems = new List<ProductSalesInfo>();
            foreach (OrderItem orderItem in orderItems) {
                string dataItemKey = orderItem.Product.Name;
                ProductSalesInfo dataItem = null;
                if (!data.TryGetValue(dataItemKey, out dataItem)) {
                    dataItem = new ProductSalesInfo(orderItem.Product.Name, orderItem.Product.UnitPrice);
                    data.Add(dataItemKey, dataItem);
                }
                dataItem.AddSoldItems(orderItem.Quantity);
            }
            DataItems = new ObservableCollection<ProductSalesInfo>(data.Values.ToList());
        }
    }
    private Task LoadDataAsync() {
        return Task.Run(() => LoadData());
    }
}
