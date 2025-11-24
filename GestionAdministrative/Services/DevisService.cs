using GestionAdministrative.Data;
using GestionAdministrative.Models;
using GestionAdministrative.Services.Interfaces;

namespace GestionAdministrative.Services;

/// <summary>
/// Service de gestion des devis
/// </summary>
public class DevisService : IDevisService
{
    private readonly AppDatabase _database;
    private readonly IFactureService _factureService;

    public DevisService(AppDatabase database, IFactureService factureService)
    {
        _database = database;
        _factureService = factureService;
    }

    public async Task<List<Devis>> GetAllDevisAsync()
    {
        await _database.InitAsync();
        return await _database.Connection
            .Table<Devis>()
            .OrderByDescending(d => d.DateEmission)
            .ToListAsync();
    }

    public async Task<Devis?> GetDevisByIdAsync(int id)
    {
        await _database.InitAsync();
        return await _database.Connection
            .Table<Devis>()
            .Where(d => d.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<Devis?> GetDevisWithDetailsAsync(int id)
    {
        await _database.InitAsync();
        var devis = await GetDevisByIdAsync(id);

        if (devis != null)
        {
            devis.Lignes = await GetDevisLignesAsync(id);
            devis.Client = await _database.Connection
                .Table<Client>()
                .Where(c => c.Id == devis.ClientId)
                .FirstOrDefaultAsync();
        }

        return devis;
    }

    public async Task<int> SaveDevisAsync(Devis devis)
    {
        await _database.InitAsync();

        if (devis.Id != 0)
        {
            devis.UpdatedAt = DateTime.UtcNow;
            return await _database.Connection.UpdateAsync(devis);
        }
        else
        {
            // Générer un numéro automatique si non fourni
            if (string.IsNullOrWhiteSpace(devis.Numero))
            {
                devis.Numero = await GenerateNumeroDevisAsync();
            }
            return await _database.Connection.InsertAsync(devis);
        }
    }

    public async Task<int> DeleteDevisAsync(Devis devis)
    {
        await _database.InitAsync();

        // Supprimer les lignes associées
        var lignes = await GetDevisLignesAsync(devis.Id);
        foreach (var ligne in lignes)
        {
            await DeleteDevisLigneAsync(ligne);
        }

        return await _database.Connection.DeleteAsync(devis);
    }

    public async Task<string> GenerateNumeroDevisAsync()
    {
        await _database.InitAsync();
        var year = DateTime.Now.Year;
        var prefix = $"DEV-{year}-";

        var lastDevis = await _database.Connection
            .Table<Devis>()
            .Where(d => d.Numero.StartsWith(prefix))
            .OrderByDescending(d => d.Numero)
            .FirstOrDefaultAsync();

        int nextNumber = 1;
        if (lastDevis != null && !string.IsNullOrEmpty(lastDevis.Numero))
        {
            var parts = lastDevis.Numero.Split('-');
            if (parts.Length == 3 && int.TryParse(parts[2], out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        return $"{prefix}{nextNumber:D4}";
    }

    public async Task<int> SaveDevisLigneAsync(DevisLigne ligne)
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

    public async Task<int> DeleteDevisLigneAsync(DevisLigne ligne)
    {
        await _database.InitAsync();
        return await _database.Connection.DeleteAsync(ligne);
    }

    public async Task<List<DevisLigne>> GetDevisLignesAsync(int devisId)
    {
        await _database.InitAsync();
        return await _database.Connection
            .Table<DevisLigne>()
            .Where(dl => dl.DevisId == devisId)
            .ToListAsync();
    }

    public async Task RecalculateDevisTotalsAsync(int devisId)
    {
        await _database.InitAsync();
        var lignes = await GetDevisLignesAsync(devisId);
        var devis = await GetDevisByIdAsync(devisId);

        if (devis != null)
        {
            devis.MontantHT = lignes.Sum(l => l.MontantHT);
            devis.MontantTVA = lignes.Sum(l => l.MontantTVA);
            devis.MontantTTC = lignes.Sum(l => l.MontantTTC);
            devis.UpdatedAt = DateTime.UtcNow;

            await _database.Connection.UpdateAsync(devis);
        }
    }

    public async Task<int> ConvertToFactureAsync(int devisId)
    {
        await _database.InitAsync();
        var devis = await GetDevisWithDetailsAsync(devisId);

        if (devis == null)
            throw new InvalidOperationException("Devis introuvable");

        if (devis.Statut == "Converti")
            throw new InvalidOperationException("Ce devis a déjà été converti");

        // Créer la facture
        var facture = new Facture
        {
            Numero = await _factureService.GenerateNumeroFactureAsync(),
            ClientId = devis.ClientId,
            DevisId = devis.Id,
            DateEmission = DateTime.Now,
            DateEcheance = DateTime.Now.AddDays(30),
            Statut = "EnAttente",
            MontantHT = devis.MontantHT,
            MontantTVA = devis.MontantTVA,
            MontantTTC = devis.MontantTTC,
            Notes = devis.Notes
        };

        var factureId = await _factureService.SaveFactureAsync(facture);

        // Copier les lignes
        foreach (var ligneDevis in devis.Lignes)
        {
            var ligneFacture = new FactureLigne
            {
                FactureId = factureId,
                PrestationId = ligneDevis.PrestationId,
                Description = ligneDevis.Description,
                Quantite = ligneDevis.Quantite,
                PrixUnitaireHT = ligneDevis.PrixUnitaireHT,
                TauxTVA = ligneDevis.TauxTVA
            };

            await _factureService.SaveFactureLigneAsync(ligneFacture);
        }

        // Mettre à jour le statut du devis
        devis.Statut = "Converti";
        devis.FactureId = factureId;
        await SaveDevisAsync(devis);

        return factureId;
    }

    public async Task<List<Devis>> GetDevisByClientAsync(int clientId)
    {
        await _database.InitAsync();
        return await _database.Connection
            .Table<Devis>()
            .Where(d => d.ClientId == clientId)
            .OrderByDescending(d => d.DateEmission)
            .ToListAsync();
    }
}
