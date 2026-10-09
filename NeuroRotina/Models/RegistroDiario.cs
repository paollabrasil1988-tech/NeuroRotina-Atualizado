using SQLite;

namespace NeuroRotina.Models;

/// <summary>Anotação do cuidador sobre o dia da criança (tela Histórico Diário).</summary>
public class RegistroDiario
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int CriancaId { get; set; }

    public DateTime Data { get; set; }

    public string Comportamento { get; set; } = string.Empty;

    public string Observacao { get; set; } = string.Empty;
}
