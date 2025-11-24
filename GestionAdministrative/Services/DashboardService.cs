using GestionAdministrative.Data;
using GestionAdministrative.Models;
using GestionAdministrative.Services.Interfaces;

namespace GestionAdministrative.Services;

/// <summary>
/// Service pour les statistiques et le dashboard
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly AppDatabase _database;

    public DashboardService(AppDatabase database)
    {
        _database = database;
    }

    public async Task<int> GetTotalClientsAsync()
    {
        await _database.InitAsync();
        return await _database.Connection
            .Table<Client>()
            .CountAsync();
    }

    public async Task<decimal> GetChiffreAffaireTotalAsync()
    {
        await _database.InitAsync();
        var factures = await _database.Connection
            .Table<Facture>()
            .Where(f => f.Statut == "Payée")
            .ToListAsync();

        return factures.Sum(f => f.MontantTTC);
    }

    public async Task<decimal> GetChiffreAffaireMensuelAsync(int annee, int mois)
    {
        await _database.InitAsync();
        var factures = await _database.Connection
            .Table<Facture>()
            .Where(f => f.Statut == "Payée")
            .ToListAsync();

        return factures
            .Where(f => f.DatePaiement.HasValue &&
                       f.DatePaiement.Value.Year == annee &&
                       f.DatePaiement.Value.Month == mois)
            .Sum(f => f.MontantTTC);
    }

    public async Task<Dictionary<string, int>> GetFacturesParStatutAsync()
    {
        await _database.InitAsync();
        var factures = await _database.Connection
            .Table<Facture>()
            .ToListAsync();

        return factures
            .GroupBy(f => f.Statut)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public async Task<List<(string Mois, decimal Montant)>> GetCADerniersMoisAsync(int nbMois = 12)
    {
        await _database.InitAsync();
        var factures = await _database.Connection
            .Table<Facture>()
            .Where(f => f.Statut == "Payée")
            .ToListAsync();

        var result = new List<(string Mois, decimal Montant)>();
        var dateDebut = DateTime.Now.AddMonths(-nbMois);

        for (int i = 0; i < nbMois; i++)
        {
            var mois = DateTime.Now.AddMonths(-i);
            var montant = factures
                .Where(f => f.DatePaiement.HasValue &&
                           f.DatePaiement.Value.Year == mois.Year &&
                           f.DatePaiement.Value.Month == mois.Month)
                .Sum(f => f.MontantTTC);

            result.Add(($"{mois:MMM yyyy}", montant));
        }

        result.Reverse();
        return result;
    }

    public async Task<int> GetNombreDevisEnAttenteAsync()
    {
        await _database.InitAsync();
        return await _database.Connection
            .Table<Devis>()
            .Where(d => d.Statut == "Envoyé" || d.Statut == "Brouillon")
            .CountAsync();
    }

    public async Task<decimal> GetMontantFacturesImpayeesAsync()
    {
        await _database.InitAsync();
        var factures = await _database.Connection
            .Table<Facture>()
            .Where(f => f.Statut == "EnAttente" || f.Statut == "Impayée" || f.Statut == "PartialementPayée")
            .ToListAsync();

        return factures.Sum(f => f.MontantRestant);
    }
}
