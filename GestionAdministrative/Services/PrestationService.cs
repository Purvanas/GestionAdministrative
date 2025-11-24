using GestionAdministrative.Data;
using GestionAdministrative.Models;
using GestionAdministrative.Services.Interfaces;

namespace GestionAdministrative.Services;

/// <summary>
/// Service de gestion des prestations
/// </summary>
public class PrestationService : IPrestationService
{
    private readonly AppDatabase _database;

    public PrestationService(AppDatabase database)
    {
        _database = database;
    }

    public async Task<List<Prestation>> GetAllPrestationsAsync()
    {
        await _database.InitAsync();
        return await _database.Connection
            .Table<Prestation>()
            .OrderBy(p => p.Nom)
            .ToListAsync();
    }

    public async Task<List<Prestation>> GetActivePrestationsAsync()
    {
        await _database.InitAsync();
        return await _database.Connection
            .Table<Prestation>()
            .Where(p => p.EstActif)
            .OrderBy(p => p.Nom)
            .ToListAsync();
    }

    public async Task<Prestation?> GetPrestationByIdAsync(int id)
    {
        await _database.InitAsync();
        return await _database.Connection
            .Table<Prestation>()
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SavePrestationAsync(Prestation prestation)
    {
        await _database.InitAsync();

        if (prestation.Id != 0)
        {
            prestation.UpdatedAt = DateTime.UtcNow;
            return await _database.Connection.UpdateAsync(prestation);
        }
        else
        {
            return await _database.Connection.InsertAsync(prestation);
        }
    }

    public async Task<int> DeletePrestationAsync(Prestation prestation)
    {
        await _database.InitAsync();
        return await _database.Connection.DeleteAsync(prestation);
    }

    public async Task<int> ToggleActivationAsync(int prestationId)
    {
        await _database.InitAsync();
        var prestation = await GetPrestationByIdAsync(prestationId);

        if (prestation != null)
        {
            prestation.EstActif = !prestation.EstActif;
            prestation.UpdatedAt = DateTime.UtcNow;
            return await _database.Connection.UpdateAsync(prestation);
        }

        return 0;
    }
}
