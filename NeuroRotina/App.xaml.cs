using NeuroRotina.Data;
using NeuroRotina.Services;

namespace NeuroRotina;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // O design do app (Figma) é claro. Isso evita que o modo escuro do
        // celular deixe fundos e textos com cores erradas.
        UserAppTheme = AppTheme.Light;

        MainPage = new AppShell();

        InicializarBanco();
    }

    private async void InicializarBanco()
    {
        try
        {
            var database = new DatabaseService();

            await database.InicializarAsync();

            // Garante que os alertas semanais estejam agendados toda vez que o app abre.
            await AlarmeService.ReagendarTodasAsync(database);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao iniciar o banco: {ex}");
        }
    }
}
