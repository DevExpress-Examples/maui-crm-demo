namespace CrmDemo.Views.Dashboards;

public partial class InfoIconElement : ContentView {
    public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(string), typeof(InfoIconElement));
    public static readonly BindableProperty DescriptionProperty = BindableProperty.Create(nameof(Description), typeof(string), typeof(InfoIconElement));
    public static readonly BindableProperty ImageProperty = BindableProperty.Create(nameof(Image), typeof(ImageSource), typeof(InfoIconElement));
    public static readonly BindableProperty ImageColorProperty = BindableProperty.Create(nameof(ImageColor), typeof(Color), typeof(InfoIconElement),
        propertyChanged: (o, _, _) => ((InfoIconElement)o).OnImageColorChanged());

    public string Value {
        get { return (string)GetValue(ValueProperty); }
        set { SetValue(ValueProperty, value); }
    }
    public string Description {
        get { return (string)GetValue(DescriptionProperty); }
        set { SetValue(DescriptionProperty, value); }
    }
    public ImageSource Image {
        get { return (ImageSource)GetValue(ImageProperty); }
        set { SetValue(ImageProperty, value); }
    }
    public Color ImageColor {
        get { return (Color)GetValue(ImageColorProperty); }
        set { SetValue(ImageColorProperty, value); }
    }

    public InfoIconElement() {
        InitializeComponent();
        rootPanel.BindingContext = this;
    }

    private void OnImageColorChanged() {
        image.TintColor = ImageColor;
    }
}