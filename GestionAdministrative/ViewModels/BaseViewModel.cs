using CommunityToolkit.Mvvm.ComponentModel;

namespace GestionAdministrative.ViewModels;

/// <summary>
/// ViewModel de base avec propriétés communes
/// </summary>
public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError;

    public void ShowError(string message)
    {
        ErrorMessage = message;
        HasError = true;
    }

    public void ClearError()
    {
        ErrorMessage = string.Empty;
        HasError = false;
    }
}
