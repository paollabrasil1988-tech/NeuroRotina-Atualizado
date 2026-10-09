using Android.App;
using Android.Content;
using NeuroRotina.Data;

namespace NeuroRotina.Services;

/// <summary>
/// Depois que o celular reinicia (ou o app é atualizado) o Android apaga os alarmes.
/// Este receptor lê as rotinas salvas e agenda tudo de novo.
/// </summary>
[BroadcastReceiver(Enabled = true, Exported = true)]
[IntentFilter(new[]
{
    Intent.ActionBootCompleted,
    "android.intent.action.MY_PACKAGE_REPLACED"
})]
public class BootReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context == null)
        {
            return;
        }

        var pendente = GoAsync();

        Task.Run(async () =>
        {
            try
            {
                string caminho = Path.Combine(
                    context.FilesDir!.AbsolutePath,
                    "neurorotina.db3");

                var banco = new DatabaseService(caminho);

                await banco.InicializarAsync();
                await AlarmeService.ReagendarTodasAsync(banco);
            }
            catch
            {
                // Se algo falhar, os alarmes serão reagendados quando o app for aberto.
            }
            finally
            {
                pendente?.Finish();
            }
        });
    }
}
