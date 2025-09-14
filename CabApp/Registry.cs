using CabApp.Core.Abstraction;
using CabApp.Core.Implementation;
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
                    services.AddSingleton<IAppLogger, AppLogger>();
                    services.AddSingleton<App>();
                    services.AddSingleton<IDataStore, DataStore>(provider =>
                    {
                        var logger = provider.GetService<IAppLogger>();
                        var dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
                        return new DataStore(dataDirectory, logger!);
                    });
                });
        }
    }
}
