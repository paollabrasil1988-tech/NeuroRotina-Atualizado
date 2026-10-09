using Android.App;
using Android.Content;

namespace NeuroRotina.Services;

/// <summary>
/// Recebe o disparo do alarme: mostra a notificação com som e agenda
/// o mesmo dia da semana seguinte (repetição semanal).
/// </summary>
[BroadcastReceiver(Enabled = true, Exported = false)]
public class AlarmeReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context == null || intent == null)
        {
            return;
        }

        int rotinaId = intent.GetIntExtra(AgendadorAndroid.ExtraRotinaId, 0);
        int dia = intent.GetIntExtra(AgendadorAndroid.ExtraDia, 0);
        int hora = intent.GetIntExtra(AgendadorAndroid.ExtraHora, 0);
        int minuto = intent.GetIntExtra(AgendadorAndroid.ExtraMinuto, 0);

        string titulo = intent.GetStringExtra(AgendadorAndroid.ExtraTitulo) ?? string.Empty;
        string descricao = intent.GetStringExtra(AgendadorAndroid.ExtraDescricao) ?? string.Empty;
        string som = intent.GetStringExtra(AgendadorAndroid.ExtraSom) ?? "alarme";

        if (rotinaId == 0)
        {
            return;
        }

        AgendadorAndroid.MostrarNotificacao(
            context,
            rotinaId,
            dia,
            titulo,
            descricao,
            som);

        // A referência de +1 minuto garante que o próximo alarme seja na semana que vem.
        AgendadorAndroid.AgendarDia(
            context,
            rotinaId,
            titulo,
            descricao,
            dia,
            hora,
            minuto,
            som,
            DateTime.Now.AddMinutes(1));
    }
}
