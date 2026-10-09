using NeuroRotina.Data;
using NeuroRotina.Models;

namespace NeuroRotina.Views;

public partial class LoginCriancaPage : ContentPage
{
    private readonly DatabaseService _database;

    public LoginCriancaPage()
    {
        InitializeComponent();

        _database = new DatabaseService();
    }

    private async void BtnEntrar_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNome.Text))
        {
            await DisplayAlert(
                "Atenção",
                "Digite seu nome ou apelido.",
                "OK");

            return;
        }

        string pin = txtPin.Text?.Trim() ?? string.Empty;

        if (pin.Length != 4 || !pin.All(char.IsDigit))
        {
            await DisplayAlert(
                "Atenção",
                "Digite um PIN de 4 dígitos.",
                "OK");

            return;
        }

        Crianca? crianca =
            await _database.BuscarCriancaPorNomeEPinAsync(
                txtNome.Text.Trim(),
                pin);

        if (crianca == null)
        {
            lblMensagem.Text = "NOME OU PIN INCORRETOS.";
            return;
        }

        SessaoCrianca.CriancaId = crianca.Id;
        SessaoCrianca.Nome = crianca.Nome;
        SessaoCrianca.NivelSuporte = crianca.NivelSuporte;

        await Shell.Current.GoToAsync(
            nameof(InicioCriancaPage));
    }

    private async void BtnVoltar_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}