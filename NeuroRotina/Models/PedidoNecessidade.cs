using SQLite;

namespace NeuroRotina.Models;

/// <summary>Cada vez que a criança toca em um botão da tela de Comunicação.</summary>
public class PedidoNecessidade
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int CriancaId { get; set; }

    // Água, Banheiro, Brincar, Comer ou Ajuda
    public string Tipo { get; set; } = string.Empty;

    public DateTime DataHora { get; set; }
}
