using System;
using System.Globalization;
using System.Threading.Tasks;
using DevExpress.Maui.Editors;
using Microsoft.Maui.Controls;

namespace CrmDemo.Views.Customers;

public partial class ImageEditView : ContentPage {
    private TaskCompletionSource<byte[]> pageResultCompletionSource;

    public ImageEditView() {
        InitializeComponent();
    }

    public ImageEditView(ImageSource imageSource) {
        InitializeComponent();
        pageResultCompletionSource = new TaskCompletionSource<byte[]>();
        editor.Source = imageSource;
    }

    public Task<byte[]> WaitForResultAsync() {
        return pageResultCompletionSource.Task;
    }

    async void BackPressed(object sender, EventArgs e) {
        pageResultCompletionSource.SetResult(null);
        await Navigation.PopAsync();
    }

    async void CropPressed(object sender, EventArgs e) {
        using (MemoryStream stream = new MemoryStream()) {
            editor.SaveAsStream(stream, DevExpress.Maui.Editors.ImageFormat.Jpeg);
            stream.Position = 0;
            pageResultCompletionSource.SetResult(stream.ToArray());
        }
        await Navigation.PopAsync();
    }
}

public class FrameTypeToImageStringConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return (CropAreaShape)value == CropAreaShape.Ellipse ? "ic_frame_rect" : "ic_frame_circle";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}