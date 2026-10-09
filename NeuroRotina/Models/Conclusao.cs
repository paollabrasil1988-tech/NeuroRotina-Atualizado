using SQLite;

namespace NeuroRotina.Models;

/// <summary>
/// Registra o que a criança já fez em cada dia.
/// AtividadeId = 0 significa que a rotina inteira foi concluída.
/// </summary>
public class Conclusao
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int CriancaId { get; set; }

    public int RotinaId { get; set; }

    public int AtividadeId { get; set; }

    // Formato yyyy-MM-dd
    public string Data { get; set; } = string.Empty;
}
