using GestionAdministrative.Models;

namespace GestionAdministrative.Services.Interfaces;

/// <summary>
/// Interface pour la gestion des prestations
/// </summary>
public interface IPrestationService
{
    Task<List<Prestation>> GetAllPrestationsAsync();
    Task<List<Prestation>> GetActivePrestationsAsync();
    Task<Prestation?> GetPrestationByIdAsync(int id);
    Task<int> SavePrestationAsync(Prestation prestation);
    Task<int> DeletePrestationAsync(Prestation prestation);
    Task<int> ToggleActivationAsync(int prestationId);
}
