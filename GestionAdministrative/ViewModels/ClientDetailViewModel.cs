using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionAdministrative.Models;
using GestionAdministrative.Services.Interfaces;

namespace GestionAdministrative.ViewModels;

/// <summary>
/// ViewModel pour les détails d'un client
/// </summary>
[QueryProperty(nameof(ClientId), "id")]
public partial class ClientDetailViewModel : BaseViewModel
{
    private readonly IClientService _clientService;

    [ObservableProperty]
    private int clientId;

    [ObservableProperty]
    private Client client = new();

    [ObservableProperty]
    private bool isNewClient = true;

    public ClientDetailViewModel(IClientService clientService)
    {
        _clientService = clientService;
        Title = "Détails Client";
    }

    partial void OnClientIdChanged(int value)
    {
        if (value > 0)
        {
            IsNewClient = false;
            _ = LoadClientAsync(value);
        }
    }

    private async Task LoadClientAsync(int id)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ClearError();

            var loadedClient = await _clientService.GetClientByIdAsync(id);
            if (loadedClient != null)
            {
                Client = loadedClient;
                Title = $"Client - {Client.Nom}";
            }
        }
        catch (Exception ex)
        {
            ShowError($"Erreur lors du chargement : {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ClearError();

            // Validation
            if (string.IsNullOrWhiteSpace(Client.Nom))
            {
                ShowError("Le nom est obligatoire");
                return;
            }

            if (string.IsNullOrWhiteSpace(Client.Email))
            {
                ShowError("L'email est obligatoire");
                return;
            }

            await _clientService.SaveClientAsync(Client);

            // Retour à la liste
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ShowError($"Erreur lors de la sauvegarde : {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
