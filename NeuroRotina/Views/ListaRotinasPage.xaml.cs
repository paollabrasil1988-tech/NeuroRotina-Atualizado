using NeuroRotina.Data;
using NeuroRotina.Models;
using NeuroRotina.Services;

namespace NeuroRotina.Views;

[QueryProperty(nameof(CriancaId), "criancaId")]
public partial class ListaRotinasPage : ContentPage
{
    private readonly DatabaseService _database;

    public int CriancaId { get; set; }

    public ListaRotinasPage()
    {
        InitializeComponent();

        _database = new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await CarregarRotinasAsync();
    }

    private static Style Estilo(string chave) =>
        (Style)Application.Current!.Resources[chave];

    private async Task CarregarRotinasAsync()
    {
        ListaRotinas.Children.Clear();

        List<Rotina> rotinas =
            await _database.BuscarRotinasPorCriancaAsync(CriancaId);

        if (rotinas.Count == 0)
        {
            ListaRotinas.Children.Add(
                new Label
                {
                    Text = "Nenhuma rotina cadastrada.",
                    FontSize = 18,
                    HorizontalOptions = LayoutOptions.Center
                });

            return;
        }

        List<Conclusao> feitasHoje =
            await _database.BuscarConclusoesDoDiaAsync(
                CriancaId,
                Util.DataIso(DateTime.Today));

        foreach (Rotina rotina in rotinas.OrderBy(r => r.Horario))
        {
            bool feita = feitasHoje.Any(c =>
                c.RotinaId == rotina.Id && c.AtividadeId == 0);

            var cartao = new Border
            {
                Style = Estilo("Cartao")
            };

            var layout = new VerticalStackLayout
            {
                Spacing = 8
            };

            layout.Children.Add(new Label
            {
                Text = Util.HoraBr(rotina.Horario),
                FontSize = 30,
                FontFamily = "serif",
                TextColor = Cores.Azul
            });

            layout.Children.Add(new Label
            {
                Text = rotina.Titulo.ToUpper(),
                FontSize = 22,
                FontAttributes = FontAttributes.Bold,
                TextColor = Cores.Texto
            });

            if (!string.IsNullOrWhiteSpace(rotina.Descricao))
            {
                layout.Children.Add(new Label
                {
                    Text = rotina.Descricao,
                    FontSize = 16,
                    TextColor = Cores.Texto
                });
            }

            layout.Children.Add(new Label
            {
                Text = Util.DescreverDias(rotina.DiasSemana),
                FontSize = 15,
                TextColor = Colors.Gray
            });

            layout.Children.Add(new Label
            {
                Text = rotina.AlertaAtivo
                    ? $"🔔 Alerta ligado ({SomPorExtenso(rotina.Som)})"
                    : "🔕 Sem alerta sonoro",
                FontSize = 15,
                TextColor = rotina.AlertaAtivo ? Cores.Azul : Colors.Gray
            });

            layout.Children.Add(new Label
            {
                Text = feita ? "✔ Feita hoje" : "Ainda não feita hoje",
                FontSize = 15,
                FontAttributes = FontAttributes.Bold,
                TextColor = feita ? Cores.Verde : Colors.Gray
            });

            var botaoEditar = new Button
            {
                Text = "Editar rotina",
                Style = Estilo("BotaoPrimario"),
                HeightRequest = 50,
                FontSize = 18
            };

            botaoEditar.Clicked += async (sender, args) =>
            {
                await Shell.Current.GoToAsync(
                    $"{nameof(RotinaPage)}?criancaId={CriancaId}&rotinaId={rotina.Id}");
            };

            var botaoAtividade = new Button
            {
                Text = "Adicionar atividade",
                Style = Estilo("BotaoSecundario"),
                HeightRequest = 48,
                FontSize = 17
            };

            botaoAtividade.Clicked += async (sender, args) =>
            {
                await Shell.Current.GoToAsync(
                    $"{nameof(AtividadePage)}?rotinaId={rotina.Id}&rotinaTitulo={Uri.EscapeDataString(rotina.Titulo)}");
            };

            var botaoVerAtividades = new Button
            {
                Text = "Ver atividades",
                Style = Estilo("BotaoSecundario"),
                HeightRequest = 48,
                FontSize = 17
            };

            botaoVerAtividades.Clicked += async (sender, args) =>
            {
                await Shell.Current.GoToAsync(
                    $"{nameof(ListaAtividadesPage)}?rotinaId={rotina.Id}");
            };

            var botaoExcluir = new Button
            {
                Text = "Excluir rotina",
                Style = Estilo("BotaoPerigo")
            };

            botaoExcluir.Clicked += async (sender, args) =>
            {
                bool confirmou = await DisplayAlert(
                    "Excluir rotina",
                    $"Excluir \"{rotina.Titulo}\", suas atividades e o alerta?",
                    "Excluir",
                    "Cancelar");

                if (!confirmou)
                {
                    return;
                }

                AlarmeService.Cancelar(rotina.Id);
                await _database.ExcluirRotinaAsync(rotina.Id);
                await CarregarRotinasAsync();
            };

            layout.Children.Add(botaoEditar);
            layout.Children.Add(botaoAtividade);
            layout.Children.Add(botaoVerAtividades);
            layout.Children.Add(botaoExcluir);

            cartao.Content = layout;

            ListaRotinas.Children.Add(cartao);
        }
    }

    private static string SomPorExtenso(string som) => som switch
    {
        "suave" => "aviso suave",
        "vibrar" => "só vibrar",
        _ => "alarme"
    };
}
