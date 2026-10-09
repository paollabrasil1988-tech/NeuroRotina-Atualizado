using SQLite;

namespace NeuroRotina.Models;

public class Rotina
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int CriancaId { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public DateTime Data { get; set; }

    // Mantido por compatibilidade com versões antigas do app.
    // A conclusão diária agora fica na tabela Conclusao.
    public bool Concluida { get; set; }

    // Horário de início da rotina (e do alarme).
    public TimeSpan Horario { get; set; } = new TimeSpan(8, 0, 0);

    // Dias da semana separados por vírgula: 0 = domingo ... 6 = sábado. Vazio = todos os dias.
    public string DiasSemana { get; set; } = string.Empty;

    // Se verdadeiro, o celular toca um alerta semanal no horário da rotina.
    public bool AlertaAtivo { get; set; }

    // "alarme", "suave" ou "vibrar".
    public string Som { get; set; } = "alarme";
}
