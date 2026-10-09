using NeuroRotina.Data;
using NeuroRotina.Models;
using NeuroRotina.Services;

namespace NeuroRotina.Views;

[QueryProperty(nameof(CriancaId), "criancaId")]
public partial class HistoricoPage : ContentPage
{
    private readonly DatabaseService _database;

    public int CriancaId { get; set; }

    public HistoricoPage()
    {
        InitializeComponent();

        _database = new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await CarregarAsync();
    }

    private static Style Estilo(string chave) =>
        (Style)Application.Current!.Resources[chave];

    // Cria um texto no formato do Figma: "Rótulo:" em negrito seguido do valor.
    private static Label LinhaCampo(string rotulo, string valor)
    {
        var texto = new FormattedString();

        texto.Spans.Add(new Span
        {
            Text = rotulo + " ",
            FontAttributes = FontAttributes.Bold
        });

        texto.Spans.Add(new Span
        {
            Text = valor
        });

        return new Label
        {
            FormattedText = texto,
            FontSize = 16,
            LineHeight = 1.3,
            TextColor = Cores.Texto
        };
    }

    private async Task CarregarAsync()
    {
        ListaRegistros.Children.Clear();

        DateTime hoje = DateTime.Today;

        ListaRegistros.Children.Add(await CriarCartaoHojeAsync(hoje));

        List<RegistroDiario> registros =
            await _database.BuscarRegistrosPorCriancaAsync(CriancaId);

        if (registros.Count == 0)
        {
            ListaRegistros.Children.Add(new Label
            {
                Text = "Nenhum registro ainda. Toque em \"Novo registro\" para anotar como foi o dia.",
                FontSize = 16,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Cores.Texto
            });

            return;
        }

        foreach (RegistroDiario registro in registros)
        {
            var cartao = new Border
            {
                Style = Estilo("Cartao")
            };

            var layout = new VerticalStackLayout
            {
                Spacing = 6
            };

            layout.Children.Add(LinhaCampo("Data:", Util.DataBr(registro.Data)));
            layout.Children.Add(LinhaCampo("Comportamento:", registro.Comportamento));
            layout.Children.Add(LinhaCampo("Observação:", registro.Observacao));

            var botaoExcluir = new Button
            {
                Text = "Excluir registro",
                Style = Estilo("BotaoPerigo"),
                HeightRequest = 42,
                FontSize = 15,
                Margin = new Thickness(0, 8, 0, 0)
            };

            botaoExcluir.Clicked += async (sender, args) =>
            {
                bool confirmou = await DisplayAlert(
                    "Excluir registro",
                    "Deseja excluir este registro?",
                    "Excluir",
                    "Cancelar");

                if (!confirmou)
                {
                    return;
                }

                await _database.ExcluirRegistroAsync(registro.Id);
                await CarregarAsync();
            };

            layout.Children.Add(botaoExcluir);

            cartao.Content = layout;

            ListaRegistros.Children.Add(cartao);
        }
    }

    // Cartão automático com o que aconteceu hoje (rotinas feitas e pedidos de necessidades).
    private async Task<Border> CriarCartaoHojeAsync(DateTime hoje)
    {
        int diaSemana = (int)hoje.DayOfWeek;

        List<Rotina> rotinas =
            await _database.BuscarRotinasPorCriancaAsync(CriancaId);

        List<Rotina> deHoje = rotinas
            .Where(r => Util.ContemDia(r.DiasSemana, diaSemana))
            .ToList();

        List<Conclusao> feitas =
            await _database.BuscarConclusoesDoDiaAsync(
                CriancaId,
                Util.DataIso(hoje));

        int rotinasFeitas = deHoje.Count(r =>
            feitas.Any(c => c.RotinaId == r.Id && c.AtividadeId == 0));

        string resumoPedidos =
            await _database.MontarResumoPedidosAsync(CriancaId, hoje);

        var cartao = new Border
        {
            Style = Estilo("Cartao"),
            Stroke = Cores.Azul,
            StrokeThickness = 3
        };

        var layout = new VerticalStackLayout
        {
            Spacing = 6
        };

        layout.Children.Add(LinhaCampo("Hoje:", Util.DataBr(hoje)));

        layout.Children.Add(LinhaCampo(
            "Rotinas:",
            deHoje.Count == 0
                ? "nenhuma rotina para hoje."
                : $"{rotinasFeitas} de {deHoje.Count} feitas."));

        layout.Children.Add(LinhaCampo(
            "Comunicação:",
            string.IsNullOrEmpty(resumoPedidos)
                ? "nenhum pedido hoje."
                : resumoPedidos));

        cartao.Content = layout;

        return cartao;
    }

    private async void BtnNovoRegistro_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(
            $"{nameof(RegistroDiarioPage)}?criancaId={CriancaId}");
    }
}
