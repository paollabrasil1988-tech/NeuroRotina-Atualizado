
using NeuroRotina.Data;
using NeuroRotina.Services;

namespace NeuroRotina;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Mantém o tema claro, conforme o design do aplicativo.
        UserAppTheme = AppTheme.Light;

        InicializarBanco();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }

    private async void InicializarBanco()
    {
        try
        {
            var database = new DatabaseService();

            await database.InicializarAsync();

            // Reagenda os alertas semanais ao iniciar o aplicativo.
            await AlarmeService.ReagendarTodasAsync(database);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Erro ao iniciar o banco: {ex}");
        }
    }
}