using SQLite;

namespace NeuroRotina.Models;

public class Atividade
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int RotinaId { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public TimeSpan Horario { get; set; }

    public bool Concluida { get; set; }
}