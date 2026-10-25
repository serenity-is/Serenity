using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Serene;

public class Program
{
    public static int Main(string[] args)
    {
        if (new AppServices.DynamicDataGenerator().RunIf(args) is int dynamicDataExit)
            return dynamicDataExit;

        using var host = CreateHostBuilder(args).Build();

        if (host.RunMigrationsIf(args) is int migrateExit)
            return migrateExit;

        host.Run();
        return 0;
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStaticWebAssets();
                webBuilder.UseStartup<Startup>();
            })
            .ConfigureAppConfiguration((builderContext, config) =>
            {
                config.AddJsonFile("appsettings.bundles.json");
                config.AddJsonFile("appsettings.machine.json", optional: true);
            });
    }
}