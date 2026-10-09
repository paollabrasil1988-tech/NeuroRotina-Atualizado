using NeuroRotina.Views;

namespace NeuroRotina;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(
            nameof(InicioPage),
            typeof(InicioPage));

        Routing.RegisterRoute(
            nameof(CriancaPage),
            typeof(CriancaPage));

        Routing.RegisterRoute(
            nameof(ListaCriancasPage),
            typeof(ListaCriancasPage));

        Routing.RegisterRoute(
            nameof(RotinaPage),
            typeof(RotinaPage));

        Routing.RegisterRoute(
            nameof(ListaRotinasPage),
            typeof(ListaRotinasPage));

        Routing.RegisterRoute(
            nameof(AtividadePage),
            typeof(AtividadePage));

        Routing.RegisterRoute(
            nameof(ListaAtividadesPage),
            typeof(ListaAtividadesPage));

        Routing.RegisterRoute(
            nameof(CadastroUsuarioPage),
            typeof(CadastroUsuarioPage));

        Routing.RegisterRoute(
            nameof(LoginCriancaPage),
            typeof(LoginCriancaPage));

        Routing.RegisterRoute(
            nameof(InicioCriancaPage),
            typeof(InicioCriancaPage));

        Routing.RegisterRoute(
            nameof(AtividadesCriancaPage),
            typeof(AtividadesCriancaPage));

        Routing.RegisterRoute(
            nameof(NecessidadesPage),
            typeof(NecessidadesPage));

        Routing.RegisterRoute(
            nameof(HistoricoPage),
            typeof(HistoricoPage));

        Routing.RegisterRoute(
            nameof(RegistroDiarioPage),
            typeof(RegistroDiarioPage));
    }
}