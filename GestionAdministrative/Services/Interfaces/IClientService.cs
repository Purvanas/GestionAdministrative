using GestionAdministrative.Models;

namespace GestionAdministrative.Services.Interfaces;

/// <summary>
/// Interface pour la gestion des clients
/// </summary>
public interface IClientService
{
    Task<List<Client>> GetAllClientsAsync();
    Task<Client?> GetClientByIdAsync(int id);
    Task<int> SaveClientAsync(Client client);
    Task<int> DeleteClientAsync(Client client);
    Task<List<Devis>> GetClientDevisAsync(int clientId);
    Task<List<Facture>> GetClientFacturesAsync(int clientId);
    Task<bool> ClientHasDocumentsAsync(int clientId);
}
