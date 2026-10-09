namespace NeuroRotina.Views;

public partial class CabecalhoView : ContentView
{
    public static readonly BindableProperty TituloProperty =
        BindableProperty.Create(
            nameof(Titulo),
            typeof(string),
            typeof(CabecalhoView),
            string.Empty,
            propertyChanged: (bindable, antigo, novo) =>
                ((CabecalhoView)bindable).lblTitulo.Text = (string)novo);

    public static readonly BindableProperty MostrarVoltarProperty =
        BindableProperty.Create(
            nameof(MostrarVoltar),
            typeof(bool),
            typeof(CabecalhoView),
            true,
            propertyChanged: (bindable, antigo, novo) =>
                ((CabecalhoView)bindable).btnVoltar.IsVisible = (bool)novo);

    public string Titulo
    {
        get => (string)GetValue(TituloProperty);
        set => SetValue(TituloProperty, value);
    }

    public bool MostrarVoltar
    {
        get => (bool)GetValue(MostrarVoltarProperty);
        set => SetValue(MostrarVoltarProperty, value);
    }

    public CabecalhoView()
    {
        InitializeComponent();
    }

    private async void Voltar_Tapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
