using NeuroRotina.Models;

namespace NeuroRotina.Views;

public partial class InicioPage : ContentPage
{
    public InicioPage()
    {
        InitializeComponent();

        lblBoasVindas.Text = string.IsNullOrWhiteSpace(SessaoUsuario.Nome)
            ? "Bem-vindo(a)!"
            : $"Olá, {SessaoUsuario.Nome}!";
    }

    private async void BtnMinhasCriancas_Clicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync(
            nameof(ListaCriancasPage));
    }

    private async void BtnCadastrarCrianca_Clicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync(
            nameof(CriancaPage));
    }

    private async void BtnConfiguracoes_Clicked(
        object sender,
        EventArgs e)
    {
        await DisplayAlert(
            "CONFIGURAÇÕES",
            "A área de configurações será criada posteriormente.",
            "OK");
    }

    private async void BtnSair_Clicked(
        object sender,
        EventArgs e)
    {
        SessaoUsuario.Limpar();

        await Shell.Current.GoToAsync("//LoginPage");
    }
}
