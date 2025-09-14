using CabApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

try
{
    using IHost host = Registry.CreateHostBuilder(args).Build();
    using var scope = host.Services.CreateScope();

    var services = scope.ServiceProvider;
    services.GetRequiredService<App>().Run(args);


} catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}