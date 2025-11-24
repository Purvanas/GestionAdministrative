using GestionAdministrative.ViewModels;

namespace GestionAdministrative.Views;

public partial class ClientDetailPage : ContentPage
{
    public ClientDetailPage(ClientDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
