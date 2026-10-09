using NeuroRotina.Data;
using NeuroRotina.Models;

namespace NeuroRotina.Views;

[QueryProperty(nameof(RotinaId), "rotinaId")]
public partial class ListaAtividadesPage : ContentPage
{
    private readonly DatabaseService _database;

    public int RotinaId { get; set; }

    public ListaAtividadesPage()
    {
        InitializeComponent();

        _database = new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await CarregarAtividadesAsync();
    }

    private async Task CarregarAtividadesAsync()
    {
        ListaAtividades.Children.Clear();

        List<Atividade> atividades =
            await _database.BuscarAtividadesPorRotinaAsync(RotinaId);

        if (atividades.Count == 0)
        {
            ListaAtividades.Children.Add(
                new Label
                {
                    Text = "Nenhuma atividade cadastrada.",
                    FontSize = 18,
                    HorizontalOptions = LayoutOptions.Center
                });

            return;
        }

        foreach (Atividade atividade in atividades)
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
                Text = atividade.Descricao,
                FontSize = 18,
                TextColor = Services.Cores.Texto
            });

            var botaoExcluir = new Button
            {
                Text = "Excluir atividade",
                Style = (Style)Application.Current!.Resources["BotaoPerigo"]
            };

            botaoExcluir.Clicked += async (sender, args) =>
            {
                bool confirmou = await DisplayAlert(
                    "Excluir atividade",
                    "Deseja excluir esta atividade?",
                    "Excluir",
                    "Cancelar");

                if (!confirmou)
                {
                    return;
                }

                await _database.ExcluirAtividadeAsync(atividade.Id);
                await CarregarAtividadesAsync();
            };

            layout.Children.Add(botaoExcluir);

            cartao.Content = layout;

            ListaAtividades.Children.Add(cartao);
        }
    }
}
