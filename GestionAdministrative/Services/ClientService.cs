using GestionAdministrative.Data;
using GestionAdministrative.Models;
using GestionAdministrative.Services.Interfaces;

namespace GestionAdministrative.Services;

/// <summary>
/// Service de gestion des clients
/// </summary>
public class ClientService : IClientService
{
    private readonly AppDatabase _database;

    public ClientService(AppDatabase database)
    {
        _database = database;
    }

    public async Task<List<Client>> GetAllClientsAsync()
    {
        await _database.InitAsync();
        return await _database.Connection
            .Table<Client>()
            .OrderBy(c => c.Nom)
            .ToListAsync();
    }

    public async Task<Client?> GetClientByIdAsync(int id)
    {
        await _database.InitAsync();
        return await _database.Connection
            .Table<Client>()
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SaveClientAsync(Client client)
    {
        await _database.InitAsync();

        if (client.Id != 0)
        {
            client.UpdatedAt = DateTime.UtcNow;
            return await _database.Connection.UpdateAsync(client);
        }
        else
        {
            return await _database.Connection.InsertAsync(client);
        }
    }

    public async Task<int> DeleteClientAsync(Client client)
    {
        await _database.InitAsync();
        return await _database.Connection.DeleteAsync(client);
    }

    public async Task<List<Devis>> GetClientDevisAsync(int clientId)
    {
        await _database.InitAsync();
        return await _database.Connection
            .Table<Devis>()
            .Where(d => d.ClientId == clientId)
            .OrderByDescending(d => d.DateEmission)
            .ToListAsync();
    }

    public async Task<List<Facture>> GetClientFacturesAsync(int clientId)
    {
        await _database.InitAsync();
        return await _database.Connection
            .Table<Facture>()
            .Where(f => f.ClientId == clientId)
            .OrderByDescending(f => f.DateEmission)
            .ToListAsync();
    }

    public async Task<bool> ClientHasDocumentsAsync(int clientId)
    {
        await _database.InitAsync();

        var devisCount = await _database.Connection
            .Table<Devis>()
            .Where(d => d.ClientId == clientId)
            .CountAsync();

        var facturesCount = await _database.Connection
            .Table<Facture>()
            .Where(f => f.ClientId == clientId)
            .CountAsync();

        return (devisCount + facturesCount) > 0;
    }
}
