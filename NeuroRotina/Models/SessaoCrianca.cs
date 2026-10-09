namespace NeuroRotina.Models;

public static class SessaoCrianca
{
    public static int CriancaId { get; set; }

    public static string Nome { get; set; } = string.Empty;

    public static string NivelSuporte { get; set; } = string.Empty;

    public static void Limpar()
    {
        CriancaId = 0;
        Nome = string.Empty;
        NivelSuporte = string.Empty;
    }
}