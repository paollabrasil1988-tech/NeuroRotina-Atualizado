using SQLite;
using NeuroRotina.Models;

namespace NeuroRotina.Data;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _database;

    public DatabaseService()
        : this(Path.Combine(
            FileSystem.AppDataDirectory,
            "neurorotina.db3"))
    {
    }

    public DatabaseService(string caminhoBanco)
    {
        _database = new SQLiteAsyncConnection(caminhoBanco);
    }

    public async Task InicializarAsync()
    {
        await _database.CreateTableAsync<Usuario>();
        await _database.CreateTableAsync<Crianca>();
        await _database.CreateTableAsync<Rotina>();
        await _database.CreateTableAsync<Atividade>();
        await _database.CreateTableAsync<Conclusao>();
        await _database.CreateTableAsync<PedidoNecessidade>();
        await _database.CreateTableAsync<RegistroDiario>();

        await AtualizarBancoAsync();
    }

    private async Task AtualizarBancoAsync()
    {
        try
        {
            await _database.ExecuteAsync(
                "ALTER TABLE Crianca ADD COLUMN NivelSuporte TEXT");
        }
        catch
        {
            // A coluna já existe.
        }

        try
        {
            await _database.ExecuteAsync(
                "ALTER TABLE Crianca ADD COLUMN Pin TEXT");
        }
        catch
        {
            // A coluna já existe.
        }

        try
        {
            await _database.ExecuteAsync(
                "ALTER TABLE Atividade ADD COLUMN Horario TEXT");
        }
        catch
        {
            // A coluna já existe.
        }
    }

    public async Task<int> CadastrarUsuarioAsync(Usuario usuario)
    {
        return await _database.InsertAsync(usuario);
    }

    public async Task<Usuario?> BuscarUsuarioAsync(
        string email,
        string senha)
    {
        return await _database
            .Table<Usuario>()
            .Where(u => u.Email == email && u.Senha == senha)
            .FirstOrDefaultAsync();
    }

    public async Task<Usuario?> BuscarUsuarioPorEmailAsync(
        string email)
    {
        return await _database
            .Table<Usuario>()
            .Where(u => u.Email == email)
            .FirstOrDefaultAsync();
    }

    public async Task<int> CadastrarCriancaAsync(
        Crianca crianca)
    {
        return await _database.InsertAsync(crianca);
    }

    public async Task<List<Crianca>> BuscarCriancasAsync()
    {
        return await _database
            .Table<Crianca>()
            .ToListAsync();
    }

    public async Task<List<Crianca>> BuscarCriancasPorUsuarioAsync(
        int usuarioId)
    {
        return await _database
            .Table<Crianca>()
            .Where(c => c.UsuarioId == usuarioId)
            .ToListAsync();
    }

    public async Task<Crianca?> BuscarCriancaPorNomeEPinAsync(
        string nome,
        string pin)
    {
        string nomeBusca = nome.Trim().ToUpper();

        List<Crianca> criancas =
            await _database
                .Table<Crianca>()
                .ToListAsync();

        return criancas.FirstOrDefault(c =>
            c.Nome.Trim().ToUpper() == nomeBusca &&
            c.Pin == pin);
    }

    public async Task<int> CadastrarRotinaAsync(
        Rotina rotina)
    {
        return await _database.InsertAsync(rotina);
    }

    public async Task<List<Rotina>> BuscarRotinasPorCriancaAsync(
        int criancaId)
    {
        return await _database
            .Table<Rotina>()
            .Where(r => r.CriancaId == criancaId)
            .ToListAsync();
    }

    public async Task<int> AtualizarRotinaAsync(
        Rotina rotina)
    {
        return await _database.UpdateAsync(rotina);
    }

    public async Task<int> CadastrarAtividadeAsync(
        Atividade atividade)
    {
        return await _database.InsertAsync(atividade);
    }

    public async Task<List<Atividade>> BuscarAtividadesPorRotinaAsync(
        int rotinaId)
    {
        return await _database
            .Table<Atividade>()
            .Where(a => a.RotinaId == rotinaId)
            .ToListAsync();
    }

    public async Task<int> AtualizarAtividadeAsync(
        Atividade atividade)
    {
        return await _database.UpdateAsync(atividade);
    }

    // ---------- Rotinas (novos) ----------

    public async Task<Rotina?> BuscarRotinaPorIdAsync(int rotinaId)
    {
        return await _database
            .Table<Rotina>()
            .Where(r => r.Id == rotinaId)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Rotina>> BuscarRotinasComAlertaAsync()
    {
        return await _database
            .Table<Rotina>()
            .Where(r => r.AlertaAtivo)
            .ToListAsync();
    }

    public async Task ExcluirRotinaAsync(int rotinaId)
    {
        await _database.Table<Atividade>()
            .DeleteAsync(a => a.RotinaId == rotinaId);

        await _database.Table<Conclusao>()
            .DeleteAsync(c => c.RotinaId == rotinaId);

        await _database.DeleteAsync<Rotina>(rotinaId);
    }

    public async Task ExcluirAtividadeAsync(int atividadeId)
    {
        await _database.Table<Conclusao>()
            .DeleteAsync(c => c.AtividadeId == atividadeId);

        await _database.DeleteAsync<Atividade>(atividadeId);
    }

    // ---------- Conclusão diária ----------

    public async Task<List<Conclusao>> BuscarConclusoesDoDiaAsync(
        int criancaId,
        string dataIso)
    {
        return await _database
            .Table<Conclusao>()
            .Where(c => c.CriancaId == criancaId && c.Data == dataIso)
            .ToListAsync();
    }

    /// <summary>Marca ou desmarca. Retorna true quando ficou marcado como feito.</summary>
    public async Task<bool> AlternarConclusaoAsync(
        int criancaId,
        int rotinaId,
        int atividadeId,
        string dataIso)
    {
        List<Conclusao> doDia =
            await BuscarConclusoesDoDiaAsync(criancaId, dataIso);

        Conclusao? existente = doDia.FirstOrDefault(c =>
            c.RotinaId == rotinaId &&
            c.AtividadeId == atividadeId);

        if (existente != null)
        {
            await _database.DeleteAsync(existente);
            return false;
        }

        await _database.InsertAsync(new Conclusao
        {
            CriancaId = criancaId,
            RotinaId = rotinaId,
            AtividadeId = atividadeId,
            Data = dataIso
        });

        return true;
    }

    // ---------- Necessidades (tela de Comunicação) ----------

    public async Task RegistrarPedidoAsync(int criancaId, string tipo)
    {
        await _database.InsertAsync(new PedidoNecessidade
        {
            CriancaId = criancaId,
            Tipo = tipo,
            DataHora = DateTime.Now
        });
    }

    public async Task<List<PedidoNecessidade>> BuscarPedidosDoDiaAsync(
        int criancaId,
        DateTime dia)
    {
        List<PedidoNecessidade> todos = await _database
            .Table<PedidoNecessidade>()
            .Where(p => p.CriancaId == criancaId)
            .ToListAsync();

        return todos
            .Where(p => p.DataHora.Date == dia.Date)
            .OrderBy(p => p.DataHora)
            .ToList();
    }

    /// <summary>Ex.: "Usou a prancha de comunicação 3 vez(es): Água x2, Banheiro x1."</summary>
    public async Task<string> MontarResumoPedidosAsync(
        int criancaId,
        DateTime dia)
    {
        List<PedidoNecessidade> pedidos =
            await BuscarPedidosDoDiaAsync(criancaId, dia);

        if (pedidos.Count == 0)
        {
            return string.Empty;
        }

        IEnumerable<string> partes = pedidos
            .GroupBy(p => p.Tipo)
            .Select(g => $"{g.Key} x{g.Count()}");

        return $"Usou a prancha de comunicação {pedidos.Count} vez(es): " +
               $"{string.Join(", ", partes)}.";
    }

    // ---------- Histórico diário ----------

    public async Task<int> CadastrarRegistroAsync(RegistroDiario registro)
    {
        return await _database.InsertAsync(registro);
    }

    public async Task<List<RegistroDiario>> BuscarRegistrosPorCriancaAsync(
        int criancaId)
    {
        List<RegistroDiario> registros = await _database
            .Table<RegistroDiario>()
            .Where(r => r.CriancaId == criancaId)
            .ToListAsync();

        return registros
            .OrderByDescending(r => r.Data)
            .ThenByDescending(r => r.Id)
            .ToList();
    }

    public async Task ExcluirRegistroAsync(int registroId)
    {
        await _database.DeleteAsync<RegistroDiario>(registroId);
    }
}
