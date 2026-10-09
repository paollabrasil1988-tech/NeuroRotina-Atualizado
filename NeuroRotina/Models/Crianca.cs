using SQLite;

namespace NeuroRotina.Models;

public class Crianca
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public string Nome { get; set; } = string.Empty;

    public DateTime DataNascimento { get; set; }

    public string NivelSuporte { get; set; } = string.Empty;

    public string Pin { get; set; } = string.Empty;

    public string Observacoes { get; set; } = string.Empty;
}