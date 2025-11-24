using Microsoft.Extensions.Logging;
using GestionAdministrative.Data;
using GestionAdministrative.Services;
using GestionAdministrative.Services.Interfaces;
using GestionAdministrative.ViewModels;
using GestionAdministrative.Views;

namespace GestionAdministrative
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // Enregistrement de la base de données
            builder.Services.AddSingleton<AppDatabase>();

            // Enregistrement des Services
            builder.Services.AddSingleton<IClientService, ClientService>();
            builder.Services.AddSingleton<IPrestationService, PrestationService>();
            builder.Services.AddSingleton<IFactureService, FactureService>();
            builder.Services.AddSingleton<IDevisService, DevisService>();
            builder.Services.AddSingleton<IDashboardService, DashboardService>();

            // Enregistrement des ViewModels
            builder.Services.AddTransient<ClientsViewModel>();
            builder.Services.AddTransient<ClientDetailViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<DevisListViewModel>();
            builder.Services.AddTransient<FacturesListViewModel>();

            // Enregistrement des Pages
            builder.Services.AddTransient<ClientsPage>();
            builder.Services.AddTransient<ClientDetailPage>();
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<DevisListPage>();
            builder.Services.AddTransient<FacturesListPage>();

            return builder.Build();
        }
    }
}
