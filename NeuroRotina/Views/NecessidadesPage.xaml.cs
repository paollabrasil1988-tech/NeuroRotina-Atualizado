using NeuroRotina.Data;
using NeuroRotina.Models;
using NeuroRotina.Services;

namespace NeuroRotina.Views;

public partial class NecessidadesPage : ContentPage
{
    private readonly DatabaseService _database;

    public NecessidadesPage()
    {
        InitializeComponent();

        _database = new DatabaseService();
    }

    private async void Necessidade_Tapped(object? sender, TappedEventArgs e)
    {
        string? chave = e.Parameter as string;

        if (chave == null)
        {
            return;
        }

        await RegistrarEFalarAsync(chave, sender as Border);
    }

    private async void BtnAjuda_Clicked(object sender, EventArgs e)
    {
        await RegistrarEFalarAsync("ajuda", null);
    }

    private async Task RegistrarEFalarAsync(string chave, Border? cartao)
    {
        TipoNecessidade? tipo = Necessidades.Buscar(chave);

        if (tipo == null)
        {
            return;
        }

        lblConfirmacao.Text = tipo.Confirmacao;

        // Realce rápido no cartão tocado (o Figma mostra o cartão selecionado em destaque).
        if (cartao != null)
        {
            cartao.BackgroundColor = Cores.VerdeClaro;
        }

        // O pedido fica salvo para o cuidador ver no Histórico Diário.
        if (SessaoCrianca.CriancaId > 0)
        {
            await _database.RegistrarPedidoAsync(
                SessaoCrianca.CriancaId,
                tipo.Nome);
        }

        await FalaService.FalarAsync(tipo.Frase);

        if (cartao != null)
        {
            cartao.BackgroundColor = Cores.Branco;
        }
    }
}
