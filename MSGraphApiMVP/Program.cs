﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using MSGraphApiMVP;

#region DI version
var builder = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options => options.ServiceName = "PrometheusMVPProvider")
    .ConfigureServices((_, services) => {
        LoggerProviderOptions.RegisterProviderOptions<ConsoleLoggerOptions, ConsoleLoggerProvider>(services);

        services.AddHostedService<WindowsBackgroundService>();

        // the ultimate cheat, should somehow use the original DI container
        {
            var factory = TokenAcquirerFactory.GetDefaultInstance();
            factory.Services.AddMicrosoftGraph();
            var client = factory.Build().GetRequiredService<GraphServiceClient>();
            services.AddSingleton(client);
        }
    });

var host = builder.Build();

await host.RunAsync();
#endregion

#region Manual version
//var factory = TokenAcquirerFactory.GetDefaultInstance();
//var services = factory.Services;

//services.AddMicrosoftGraph();

//var serviceProvider = factory.Build();

//var client = serviceProvider.GetRequiredService<GraphServiceClient>();
//var users = await client.Users.GetAsync(r => r.Options.WithAppOnly());
//users.Value.ForEach(u => Console.WriteLine(u.DisplayName));
#endregion
