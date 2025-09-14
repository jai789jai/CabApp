using CabApp.Core.Abstraction;
using CabApp.Core.Implementation;
using CabApp.Core.Implementation.MenuActions;
using CabApp.Core.Implementation.MenuActions.Cabs;
using CabApp.Core.Implementation.MenuActions.Cars;
using CabApp.Core.Implementation.MenuActions.Drivers;
using CabApp.Core.Implementation.MenuActions.Trips;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace CabApp
{
    public static class Registry
    {
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.AddNLog();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // Services Registration
                    InfraRegistration(services);
                    MenuRegistration(services);
                    CabMenuRegistration(services);
                    CarMenuRegistration(services);
                    DriverMenuRegistration(services);
                    TripMenuRegistration(services);
                    services.AddSingleton<App>();
                    services.AddTransient<ExitMenuAction>();
                });
        }

        private static void MenuRegistration(IServiceCollection services)
        {
            services.AddSingleton<IMenuService, MenuService>();
            services.AddSingleton<IMenuCoordinator, MenuCoordinator>();
        }

        private static void InfraRegistration(IServiceCollection services)
        {
            services.AddSingleton<IAppLogger, AppLogger>();
            services.AddSingleton<IDataService, DataService>();
            services.AddSingleton<IDataStore, DataStore>(provider =>
            {
                var logger = provider.GetService<IAppLogger>();
                var dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
                return new DataStore(dataDirectory, logger!);
            });
        }

        private static void CabMenuRegistration(IServiceCollection services)
        {
            services.AddTransient<CabMenuAction>();
            services.AddTransient<ViewCabsMenuAction>();
            services.AddTransient<AddCabMenuAction>();
            services.AddTransient<UpdateCabMenuAction>();
            services.AddTransient<RemoveCabMenuAction>();
        }

        private static void CarMenuRegistration(IServiceCollection services)
        {
            services.AddTransient<CarMenuAction>();
            services.AddTransient<ViewCarsMenuAction>();
            services.AddTransient<AddCarMenuAction>();
            services.AddTransient<UpdateCarMenuAction>();
            services.AddTransient<RemoveCarMenuAction>();
        }

        private static void DriverMenuRegistration(IServiceCollection services)
        {
            services.AddTransient<DriverMenuAction>();
            services.AddTransient<ViewDriversMenuAction>();
            services.AddTransient<AddDriverMenuAction>();
            services.AddTransient<UpdateDriverMenuAction>();
            services.AddTransient<RemoveDriverMenuAction>();
        }

        private static void TripMenuRegistration(IServiceCollection services)
        {
            services.AddTransient<TripMenuAction>();
            services.AddTransient<ViewTripsMenuAction>();
            services.AddTransient<AddTripMenuAction>();
            services.AddTransient<UpdateTripMenuAction>();
            services.AddTransient<RemoveTripMenuAction>();
        }
    }
}
