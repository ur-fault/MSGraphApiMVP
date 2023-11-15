using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.TokenCacheProviders.InMemory;
using Microsoft.Identity.Abstractions;
using MSGraphApiMVP;

var builder = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options => options.ServiceName = "PrometheusMVPProvider")
    .ConfigureServices((_, services) => {
        LoggerProviderOptions.RegisterProviderOptions<ConsoleLoggerOptions, ConsoleLoggerProvider>(services);

        services.AddHostedService<WindowsBackgroundService>();

        services.AddMicrosoftGraph(options => {
            options.Scopes = new[] { "User.Read.All" };
        });

        //services.AddHttpClient();
        //services.AddTokenAcquisition();

        services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme);
        services.AddInMemoryTokenCaches();
    });

var host = builder.Build();

await host.RunAsync();

//var factory = TokenAcquirerFactory.GetDefaultInstance();
//var services = factory.Services;

//services.AddMicrosoftGraph();

//var serviceProvider = factory.Build();

//var client = serviceProvider.GetRequiredService<GraphServiceClient>();
//var users = await client.Users.GetAsync(r => r.Options.WithAppOnly());
//users.Value.ForEach(u => Console.WriteLine(u.DisplayName));
