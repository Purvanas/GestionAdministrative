using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionAdministrative.Models;
using GestionAdministrative.Services.Interfaces;
using System.Collections.ObjectModel;

namespace GestionAdministrative.ViewModels;

/// <summary>
/// ViewModel pour la liste des clients
/// </summary>
public partial class ClientsViewModel : BaseViewModel
{
    private readonly IClientService _clientService;

    [ObservableProperty]
    private ObservableCollection<Client> clients = new();

    [ObservableProperty]
    private Client? selectedClient;

    [ObservableProperty]
    private bool isRefreshing;

    public ClientsViewModel(IClientService clientService)
    {
        _clientService = clientService;
        Title = "Clients";
    }

    [RelayCommand]
    private async Task LoadClientsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ClearError();

            var clientsList = await _clientService.GetAllClientsAsync();
            Clients.Clear();

            foreach (var client in clientsList)
            {
                Clients.Add(client);
            }
        }
        catch (Exception ex)
        {
            ShowError($"Erreur lors du chargement des clients : {ex.Message}");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        await LoadClientsAsync();
    }

    [RelayCommand]
    private async Task AddClientAsync()
    {
        // Navigation vers la page d'ajout
        await Shell.Current.GoToAsync("clientdetail");
    }

    [RelayCommand]
    private async Task SelectClientAsync(Client client)
    {
        if (client == null)
            return;

        SelectedClient = client;
        // Navigation vers la page de détails avec l'ID
        await Shell.Current.GoToAsync($"clientdetail?id={client.Id}");
    }

    [RelayCommand]
    private async Task DeleteClientAsync(Client client)
    {
        if (client == null)
            return;

        try
        {
            // Vérifier si le client a des documents
            var hasDocuments = await _clientService.ClientHasDocumentsAsync(client.Id);

            if (hasDocuments)
            {
                // Afficher une confirmation (à implémenter avec un popup)
                ShowError("Ce client possède des devis ou factures. Êtes-vous sûr de vouloir le supprimer ?");
                return;
            }

            await _clientService.DeleteClientAsync(client);
            Clients.Remove(client);
        }
        catch (Exception ex)
        {
            ShowError($"Erreur lors de la suppression : {ex.Message}");
        }
    }
}
