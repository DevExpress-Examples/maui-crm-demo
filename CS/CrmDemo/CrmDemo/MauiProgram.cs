using CommunityToolkit.Maui;
using DevExpress.Maui;
using DevExpress.Maui.Core;
using DevExpress.XtraReports.Security;
using DevExpress.XtraReports.UI;
using CrmDemo.DataModel.Models;
using CrmDemo.ViewModels.Common;
using CrmDemo.ViewModels.Customers;
using CrmDemo.ViewModels.Employees;
using CrmDemo.ViewModels.Home;
using CrmDemo.ViewModels.Meetings;
using CrmDemo.ViewModels.Orders;
using CrmDemo.ViewModels.Products;
using CrmDemo.ViewModels.Settings;
using CrmDemo.Views;
using CrmDemo.Views.Customers;
using CrmDemo.Views.Products;
using SkiaSharp.Views.Maui.Controls.Hosting;
using CrmDemo.Helpers;

namespace CrmDemo {
    public static class MauiProgram {
        public static MauiApp CreateMauiApp() {
            ThemeManager.ApplyThemeToSystemBars = true;
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .RegisterViewModels()
                .RegisterViews()
                .RegisterAppServices()
                .UseDevExpress(useLocalization: true)
                .UseMauiCommunityToolkit()
                .UseSkiaSharp()
                .ConfigureFonts(fonts => {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("univia-pro-regular.ttf", "Univia-Pro");
                    fonts.AddFont("roboto-bold.ttf", "Roboto-Bold");
                    fonts.AddFont("roboto-regular.ttf", "Roboto");
                })
                .ConfigureMauiHandlers(handlers => {
                    handlers.AddHandler<Shell, CustomShellRenderer>();
                });
#if DEBUG
            DotNet.Meteor.HotReload.Plugin.BuilderExtensions.EnableHotReload(builder);
#endif
            DevExpress.Maui.Charts.Initializer.Init();
            DevExpress.Maui.CollectionView.Initializer.Init();
            DevExpress.Maui.DataGrid.Initializer.Init();
            DevExpress.Maui.Scheduler.Initializer.Init();
            DevExpress.Maui.Editors.Initializer.Init();
            DevExpress.Maui.Controls.Initializer.Init();

            DevExpress.Security.Resources.AccessSettings.ReportingSpecificResources.SetRules(SerializationFormatRule.Allow(SerializationFormat.Code, SerializationFormat.Xml));
            DevExpress.Maui.Core.Localizer.StringLoader = new StringLoader();

            RegisterReportTrustedTypes();

            return builder.Build();
        }
        public static void RegisterReportTrustedTypes() {
            DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Order));
        }
        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder) {
            mauiAppBuilder.Services.AddTransient<HomeViewModel>();
            mauiAppBuilder.Services.AddTransient<CustomersViewModel>();
            mauiAppBuilder.Services.AddTransient<EmployeesViewModel>();
            mauiAppBuilder.Services.AddTransient<OrdersViewModel>();
            mauiAppBuilder.Services.AddTransient<MeetingsViewModel>();
            mauiAppBuilder.Services.AddTransient<ProductsViewModel>();
            mauiAppBuilder.Services.AddTransient<SettingsViewModel>();
            return mauiAppBuilder;
        }
        public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder) {
            mauiAppBuilder.Services.AddTransient<HomePage>();
            mauiAppBuilder.Services.AddTransient<CustomersPage>();
            mauiAppBuilder.Services.AddTransient<EmployeesPage>();
            mauiAppBuilder.Services.AddTransient<OrdersPage>();
            mauiAppBuilder.Services.AddTransient<MeetingsPage>();
            mauiAppBuilder.Services.AddTransient<ProductsPage>();
            mauiAppBuilder.Services.AddTransient<SettingsPage>();
            return mauiAppBuilder;
        }
        public static MauiAppBuilder RegisterAppServices(this MauiAppBuilder mauiAppBuilder) {
            mauiAppBuilder.Services.AddTransient<UserSessionService>();
            return mauiAppBuilder;
        }
    }
}