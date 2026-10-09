using NeuroRotina.Models;

namespace NeuroRotina.Views;

public partial class InicioCriancaPage : ContentPage
{
    public InicioCriancaPage()
    {
        InitializeComponent();

        cabecalho.Titulo = $"Olá, {SessaoCrianca.Nome}!";
    }

    private async void BtnAtividades_Clicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync(
            nameof(AtividadesCriancaPage));
    }

    private async void BtnNecessidades_Clicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync(
            nameof(NecessidadesPage));
    }

    private async void BtnSair_Clicked(
        object sender,
        EventArgs e)
    {
        SessaoCrianca.Limpar();

        await Shell.Current.GoToAsync("//LoginPage");
    }
}
