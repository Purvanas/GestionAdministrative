using GestionAdministrative.Models;

namespace GestionAdministrative.Services.Interfaces;

/// <summary>
/// Interface pour la gestion des devis
/// </summary>
public interface IDevisService
{
    Task<List<Devis>> GetAllDevisAsync();
    Task<Devis?> GetDevisByIdAsync(int id);
    Task<Devis?> GetDevisWithDetailsAsync(int id);
    Task<int> SaveDevisAsync(Devis devis);
    Task<int> DeleteDevisAsync(Devis devis);
    Task<string> GenerateNumeroDevisAsync();
    Task<int> SaveDevisLigneAsync(DevisLigne ligne);
    Task<int> DeleteDevisLigneAsync(DevisLigne ligne);
    Task<List<DevisLigne>> GetDevisLignesAsync(int devisId);
    Task RecalculateDevisTotalsAsync(int devisId);
    Task<int> ConvertToFactureAsync(int devisId);
    Task<List<Devis>> GetDevisByClientAsync(int clientId);
}
