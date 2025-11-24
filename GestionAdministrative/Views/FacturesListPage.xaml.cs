using GestionAdministrative.ViewModels;

namespace GestionAdministrative.Views;

public partial class FacturesListPage : ContentPage
{
    public FacturesListPage(FacturesListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is FacturesListViewModel viewModel)
        {
            await viewModel.LoadFacturesCommand.ExecuteAsync(null);
        }
    }
}
