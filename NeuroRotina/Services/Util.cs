using System.Globalization;

namespace NeuroRotina.Services;

public static class Util
{
    public static readonly string[] DiasCurtos =
        { "Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sáb" };

    public static readonly string[] DiasLongos =
    {
        "Domingo", "Segunda-feira", "Terça-feira", "Quarta-feira",
        "Quinta-feira", "Sexta-feira", "Sábado"
    };

    /// <summary>Data no formato yyyy-MM-dd (usada para marcar o que foi feito em cada dia).</summary>
    public static string DataIso(DateTime data) =>
        data.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    public static string DataBr(DateTime data) =>
        data.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

    public static string HoraBr(TimeSpan horario) =>
        $"{horario.Hours:00}:{horario.Minutes:00}";

    /// <summary>Converte "1,3,5" em uma lista de dias (0 = domingo ... 6 = sábado).</summary>
    public static List<int> LerDias(string? texto)
    {
        var dias = new List<int>();

        if (string.IsNullOrWhiteSpace(texto))
        {
            return dias;
        }

        foreach (string parte in texto.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            if (int.TryParse(parte, out int dia) && dia >= 0 && dia <= 6 && !dias.Contains(dia))
            {
                dias.Add(dia);
            }
        }

        dias.Sort();
        return dias;
    }

    public static string GravarDias(IEnumerable<int> dias) =>
        string.Join(",", dias.OrderBy(d => d));

    /// <summary>Rotina sem dias escolhidos vale para todos os dias.</summary>
    public static bool ContemDia(string? texto, int dia)
    {
        List<int> dias = LerDias(texto);
        return dias.Count == 0 || dias.Contains(dia);
    }

    public static string DescreverDias(string? texto)
    {
        List<int> dias = LerDias(texto);

        if (dias.Count == 0 || dias.Count == 7)
        {
            return "Todos os dias";
        }

        return string.Join(", ", dias.Select(d => DiasCurtos[d]));
    }

    /// <summary>
    /// Próxima data/hora em que o dia da semana e o horário acontecem
    /// depois da data de referência.
    /// </summary>
    public static DateTime ProximaOcorrencia(int diaSemana, TimeSpan horario, DateTime referencia)
    {
        int diferenca = (diaSemana - (int)referencia.DayOfWeek + 7) % 7;
        DateTime alvo = referencia.Date.AddDays(diferenca) + horario;

        if (alvo <= referencia)
        {
            alvo = alvo.AddDays(7);
        }

        return alvo;
    }
}
