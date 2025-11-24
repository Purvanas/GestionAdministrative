using GestionAdministrative.ViewModels;

namespace GestionAdministrative.Views;

public partial class ClientsPage : ContentPage
{
    public ClientsPage(ClientsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is ClientsViewModel viewModel)
        {
            await viewModel.LoadClientsCommand.ExecuteAsync(null);
        }
    }
}
