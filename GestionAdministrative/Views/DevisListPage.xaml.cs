using GestionAdministrative.ViewModels;

namespace GestionAdministrative.Views;

public partial class DevisListPage : ContentPage
{
    public DevisListPage(DevisListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is DevisListViewModel viewModel)
        {
            await viewModel.LoadDevisCommand.ExecuteAsync(null);
        }
    }
}
