using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionAdministrative.Models;
using GestionAdministrative.Services.Interfaces;
using System.Collections.ObjectModel;

namespace GestionAdministrative.ViewModels;

/// <summary>
/// ViewModel pour le dashboard
/// </summary>
public partial class DashboardViewModel : BaseViewModel
{
    private readonly IDashboardService _dashboardService;
    private readonly IClientService _clientService;
    private readonly IDevisService _devisService;

    [ObservableProperty]
    private int totalClients;

    [ObservableProperty]
    private decimal chiffreAffaireTotal;

    [ObservableProperty]
    private decimal chiffreAffaireMensuel;

    [ObservableProperty]
    private int nombreDevisEnAttente;

    [ObservableProperty]
    private decimal montantFacturesImpayees;

    [ObservableProperty]
    private ObservableCollection<(string Mois, decimal Montant)> cADerniersMois = new();

    [ObservableProperty]
    private Dictionary<string, int> facturesParStatut = new();

    public DashboardViewModel(
        IDashboardService dashboardService,
        IClientService clientService,
        IDevisService devisService)
    {
        _dashboardService = dashboardService;
        _clientService = clientService;
        _devisService = devisService;
        Title = "Tableau de bord";
    }

    [RelayCommand]
    private async Task LoadDashboardAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ClearError();

            // Charger toutes les statistiques en parallèle
            var tasks = new[]
            {
                LoadTotalClientsAsync(),
                LoadChiffreAffaireAsync(),
                LoadDevisEnAttenteAsync(),
                LoadFacturesImpayeesAsync(),
                LoadCADerniersMoisAsync(),
                LoadFacturesParStatutAsync()
            };

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            ShowError($"Erreur lors du chargement du dashboard : {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadTotalClientsAsync()
    {
        TotalClients = await _dashboardService.GetTotalClientsAsync();
    }

    private async Task LoadChiffreAffaireAsync()
    {
        ChiffreAffaireTotal = await _dashboardService.GetChiffreAffaireTotalAsync();
        ChiffreAffaireMensuel = await _dashboardService.GetChiffreAffaireMensuelAsync(
            DateTime.Now.Year,
            DateTime.Now.Month);
    }

    private async Task LoadDevisEnAttenteAsync()
    {
        NombreDevisEnAttente = await _dashboardService.GetNombreDevisEnAttenteAsync();
    }

    private async Task LoadFacturesImpayeesAsync()
    {
        MontantFacturesImpayees = await _dashboardService.GetMontantFacturesImpayeesAsync();
    }

    private async Task LoadCADerniersMoisAsync()
    {
        var data = await _dashboardService.GetCADerniersMoisAsync(12);
        CADerniersMois.Clear();

        foreach (var item in data)
        {
            CADerniersMois.Add(item);
        }
    }

    private async Task LoadFacturesParStatutAsync()
    {
        FacturesParStatut = await _dashboardService.GetFacturesParStatutAsync();
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadDashboardAsync();
    }
}
