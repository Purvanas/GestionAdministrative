using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionAdministrative.Models;
using GestionAdministrative.Services.Interfaces;
using System.Collections.ObjectModel;

namespace GestionAdministrative.ViewModels;

/// <summary>
/// ViewModel pour la liste des devis
/// </summary>
public partial class DevisListViewModel : BaseViewModel
{
    private readonly IDevisService _devisService;

    [ObservableProperty]
    private ObservableCollection<Devis> devisList = new();

    [ObservableProperty]
    private Devis? selectedDevis;

    [ObservableProperty]
    private bool isRefreshing;

    public DevisListViewModel(IDevisService devisService)
    {
        _devisService = devisService;
        Title = "Devis";
    }

    [RelayCommand]
    private async Task LoadDevisAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ClearError();

            var devisList = await _devisService.GetAllDevisAsync();
            DevisList.Clear();

            foreach (var devis in devisList)
            {
                DevisList.Add(devis);
            }
        }
        catch (Exception ex)
        {
            ShowError($"Erreur lors du chargement des devis : {ex.Message}");
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
        await LoadDevisAsync();
    }

    [RelayCommand]
    private async Task AddDevisAsync()
    {
        await Shell.Current.GoToAsync("devisdetail");
    }

    [RelayCommand]
    private async Task SelectDevisAsync(Devis devis)
    {
        if (devis == null)
            return;

        SelectedDevis = devis;
        await Shell.Current.GoToAsync($"devisdetail?id={devis.Id}");
    }

    [RelayCommand]
    private async Task ConvertToFactureAsync(Devis devis)
    {
        if (devis == null)
            return;

        try
        {
            IsBusy = true;
            var factureId = await _devisService.ConvertToFactureAsync(devis.Id);
            
            // Rafraîchir la liste
            await LoadDevisAsync();

            // Navigation vers la facture créée
            await Shell.Current.GoToAsync($"facturedetail?id={factureId}");
        }
        catch (Exception ex)
        {
            ShowError($"Erreur lors de la conversion : {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
