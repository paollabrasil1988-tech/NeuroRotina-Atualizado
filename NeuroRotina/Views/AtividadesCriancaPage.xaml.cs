using NeuroRotina.Data;
using NeuroRotina.Models;
using NeuroRotina.Services;

namespace NeuroRotina.Views;

public partial class AtividadesCriancaPage : ContentPage
{
    private readonly DatabaseService _database;

    public AtividadesCriancaPage()
    {
        InitializeComponent();

        _database = new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await CarregarAtividadesAsync();
    }

    private static Style Estilo(string chave) =>
        (Style)Application.Current!.Resources[chave];

    private async Task CarregarAtividadesAsync()
    {
        ListaAtividades.Children.Clear();

        DateTime hoje = DateTime.Today;
        int diaSemana = (int)hoje.DayOfWeek;
        string dataIso = Util.DataIso(hoje);

        lblHoje.Text = $"Hoje é {Util.DiasLongos[diaSemana]}";

        if (SessaoCrianca.CriancaId == 0)
        {
            ListaAtividades.Children.Add(
                new Label
                {
                    Text = "NENHUMA CRIANÇA LOGADA.",
                    FontSize = 20,
                    HorizontalOptions = LayoutOptions.Center
                });

            return;
        }

        List<Rotina> todas =
            await _database.BuscarRotinasPorCriancaAsync(
                SessaoCrianca.CriancaId);

        // Só aparecem as rotinas marcadas para o dia de hoje, em ordem de horário.
        List<Rotina> rotinasDeHoje = todas
            .Where(r => Util.ContemDia(r.DiasSemana, diaSemana))
            .OrderBy(r => r.Horario)
            .ToList();

        if (rotinasDeHoje.Count == 0)
        {
            ListaAtividades.Children.Add(
                new Label
                {
                    Text = "NADA POR ENQUANTO. APROVEITE O DIA!",
                    FontSize = 22,
                    HorizontalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center
                });

            return;
        }

        List<Conclusao> feitas =
            await _database.BuscarConclusoesDoDiaAsync(
                SessaoCrianca.CriancaId,
                dataIso);

        foreach (Rotina rotina in rotinasDeHoje)
        {
            List<Atividade> atividades =
                await _database.BuscarAtividadesPorRotinaAsync(rotina.Id);

            ListaAtividades.Children.Add(
                CriarCartaoRotina(rotina, atividades, feitas, dataIso));
        }
    }

    private Border CriarCartaoRotina(
        Rotina rotina,
        List<Atividade> atividades,
        List<Conclusao> feitas,
        string dataIso)
    {
        bool rotinaFeita = feitas.Any(c =>
            c.RotinaId == rotina.Id && c.AtividadeId == 0);

        var cartao = new Border
        {
            Style = Estilo("Cartao"),
            StrokeThickness = 4
        };

        var layout = new VerticalStackLayout
        {
            Spacing = 12
        };

        // Linha do topo: horário grande + botão para ouvir a rotina.
        var topo = new Grid
        {
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            }
        };

        topo.Add(new Label
        {
            Text = Util.HoraBr(rotina.Horario),
            FontSize = 34,
            FontFamily = "serif",
            TextColor = Cores.Azul,
            VerticalOptions = LayoutOptions.Center
        }, 0, 0);

        var botaoOuvir = new Button
        {
            Text = "🔊",
            FontSize = 26,
            WidthRequest = 56,
            HeightRequest = 56,
            CornerRadius = 28,
            Padding = 0,
            BackgroundColor = Cores.AzulClaro,
            TextColor = Cores.Branco
        };

        SemanticProperties.SetDescription(botaoOuvir, "Ouvir a rotina");

        botaoOuvir.Clicked += async (sender, args) =>
        {
            string fala = rotina.Titulo;

            if (!string.IsNullOrWhiteSpace(rotina.Descricao))
            {
                fala += ". " + rotina.Descricao;
            }

            await FalaService.FalarAsync(fala);
        };

        topo.Add(botaoOuvir, 1, 0);

        layout.Children.Add(topo);

        layout.Children.Add(new Label
        {
            Text = rotina.Titulo.ToUpper(),
            FontSize = 26,
            FontAttributes = FontAttributes.Bold,
            TextColor = Cores.Texto
        });

        if (!string.IsNullOrWhiteSpace(rotina.Descricao))
        {
            layout.Children.Add(new Label
            {
                Text = rotina.Descricao,
                FontSize = 19,
                TextColor = Cores.Texto
            });
        }

        // Passo a passo (atividades) da rotina.
        foreach (Atividade atividade in atividades)
        {
            bool atividadeFeita = feitas.Any(c =>
                c.RotinaId == rotina.Id && c.AtividadeId == atividade.Id);

            var linha = new HorizontalStackLayout
            {
                Spacing = 10
            };

            var caixa = new CheckBox
            {
                IsChecked = atividadeFeita,
                Color = Cores.Azul,
                Scale = 1.4,
                VerticalOptions = LayoutOptions.Center
            };

            caixa.CheckedChanged += async (sender, args) =>
            {
                // Evita gravar duas vezes quando o estado já está igual ao banco.
                bool estaMarcada = feitas.Any(c =>
                    c.RotinaId == rotina.Id && c.AtividadeId == atividade.Id);

                if (estaMarcada == args.Value)
                {
                    return;
                }

                bool ficouFeita = await _database.AlternarConclusaoAsync(
                    SessaoCrianca.CriancaId,
                    rotina.Id,
                    atividade.Id,
                    dataIso);

                if (ficouFeita)
                {
                    feitas.Add(new Conclusao
                    {
                        RotinaId = rotina.Id,
                        AtividadeId = atividade.Id
                    });
                }
                else
                {
                    feitas.RemoveAll(c =>
                        c.RotinaId == rotina.Id &&
                        c.AtividadeId == atividade.Id);
                }
            };

            linha.Children.Add(caixa);

            linha.Children.Add(new Label
            {
                Text = atividade.Descricao,
                FontSize = 20,
                TextColor = Cores.Texto,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 240
            });

            layout.Children.Add(linha);
        }

        // Botão grande: marcar a rotina inteira como feita.
        var botaoFeito = new Button
        {
            FontSize = 24,
            HeightRequest = 66,
            CornerRadius = 10
        };

        void AtualizarVisual(bool feita)
        {
            if (feita)
            {
                botaoFeito.Text = "✔ FEITO!";
                botaoFeito.BackgroundColor = Cores.Verde;
                botaoFeito.TextColor = Cores.Branco;
                cartao.Stroke = Cores.Verde;
            }
            else
            {
                botaoFeito.Text = "MARCAR COMO FEITO";
                botaoFeito.BackgroundColor = Cores.Azul;
                botaoFeito.TextColor = Cores.Branco;
                cartao.Stroke = Cores.Borda;
            }
        }

        AtualizarVisual(rotinaFeita);

        botaoFeito.Clicked += async (sender, args) =>
        {
            bool ficouFeita = await _database.AlternarConclusaoAsync(
                SessaoCrianca.CriancaId,
                rotina.Id,
                0,
                dataIso);

            AtualizarVisual(ficouFeita);

            if (ficouFeita)
            {
                await FalaService.FalarAsync("Muito bem!");
            }
        };

        layout.Children.Add(botaoFeito);

        cartao.Content = layout;

        return cartao;
    }
}
