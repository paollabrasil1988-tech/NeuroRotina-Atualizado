using NeuroRotina.Data;
using NeuroRotina.Models;

namespace NeuroRotina.Views;

public partial class CriancaPage : ContentPage
{
    private readonly DatabaseService _database;

    public CriancaPage()
    {
        InitializeComponent();

        _database = new DatabaseService();
    }

    private async void BtnCadastrar_Clicked(object sender, EventArgs e)
    {
        if (SessaoUsuario.UsuarioId == 0)
        {
            await DisplayAlert(
                "Atenção",
                "Nenhum adulto está logado.",
                "OK");

            return;
        }

        if (string.IsNullOrWhiteSpace(txtNome.Text))
        {
            await DisplayAlert(
                "Atenção",
                "Informe o nome da criança.",
                "OK");

            return;
        }

        if (string.IsNullOrWhiteSpace(
            pckNivelSuporte.SelectedItem?.ToString()))
        {
            await DisplayAlert(
                "Atenção",
                "Selecione o nível de suporte.",
                "OK");

            return;
        }

        string pin = txtPin.Text?.Trim() ?? string.Empty;

        if (pin.Length != 4 || !pin.All(char.IsDigit))
        {
            await DisplayAlert(
                "Atenção",
                "O PIN deve ter exatamente 4 dígitos.",
                "OK");

            return;
        }

        // No .NET 9 DatePicker.Date é DateTime; no .NET 10 é DateTime?.
        // Esta forma compila nos dois.
        DateTime? dataEscolhida = dtpNascimento.Date;

        var crianca = new Crianca
        {
            UsuarioId = SessaoUsuario.UsuarioId,
            Nome = txtNome.Text.Trim(),
            DataNascimento = dataEscolhida ?? DateTime.Today,
            NivelSuporte = pckNivelSuporte.SelectedItem.ToString()!,
            Pin = pin,
            Observacoes = txtObservacoes.Text?.Trim() ?? string.Empty
        };

        await _database.CadastrarCriancaAsync(crianca);

        await DisplayAlert(
            "Sucesso",
            "Criança cadastrada com sucesso!",
            "OK");

        txtNome.Text = string.Empty;
        pckNivelSuporte.SelectedItem = null;
        txtPin.Text = string.Empty;
        txtObservacoes.Text = string.Empty;
    }
}