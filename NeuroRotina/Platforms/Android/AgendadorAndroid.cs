using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using AndroidX.Core.App;

namespace NeuroRotina.Services;

/// <summary>
/// Agenda os alarmes semanais das rotinas usando o AlarmManager do Android
/// (mesma tecnologia dos aplicativos de despertador).
/// Cada rotina gera um alarme por dia da semana escolhido. Quando o alarme
/// dispara, o AlarmeReceiver mostra a notificação com som e já agenda o
/// mesmo dia da semana seguinte.
/// </summary>
internal static class AgendadorAndroid
{
    public const string AcaoAlarme = "neurorotina.ALARME_ROTINA";

    public const string ExtraRotinaId = "rotinaId";
    public const string ExtraTitulo = "titulo";
    public const string ExtraDescricao = "descricao";
    public const string ExtraDia = "dia";
    public const string ExtraHora = "hora";
    public const string ExtraMinuto = "minuto";
    public const string ExtraSom = "som";

    private const string CanalAlarme = "neurorotina_alarme";
    private const string CanalSuave = "neurorotina_suave";
    private const string CanalVibrar = "neurorotina_vibrar";

    // Código único de cada alarme: rotina + dia da semana (0 = domingo ... 6 = sábado).
    private static int Codigo(int rotinaId, int dia) => (rotinaId * 10) + dia;

    public static void Agendar(
        Context context,
        int rotinaId,
        string titulo,
        string descricao,
        TimeSpan horario,
        IEnumerable<int> dias,
        bool alertaAtivo,
        string som)
    {
        CancelarRotina(context, rotinaId);

        if (!alertaAtivo)
        {
            return;
        }

        foreach (int dia in dias)
        {
            AgendarDia(
                context,
                rotinaId,
                titulo,
                descricao,
                dia,
                horario.Hours,
                horario.Minutes,
                som,
                DateTime.Now);
        }
    }

    public static void AgendarDia(
        Context context,
        int rotinaId,
        string titulo,
        string descricao,
        int dia,
        int hora,
        int minuto,
        string som,
        DateTime referencia)
    {
        var gerenteAlarmes =
            (AlarmManager?)context.GetSystemService(Context.AlarmService);

        if (gerenteAlarmes == null)
        {
            return;
        }

        DateTime proxima = Util.ProximaOcorrencia(
            dia,
            new TimeSpan(hora, minuto, 0),
            referencia);

        long disparoMs = new DateTimeOffset(proxima).ToUnixTimeMilliseconds();

        PendingIntent pedido = CriarPedido(
            context,
            rotinaId,
            titulo,
            descricao,
            dia,
            hora,
            minuto,
            som);

        // No Android 12+ o horário exato precisa da permissão "Alarmes e lembretes".
        // Sem ela, o alarme ainda toca, mas pode atrasar alguns minutos.
        if (OperatingSystem.IsAndroidVersionAtLeast(31) &&
            !gerenteAlarmes.CanScheduleExactAlarms())
        {
            gerenteAlarmes.SetAndAllowWhileIdle(
                AlarmType.RtcWakeup,
                disparoMs,
                pedido);
        }
        else
        {
            gerenteAlarmes.SetExactAndAllowWhileIdle(
                AlarmType.RtcWakeup,
                disparoMs,
                pedido);
        }
    }

    public static void CancelarRotina(Context context, int rotinaId)
    {
        var gerenteAlarmes =
            (AlarmManager?)context.GetSystemService(Context.AlarmService);

        if (gerenteAlarmes == null)
        {
            return;
        }

        for (int dia = 0; dia < 7; dia++)
        {
            PendingIntent pedido = CriarPedido(
                context,
                rotinaId,
                string.Empty,
                string.Empty,
                dia,
                0,
                0,
                string.Empty);

            gerenteAlarmes.Cancel(pedido);
            pedido.Cancel();
        }
    }

    private static PendingIntent CriarPedido(
        Context context,
        int rotinaId,
        string titulo,
        string descricao,
        int dia,
        int hora,
        int minuto,
        string som)
    {
        var intent = new Intent(context, typeof(AlarmeReceiver));
        intent.SetAction(AcaoAlarme);
        intent.PutExtra(ExtraRotinaId, rotinaId);
        intent.PutExtra(ExtraTitulo, titulo);
        intent.PutExtra(ExtraDescricao, descricao);
        intent.PutExtra(ExtraDia, dia);
        intent.PutExtra(ExtraHora, hora);
        intent.PutExtra(ExtraMinuto, minuto);
        intent.PutExtra(ExtraSom, som);

        return PendingIntent.GetBroadcast(
            context,
            Codigo(rotinaId, dia),
            intent,
            PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable)!;
    }

