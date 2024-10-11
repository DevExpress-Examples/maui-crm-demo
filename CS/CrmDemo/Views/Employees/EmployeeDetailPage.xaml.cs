using DevExpress.Maui.Core;
using DevExpress.Maui.Core.Internal;
using CrmDemo.DataModel.Models;

namespace CrmDemo.Views.Employees;

public partial class EmployeeDetailPage : ContentPage {
    private Person Item => (Person)(BindingContext as DetailFormViewModel).Item;

    public EmployeeDetailPage() {
        InitializeComponent();

    }

    protected override void OnAppearing() {
        base.OnAppearing();
        col.Palette = new[] {
            ThemeManager.Theme.Scheme.Primary,
            ThemeManager.Theme.Scheme.Primary.OverrideAlpha(0.9),
            ThemeManager.Theme.Scheme.Primary.OverrideAlpha(0.8),
            ThemeManager.Theme.Scheme.Primary.OverrideAlpha(0.7),
            ThemeManager.Theme.Scheme.Primary.OverrideAlpha(0.6),
            ThemeManager.Theme.Scheme.Primary.OverrideAlpha(0.5),
        };
    }

    async void MessageClick(object sender, EventArgs e) {
        if (Sms.Default.IsComposeSupported) {
            string[] recipients = new[] { Item.Phone };

            var message = new SmsMessage(string.Empty, recipients);

            await Sms.Default.ComposeAsync(message);
        }
    }
    void CallClick(object sender, EventArgs e) {
        if (PhoneDialer.Default.IsSupported && !String.IsNullOrEmpty(Item.Phone))
            PhoneDialer.Default.Open(Item.Phone);
    }
    async void MailClick(object sender, EventArgs e) {
        if (Email.Default.IsComposeSupported) {
            string[] recipients = new[] { Item.Email };

            var message = new EmailMessage {
                BodyFormat = EmailBodyFormat.PlainText,
                To = recipients.ToList()
            };

            await Email.Default.ComposeAsync(message);
        }
    }
    async void CopyPhoneClick(object sender, EventArgs e) {
        await Clipboard.Default.SetTextAsync(Item.Phone);
    }
    async void CopyEmailClick(object sender, EventArgs e) {
        await Clipboard.Default.SetTextAsync(Item.Email);
    }
}