namespace NeuroRotina.Views;

public partial class LogoView : ContentView
{
    public static readonly BindableProperty TamanhoProperty =
        BindableProperty.Create(
            nameof(Tamanho),
            typeof(double),
            typeof(LogoView),
            160d,
            propertyChanged: (bindable, antigo, novo) =>
            {
                var logo = (LogoView)bindable;
                logo.areaLogo.WidthRequest = (double)novo;
                logo.areaLogo.HeightRequest = (double)novo;
            });

    public double Tamanho
    {
        get => (double)GetValue(TamanhoProperty);
        set => SetValue(TamanhoProperty, value);
    }

    public LogoView()
    {
        InitializeComponent();
    }
}
