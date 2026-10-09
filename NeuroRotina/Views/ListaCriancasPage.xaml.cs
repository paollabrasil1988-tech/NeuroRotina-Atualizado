using NeuroRotina.Data;
using NeuroRotina.Models;

namespace NeuroRotina.Views;

public partial class ListaCriancasPage : ContentPage
{
    private readonly DatabaseService _database;

    public ListaCriancasPage()
    {
        InitializeComponent();

        _database = new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await CarregarCriancasAsync();
    }

    private async Task CarregarCriancasAsync()
    {
        ListaCriancas.Children.Clear();

        if (SessaoUsuario.UsuarioId == 0)
        {
            ListaCriancas.Children.Add(
                new Label
                {
                    Text = "Nenhum adulto está logado.",
                    FontSize = 18,
                    HorizontalOptions = LayoutOptions.Center
                });

            return;
        }

        List<Crianca> criancas =
            await _database.BuscarCriancasPorUsuarioAsync(
                SessaoUsuario.UsuarioId);

        if (criancas.Count == 0)
        {
            ListaCriancas.Children.Add(
                new Label
                {
                    Text = "Nenhuma criança cadastrada.",
                    FontSize = 18,
                    HorizontalOptions = LayoutOptions.Center
                });

            return;
        }

        foreach (Crianca crianca in criancas)
        {
            var cartao = new Border
            {
                Style = (Style)Application.Current!.Resources["Cartao"]
            };

            var layout = new VerticalStackLayout
            {
                Spacing = 10
            };

            layout.Children.Add(new Label
            {
                Text = crianca.Nome,
                FontSize = 24,
                FontFamily = "serif",
                TextColor = Services.Cores.Azul
            });

            if (!string.IsNullOrWhiteSpace(crianca.NivelSuporte))
            {
                layout.Children.Add(new Label
                {
                    Text = crianca.NivelSuporte,
                    FontSize = 15,
                    TextColor = Colors.Gray
                });
            }

            var botaoRotinas = new Button
            {
                Text = "Rotinas e alertas",
                Style = (Style)Application.Current!.Resources["BotaoPrimario"]
            };

            botaoRotinas.Clicked += async (sender, args) =>
            {
                await Shell.Current.GoToAsync(
                    $"{nameof(RotinaPage)}?criancaId={crianca.Id}");
            };

            var botaoHistorico = new Button
            {
                Text = "Histórico diário",
                Style = (Style)Application.Current!.Resources["BotaoSecundario"]
            };

            botaoHistorico.Clicked += async (sender, args) =>
            {
                await Shell.Current.GoToAsync(
                    $"{nameof(HistoricoPage)}?criancaId={crianca.Id}");
            };

            layout.Children.Add(botaoRotinas);
            layout.Children.Add(botaoHistorico);

            cartao.Content = layout;

            ListaCriancas.Children.Add(cartao);
        }
    }

    private async void BtnNovaCrianca_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CriancaPage));
    }
}
