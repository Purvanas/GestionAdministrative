namespace GestionAdministrative.Services.Interfaces;

/// <summary>
/// Interface pour les statistiques du dashboard
/// </summary>
public interface IDashboardService
{
    Task<int> GetTotalClientsAsync();
    Task<decimal> GetChiffreAffaireTotalAsync();
    Task<decimal> GetChiffreAffaireMensuelAsync(int annee, int mois);
    Task<Dictionary<string, int>> GetFacturesParStatutAsync();
    Task<List<(string Mois, decimal Montant)>> GetCADerniersMoisAsync(int nbMois = 12);
    Task<int> GetNombreDevisEnAttenteAsync();
    Task<decimal> GetMontantFacturesImpayeesAsync();
}