    public static bool PodeAgendarExato(Context context)
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(31))
        {
            return true;
        }

        var gerenteAlarmes =
            (AlarmManager?)context.GetSystemService(Context.AlarmService);

        return gerenteAlarmes != null && gerenteAlarmes.CanScheduleExactAlarms();
    }

    public static void AbrirConfiguracaoAlarmeExato(Context context)
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(31))
        {
            return;
        }

        var intent = new Intent(
            Android.Provider.Settings.ActionRequestScheduleExactAlarm,
            Android.Net.Uri.Parse("package:" + context.PackageName));

        intent.AddFlags(ActivityFlags.NewTask);
        context.StartActivity(intent);
    }

    public static void MostrarNotificacao(
        Context context,
        int rotinaId,
        int dia,
        string titulo,
        string descricao,
        string som)
    {
        CriarCanais(context);

        string canal = som switch
        {
            "suave" => CanalSuave,
            "vibrar" => CanalVibrar,
            _ => CanalAlarme
        };

        var abrirApp = new Intent(context, typeof(NeuroRotina.MainActivity));
        abrirApp.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTop);

        PendingIntent toque = PendingIntent.GetActivity(
            context,
            Codigo(rotinaId, dia),
            abrirApp,
            PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable)!;

        string texto = string.IsNullOrWhiteSpace(descricao)
            ? "Está na hora da rotina!"
            : descricao;

        var construtor = new NotificationCompat.Builder(context, canal)
            .SetSmallIcon(Android.Resource.Drawable.IcLockIdleAlarm)!
            .SetContentTitle(string.IsNullOrWhiteSpace(titulo) ? "NeuroRotina" : titulo)!
            .SetContentText(texto)!
            .SetPriority(NotificationCompat.PriorityHigh)!
            .SetCategory(NotificationCompat.CategoryAlarm)!
            .SetVisibility(NotificationCompat.VisibilityPublic)!
            .SetAutoCancel(true)!
            .SetContentIntent(toque)!;

        // Antes do Android 8 o som é definido na própria notificação (não existem canais).
        if (!OperatingSystem.IsAndroidVersionAtLeast(26) && som != "vibrar")
        {
            construtor.SetSound(RingtoneManager.GetDefaultUri(
                som == "suave" ? RingtoneType.Notification : RingtoneType.Alarm));
        }

        var gerente =
            (NotificationManager?)context.GetSystemService(Context.NotificationService);

        gerente?.Notify(Codigo(rotinaId, dia), construtor.Build());
    }

    private static void CriarCanais(Context context)
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            return;
        }

        var gerente =
            (NotificationManager?)context.GetSystemService(Context.NotificationService);

        if (gerente == null)
        {
            return;
        }

        var atributosAlarme = new AudioAttributes.Builder()!
            .SetUsage(AudioUsageKind.Alarm)!
            .SetContentType(AudioContentType.Sonification)!
            .Build();

        var atributosSuave = new AudioAttributes.Builder()!
            .SetUsage(AudioUsageKind.Notification)!
            .SetContentType(AudioContentType.Sonification)!
            .Build();

        var alarme = new NotificationChannel(
            CanalAlarme,
            "Alarme de rotina",
            NotificationImportance.High)
        {
            Description = "Toca o som de alarme na hora de cada rotina."
        };
        alarme.SetSound(
            RingtoneManager.GetDefaultUri(RingtoneType.Alarm),
            atributosAlarme);
        alarme.EnableVibration(true);

        var suave = new NotificationChannel(
            CanalSuave,
            "Aviso suave de rotina",
            NotificationImportance.High)
        {
            Description = "Toca o som de notificação na hora de cada rotina."
        };
        suave.SetSound(
            RingtoneManager.GetDefaultUri(RingtoneType.Notification),
            atributosSuave);
        suave.EnableVibration(true);

        var vibrar = new NotificationChannel(
            CanalVibrar,
            "Somente vibrar",
            NotificationImportance.High)
        {
            Description = "Apenas vibra na hora de cada rotina."
        };
        vibrar.SetSound(null, null);
        vibrar.EnableVibration(true);

        gerente.CreateNotificationChannel(alarme);
        gerente.CreateNotificationChannel(suave);
        gerente.CreateNotificationChannel(vibrar);
    }
}
