using NeuroRotina.Data;
using NeuroRotina.Models;

namespace NeuroRotina.Views;

public partial class LoginPage : ContentPage
{
    private readonly DatabaseService _database;

    public LoginPage()
    {
        InitializeComponent();

        _database = new DatabaseService();
    }

    private async void BtnEntrar_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtEmail.Text) ||
            string.IsNullOrWhiteSpace(txtSenha.Text))
        {
            await DisplayAlert(
                "Atenção",
                "Preencha o e-mail e a senha.",
                "OK");

            return;
        }

        var usuario = await _database.BuscarUsuarioAsync(
            txtEmail.Text.Trim(),
            txtSenha.Text);

        if (usuario == null)
        {
            lblMensagem.Text = "E-mail ou senha incorretos.";
            return;
        }

        SessaoUsuario.UsuarioId = usuario.Id;
        SessaoUsuario.Nome = usuario.Nome;
        SessaoUsuario.Email = usuario.Email;

        await DisplayAlert(
            "Sucesso",
            $"Bem-vinda, {usuario.Nome}!",
            "OK");

        await Shell.Current.GoToAsync(nameof(InicioPage));
    }

    private async void BtnCriarConta_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CadastroUsuarioPage));
    }

    private async void BtnEntrarComoCrianca_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(LoginCriancaPage));
    }
}