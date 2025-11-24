using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionAdministrative.Models;
using GestionAdministrative.Services.Interfaces;
using System.Collections.ObjectModel;

namespace GestionAdministrative.ViewModels;

/// <summary>
/// ViewModel pour la liste des factures
/// </summary>
public partial class FacturesListViewModel : BaseViewModel
{
    private readonly IFactureService _factureService;

    [ObservableProperty]
    private ObservableCollection<Facture> factures = new();

    [ObservableProperty]
    private Facture? selectedFacture;

    [ObservableProperty]
    private bool isRefreshing;

    public FacturesListViewModel(IFactureService factureService)
    {
        _factureService = factureService;
        Title = "Factures";
    }

    [RelayCommand]
    private async Task LoadFacturesAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ClearError();

            var facturesList = await _factureService.GetAllFacturesAsync();
            Factures.Clear();

            foreach (var facture in facturesList)
            {
                Factures.Add(facture);
            }
        }
        catch (Exception ex)
        {
            ShowError($"Erreur lors du chargement des factures : {ex.Message}");
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
        await LoadFacturesAsync();
    }

    [RelayCommand]
    private async Task AddFactureAsync()
    {
        await Shell.Current.GoToAsync("facturedetail");
    }

    [RelayCommand]
    private async Task SelectFactureAsync(Facture facture)
    {
        if (facture == null)
            return;

        SelectedFacture = facture;
        await Shell.Current.GoToAsync($"facturedetail?id={facture.Id}");
    }

    // Note: UpdateStatutAsync doit être appelé depuis la page de détails de la facture
    // car RelayCommand ne supporte pas les méthodes avec 2 paramètres
    public async Task UpdateStatutAsync(int factureId, string statut)
    {
        try
        {
            IsBusy = true;
            await _factureService.UpdateStatutAsync(factureId, statut);
            await LoadFacturesAsync();
        }
        catch (Exception ex)
        {
            ShowError($"Erreur lors de la mise à jour : {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
