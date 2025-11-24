using GestionAdministrative.Views;

namespace GestionAdministrative
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Enregistrement des routes de navigation
            Routing.RegisterRoute("clientdetail", typeof(ClientDetailPage));
            Routing.RegisterRoute("devisdetail", typeof(ClientDetailPage)); // À remplacer par DevisDetailPage
            Routing.RegisterRoute("facturedetail", typeof(ClientDetailPage)); // À remplacer par FactureDetailPage
        }
    }
}
