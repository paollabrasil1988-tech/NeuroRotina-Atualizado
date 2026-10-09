using NeuroRotina.Data;
using NeuroRotina.Models;

namespace NeuroRotina.Views;

[QueryProperty(nameof(RotinaId), "rotinaId")]
[QueryProperty(nameof(RotinaTitulo), "rotinaTitulo")]
public partial class AtividadePage : ContentPage
{
    private readonly DatabaseService _database;

    public int RotinaId { get; set; }

    public string RotinaTitulo
    {
        set
        {
            lblRotina.Text = value.ToUpper();
        }
    }

    public AtividadePage()
    {
        InitializeComponent();

        _database = new DatabaseService();
    }

    private async void BtnCadastrar_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtDescricao.Text))
        {
            await DisplayAlert(
                "Atenção",
                "Informe a descrição da atividade.",
                "OK");

            return;
        }

        var atividade = new Atividade
        {
            RotinaId = RotinaId,
            Titulo = string.Empty,
            Descricao = txtDescricao.Text,
            Concluida = false
        };

        await _database.CadastrarAtividadeAsync(atividade);

        await DisplayAlert(
            "Sucesso",
            "Atividade cadastrada com sucesso!",
            "OK");

        txtDescricao.Text = string.Empty;
    }
}