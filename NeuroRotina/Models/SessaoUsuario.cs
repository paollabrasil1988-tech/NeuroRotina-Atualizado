namespace NeuroRotina.Models;

public static class SessaoUsuario
{
    public static int UsuarioId { get; set; }

    public static string Nome { get; set; } = string.Empty;

    public static string Email { get; set; } = string.Empty;

    public static void Limpar()
    {
        UsuarioId = 0;
        Nome = string.Empty;
        Email = string.Empty;
    }
}