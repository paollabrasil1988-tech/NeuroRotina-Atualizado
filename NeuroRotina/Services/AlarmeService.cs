using NeuroRotina.Data;
using NeuroRotina.Models;

namespace NeuroRotina.Services;

/// <summary>
/// Ponto único para agendar e cancelar os alertas das rotinas.
/// Hoje o agendamento está implementado para Android; nas outras
/// plataformas as chamadas simplesmente não fazem nada.
/// </summary>
public static class AlarmeService
{
    public static void Agendar(Rotina rotina)
    {
#if ANDROID
        AgendadorAndroid.Agendar(
            global::Android.App.Application.Context,
            rotina.Id,
            rotina.Titulo,
            rotina.Descricao,
            rotina.Horario,
            Util.LerDias(rotina.DiasSemana),
            rotina.AlertaAtivo,
            rotina.Som);
#endif
    }

    public static void Cancelar(int rotinaId)
    {
#if ANDROID
        AgendadorAndroid.CancelarRotina(
            global::Android.App.Application.Context,
            rotinaId);
#endif
    }

    public static async Task ReagendarTodasAsync(DatabaseService banco)
    {
        List<Rotina> rotinas = await banco.BuscarRotinasComAlertaAsync();

        foreach (Rotina rotina in rotinas)
        {
            Agendar(rotina);
        }
    }

    /// <summary>
    /// Se o Android pedir a permissão de "Alarmes e lembretes" (Android 12+),
    /// explica o motivo e abre a tela de configuração.
    /// </summary>
    public static async Task GarantirPermissaoAsync(Page pagina)
    {
#if ANDROID
        var contexto = global::Android.App.Application.Context;

        if (!AgendadorAndroid.PodeAgendarExato(contexto))
        {
            bool abrir = await pagina.DisplayAlert(
                "Permitir alarmes",
                "Para tocar exatamente na hora, permita \"Alarmes e lembretes\" para o NeuroRotina.",
                "Abrir configurações",
                "Agora não");

            if (abrir)
            {
                AgendadorAndroid.AbrirConfiguracaoAlarmeExato(contexto);
            }
        }
#else
        await Task.CompletedTask;
#endif
    }
}
