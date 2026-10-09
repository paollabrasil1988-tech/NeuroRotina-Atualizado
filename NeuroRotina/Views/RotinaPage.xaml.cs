using NeuroRotina.Data;
using NeuroRotina.Models;
using NeuroRotina.Services;

namespace NeuroRotina.Views;

[QueryProperty(nameof(CriancaId), "criancaId")]
[QueryProperty(nameof(RotinaId), "rotinaId")]
public partial class RotinaPage : ContentPage
{
    private static readonly string[] LetrasDias =
        { "D", "S", "T", "Q", "Q", "S", "S" };

    private static readonly string[] ChavesSom =
        { "alarme", "suave", "vibrar" };

    private readonly DatabaseService _database;
    private readonly Button[] _botoesDias = new Button[7];
    private readonly bool[] _diasSelecionados = new bool[7];

    public int CriancaId { get; set; }

    public int RotinaId { get; set; }

    public RotinaPage()
    {
        InitializeComponent();

        _database = new DatabaseService();

        pckSom.SelectedIndex = 0;

        CriarBotoesDias();
    }

    // Os 7 botões redondos D S T Q Q S S (como nos aplicativos de despertador).
    private void CriarBotoesDias()
    {
        for (int dia = 0; dia < 7; dia++)
        {
            int diaAtual = dia;

            var botao = new Button
            {
                Text = LetrasDias[dia],
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                Padding = 0,
                MinimumWidthRequest = 0,
                WidthRequest = 44,
                HeightRequest = 44,
                CornerRadius = 22,
                HorizontalOptions = LayoutOptions.Center
            };

            SemanticProperties.SetDescription(botao, Util.DiasLongos[dia]);

            botao.Clicked += (sender, args) =>
            {
                _diasSelecionados[diaAtual] = !_diasSelecionados[diaAtual];
                AtualizarBotaoDia(diaAtual);
            };

            _botoesDias[dia] = botao;
            AtualizarBotaoDia(dia);

            gridDias.Add(botao, dia, 0);
        }
    }

    private void AtualizarBotaoDia(int dia)
    {
        Button botao = _botoesDias[dia];

        if (_diasSelecionados[dia])
        {
            botao.BackgroundColor = Cores.Azul;
            botao.TextColor = Cores.Branco;
            botao.BorderColor = Cores.Azul;
            botao.BorderWidth = 2;
        }
        else
        {
            botao.BackgroundColor = Cores.Branco;
            botao.TextColor = Cores.Azul;
            botao.BorderColor = Cores.Azul;
            botao.BorderWidth = 2;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (RotinaId > 0)
        {
            await CarregarRotinaAsync();
        }
        else
        {
            cabecalho.Titulo = "Nova Rotina";
            btnSalvar.Text = "Salvar";
        }
    }

    private async Task CarregarRotinaAsync()
    {
        Rotina? rotina = await _database.BuscarRotinaPorIdAsync(RotinaId);

        if (rotina == null)
        {
            return;
        }

        cabecalho.Titulo = "Editar Rotina";
        btnSalvar.Text = "Salvar alterações";

        txtTitulo.Text = rotina.Titulo;
        txtDescricao.Text = rotina.Descricao;
        tpHorario.Time = rotina.Horario;
        swAlerta.IsToggled = rotina.AlertaAtivo;

        int posicaoSom = Array.IndexOf(ChavesSom, rotina.Som);
        pckSom.SelectedIndex = posicaoSom >= 0 ? posicaoSom : 0;

        List<int> dias = Util.LerDias(rotina.DiasSemana);

        for (int dia = 0; dia < 7; dia++)
        {
            _diasSelecionados[dia] = dias.Contains(dia);
            AtualizarBotaoDia(dia);
        }
    }

    private async void BtnCadastrar_Clicked(object sender, EventArgs e)
    {
        lblMensagem.Text = string.Empty;

        if (string.IsNullOrWhiteSpace(txtTitulo.Text))
        {
            await DisplayAlert(
                "Atenção",
                "Informe o título da rotina.",
                "OK");

            return;
        }

        List<int> dias = Enumerable
            .Range(0, 7)
            .Where(d => _diasSelecionados[d])
            .ToList();

        if (swAlerta.IsToggled && dias.Count == 0)
        {
            await DisplayAlert(
                "Atenção",
                "Escolha pelo menos um dia da semana para o alerta tocar.",
                "OK");

            return;
        }

        // No .NET 9 TimePicker.Time é TimeSpan; no .NET 10 é TimeSpan?.
        // Esta forma compila nos dois.
        TimeSpan? horarioEscolhido = tpHorario.Time;
        TimeSpan horario = horarioEscolhido ?? new TimeSpan(8, 0, 0);

        string som = ChavesSom[pckSom.SelectedIndex >= 0 ? pckSom.SelectedIndex : 0];

        Rotina rotina;

        if (RotinaId > 0)
        {
            Rotina? existente = await _database.BuscarRotinaPorIdAsync(RotinaId);

            if (existente == null)
            {
                await DisplayAlert(
                    "Erro",
                    "Rotina não encontrada.",
                    "OK");

                return;
            }

            rotina = existente;
        }
        else
        {
            rotina = new Rotina
            {
                CriancaId = CriancaId,
                Data = DateTime.Today,
                Concluida = false
            };
        }

        rotina.Titulo = txtTitulo.Text.Trim();
        rotina.Descricao = txtDescricao.Text?.Trim() ?? string.Empty;
        rotina.Horario = horario;
        rotina.DiasSemana = Util.GravarDias(dias);
        rotina.AlertaAtivo = swAlerta.IsToggled;
        rotina.Som = som;

        if (RotinaId > 0)
        {
            await _database.AtualizarRotinaAsync(rotina);
        }
        else
        {
            // O Insert preenche o Id da rotina, necessário para agendar o alarme.
            await _database.CadastrarRotinaAsync(rotina);
        }

        AlarmeService.Agendar(rotina);

        string resumo = rotina.AlertaAtivo
            ? $"Alerta: {Util.DescreverDias(rotina.DiasSemana)} às {Util.HoraBr(rotina.Horario)}."
            : "Sem alerta sonoro.";

        await DisplayAlert(
            "Sucesso",
            $"Rotina salva!\n{resumo}",
            "OK");

        if (rotina.AlertaAtivo)
        {
            await AlarmeService.GarantirPermissaoAsync(this);
        }

        if (RotinaId > 0)
        {
            await Shell.Current.GoToAsync("..");
            return;
        }

        txtTitulo.Text = string.Empty;
        txtDescricao.Text = string.Empty;
    }

    private async void BtnVerRotinas_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(
            $"{nameof(ListaRotinasPage)}?criancaId={CriancaId}");
    }
}
