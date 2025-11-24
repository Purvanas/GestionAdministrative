using GestionAdministrative.Data;
using GestionAdministrative.Models;
using GestionAdministrative.Services.Interfaces;

namespace GestionAdministrative.Services;

/// <summary>
/// Service de gestion des factures
/// </summary>
public class FactureService : IFactureService
{
    private readonly AppDatabase _database;

    public FactureService(AppDatabase database)
    {
        _database = database;
    }

    public async Task<List<Facture>> GetAllFacturesAsync()
    {
        await _database.InitAsync();
        return await _database.Connection
            .Table<Facture>()
            .OrderByDescending(f => f.DateEmission)
            .ToListAsync();
    }

    public async Task<Facture?> GetFactureByIdAsync(int id)
    {
        await _database.InitAsync();
        return await _database.Connection
            .Table<Facture>()
            .Where(f => f.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<Facture?> GetFactureWithDetailsAsync(int id)
    {
        await _database.InitAsync();
        var facture = await GetFactureByIdAsync(id);

        if (facture != null)
        {
            facture.Lignes = await GetFactureLignesAsync(id);
            facture.Client = await _database.Connection
                .Table<Client>()
                .Where(c => c.Id == facture.ClientId)
                .FirstOrDefaultAsync();
        }

        return facture;
    }

    public async Task<int> SaveFactureAsync(Facture facture)
    {
        await _database.InitAsync();

        if (facture.Id != 0)
        {
            facture.UpdatedAt = DateTime.UtcNow;
            return await _database.Connection.UpdateAsync(facture);
        }
        else
        {
            // Générer un numéro automatique si non fourni
            if (string.IsNullOrWhiteSpace(facture.Numero))
            {
                facture.Numero = await GenerateNumeroFactureAsync();
            }
            return await _database.Connection.InsertAsync(facture);
        }
    }

    public async Task<int> DeleteFactureAsync(Facture facture)
    {
        await _database.InitAsync();

        // Supprimer les lignes associées
        var lignes = await GetFactureLignesAsync(facture.Id);
        foreach (var ligne in lignes)
        {
            await DeleteFactureLigneAsync(ligne);
        }

        return await _database.Connection.DeleteAsync(facture);
    }

    public async Task<string> GenerateNumeroFactureAsync()
    {
        await _database.InitAsync();
        var year = DateTime.Now.Year;
        var prefix = $"FAC-{year}-";

        var lastFacture = await _database.Connection
            .Table<Facture>()
            .Where(f => f.Numero.StartsWith(prefix))
            .OrderByDescending(f => f.Numero)
            .FirstOrDefaultAsync();

        int nextNumber = 1;
        if (lastFacture != null && !string.IsNullOrEmpty(lastFacture.Numero))
        {
            var parts = lastFacture.Numero.Split('-');
            if (parts.Length == 3 && int.TryParse(parts[2], out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        return $"{prefix}{nextNumber:D4}";
    }

    public async Task<int> SaveFactureLigneAsync(FactureLigne ligne)
    {
        await _database.InitAsync();

        if (ligne.Id != 0)
        {
            ligne.UpdatedAt = DateTime.UtcNow;
            return await _database.Connection.UpdateAsync(ligne);
        }
        else
        {
            return await _database.Connection.InsertAsync(ligne);
        }
    }

    public async Task<int> DeleteFactureLigneAsync(FactureLigne ligne)
    {
        await _database.InitAsync();
        return await _database.Connection.DeleteAsync(ligne);
    }

    public async Task<List<FactureLigne>> GetFactureLignesAsync(int factureId)
    {
        await _database.InitAsync();
        return await _database.Connection
            .Table<FactureLigne>()
            .Where(fl => fl.FactureId == factureId)
            .ToListAsync();
    }

    public async Task RecalculateFactureTotalsAsync(int factureId)
    {
        await _database.InitAsync();
        var lignes = await GetFactureLignesAsync(factureId);
        var facture = await GetFactureByIdAsync(factureId);

        if (facture != null)
        {
            facture.MontantHT = lignes.Sum(l => l.MontantHT);
            facture.MontantTVA = lignes.Sum(l => l.MontantTVA);
            facture.MontantTTC = lignes.Sum(l => l.MontantTTC);
            facture.UpdatedAt = DateTime.UtcNow;

            await _database.Connection.UpdateAsync(facture);
        }
    }

    public async Task<int> UpdateStatutAsync(int factureId, string statut)
    {
        await _database.InitAsync();
        var facture = await GetFactureByIdAsync(factureId);

        if (facture != null)
        {
            facture.Statut = statut;
            facture.UpdatedAt = DateTime.UtcNow;

            if (statut == "Payée" && facture.DatePaiement == null)
            {
                facture.DatePaiement = DateTime.Now;
                facture.MontantPaye = facture.MontantTTC;
            }

            return await _database.Connection.UpdateAsync(facture);
        }

        return 0;
    }

    public async Task<int> EnregistrerPaiementAsync(int factureId, decimal montant, DateTime datePaiement)
    {
        await _database.InitAsync();
        var facture = await GetFactureByIdAsync(factureId);

        if (facture != null)
        {
            facture.MontantPaye += montant;
            facture.DatePaiement = datePaiement;
            facture.UpdatedAt = DateTime.UtcNow;

            // Mettre à jour le statut
            if (facture.MontantPaye >= facture.MontantTTC)
            {
                facture.Statut = "Payée";
            }
            else if (facture.MontantPaye > 0)
            {
                facture.Statut = "PartialementPayée";
            }

            return await _database.Connection.UpdateAsync(facture);
        }

        return 0;
    }

    public async Task<List<Facture>> GetFacturesByClientAsync(int clientId)
    {
        await _database.InitAsync();
        return await _database.Connection
            .Table<Facture>()
            .Where(f => f.ClientId == clientId)
            .OrderByDescending(f => f.DateEmission)
            .ToListAsync();
    }

    public async Task<List<Facture>> GetFacturesImpayeesAsync()
    {
        await _database.InitAsync();
        return await _database.Connection
            .Table<Facture>()
            .Where(f => f.Statut == "EnAttente" || f.Statut == "Impayée" || f.Statut == "PartialementPayée")
            .OrderBy(f => f.DateEcheance)
            .ToListAsync();
    }
}
