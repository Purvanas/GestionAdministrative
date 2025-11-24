using GestionAdministrative.Models;

namespace GestionAdministrative.Services.Interfaces;

/// <summary>
/// Interface pour la gestion des factures
/// </summary>
public interface IFactureService
{
    Task<List<Facture>> GetAllFacturesAsync();
    Task<Facture?> GetFactureByIdAsync(int id);
    Task<Facture?> GetFactureWithDetailsAsync(int id);
    Task<int> SaveFactureAsync(Facture facture);
    Task<int> DeleteFactureAsync(Facture facture);
    Task<string> GenerateNumeroFactureAsync();
    Task<int> SaveFactureLigneAsync(FactureLigne ligne);
    Task<int> DeleteFactureLigneAsync(FactureLigne ligne);
    Task<List<FactureLigne>> GetFactureLignesAsync(int factureId);
    Task RecalculateFactureTotalsAsync(int factureId);
    Task<int> UpdateStatutAsync(int factureId, string statut);
    Task<int> EnregistrerPaiementAsync(int factureId, decimal montant, DateTime datePaiement);
    Task<List<Facture>> GetFacturesByClientAsync(int clientId);
    Task<List<Facture>> GetFacturesImpayeesAsync();
}
