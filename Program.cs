using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ALSO;

internal class Program
{
  static void Main(string[] args)
  {
    var host = new HostBuilder()
        .ConfigureFunctionsWorkerDefaults()
        .ConfigureAppConfiguration((context, config) =>
        {
          // Add appsettings.json
          config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

          // Add local.settings.json for local development
          config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

          // Add environment variables
          config.AddEnvironmentVariables();
        })
        .ConfigureServices((ctx, services) =>
        {
          services.AddHttpClient<IProvisionApi, ProvisionApi>();
          services.Configure<ApiOptions>(ctx.Configuration.GetSection("Apis"));
        })
        .Build();

    host.Run();
  }
}
