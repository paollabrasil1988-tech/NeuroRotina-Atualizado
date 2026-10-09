using NeuroRotina.Data;
using NeuroRotina.Models;

namespace NeuroRotina.Views;

public partial class CadastroUsuarioPage : ContentPage
{
    private readonly DatabaseService _database;

    public CadastroUsuarioPage()
    {
        InitializeComponent();

        _database = new DatabaseService();
    }

    private async void BtnCriarConta_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNome.Text))
        {
            await DisplayAlert(
                "Atenção",
                "Informe seu nome.",
                "OK");

            return;
        }

        if (string.IsNullOrWhiteSpace(txtEmail.Text))
        {
            await DisplayAlert(
                "Atenção",
                "Informe seu e-mail.",
                "OK");

            return;
        }

        if (string.IsNullOrWhiteSpace(txtSenha.Text))
        {
            await DisplayAlert(
                "Atenção",
                "Informe uma senha.",
                "OK");

            return;
        }

        if (txtSenha.Text != txtConfirmarSenha.Text)
        {
            await DisplayAlert(
                "Atenção",
                "As senhas não são iguais.",
                "OK");

            return;
        }

        var usuarioExistente = await _database.BuscarUsuarioPorEmailAsync(
            txtEmail.Text.Trim());

        if (usuarioExistente != null)
        {
            await DisplayAlert(
                "Atenção",
                "Este e-mail já está cadastrado.",
                "OK");

            return;
        }

        var usuario = new Usuario
        {
            Nome = txtNome.Text.Trim(),
            Email = txtEmail.Text.Trim(),
            Senha = txtSenha.Text
        };

        await _database.CadastrarUsuarioAsync(usuario);

        await DisplayAlert(
            "Sucesso",
            "Conta criada com sucesso!",
            "OK");

        await Shell.Current.GoToAsync("..");
    }
}