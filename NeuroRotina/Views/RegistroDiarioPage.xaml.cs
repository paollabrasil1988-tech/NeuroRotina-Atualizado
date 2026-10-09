using NeuroRotina.Data;
using NeuroRotina.Models;

namespace NeuroRotina.Views;

[QueryProperty(nameof(CriancaId), "criancaId")]
public partial class RegistroDiarioPage : ContentPage
{
    private readonly DatabaseService _database;

    // Último resumo automático colocado na observação (para não apagar o que o cuidador digitou).
    private string _ultimoResumo = string.Empty;

    public int CriancaId { get; set; }

    public RegistroDiarioPage()
    {
        InitializeComponent();

        _database = new DatabaseService();

        dtpData.Date = DateTime.Today;
        pckComportamento.SelectedIndex = 0;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await PreencherResumoAsync();
    }

    private DateTime DataEscolhida()
    {
        // No .NET 9 DatePicker.Date é DateTime; no .NET 10 é DateTime?.
        DateTime? data = dtpData.Date;
        return data ?? DateTime.Today;
    }

    private async Task PreencherResumoAsync()
    {
        string resumo = await _database.MontarResumoPedidosAsync(
            CriancaId,
            DataEscolhida());

        bool podeSubstituir =
            string.IsNullOrWhiteSpace(txtObservacao.Text) ||
            txtObservacao.Text == _ultimoResumo;

        if (podeSubstituir)
        {
            txtObservacao.Text = resumo;
            _ultimoResumo = resumo;
        }
    }

    private async void DtpData_DateSelected(object? sender, DateChangedEventArgs e)
    {
        await PreencherResumoAsync();
    }

    private async void BtnSalvar_Clicked(object sender, EventArgs e)
    {
        if (pckComportamento.SelectedIndex < 0)
        {
            await DisplayAlert(
                "Atenção",
                "Escolha o comportamento.",
                "OK");

            return;
        }

        if (string.IsNullOrWhiteSpace(txtObservacao.Text))
        {
            await DisplayAlert(
                "Atenção",
                "Escreva uma observação sobre o dia.",
                "OK");

            return;
        }

        var registro = new RegistroDiario
        {
            CriancaId = CriancaId,
            Data = DataEscolhida(),
            Comportamento = pckComportamento.SelectedItem?.ToString() ?? string.Empty,
            Observacao = txtObservacao.Text.Trim()
        };

        await _database.CadastrarRegistroAsync(registro);

        await DisplayAlert(
            "Sucesso",
            "Registro salvo!",
            "OK");

        await Shell.Current.GoToAsync("..");
    }
}
