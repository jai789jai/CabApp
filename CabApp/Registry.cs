using CabApp.Core.Abstraction;
using CabApp.Core.Implementation;
using CabApp.Core.Implementation.MenuActions;
using CabApp.Core.Implementation.MenuActions.Cabs;
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
        }
    }
}
