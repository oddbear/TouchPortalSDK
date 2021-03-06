using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TouchPortalSDK.Sockets;

namespace TouchPortalSDK.Sample
{
    public static class Startup
    {
        /// <summary>
        /// Build ServiceProvider by a standard dotnet pattern.
        /// </summary>
        /// <returns>ServiceProvider</returns>
        public static IServiceProvider CreateServiceProvider()
        {
            var serviceCollection = new ServiceCollection();

            //Add Logging:
            serviceCollection.AddLogging(configure => configure.AddSimpleConsole(options => options.TimestampFormat = "[yyyy.MM.dd HH:mm:ss] "));

            //Add configuration:
            var configurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json")
                .Build();

            serviceCollection.Configure<TouchPortalOptions>(configurationRoot.GetSection("TouchPortalClientSettings"));

            //Add services:
            serviceCollection.AddSingleton<ITouchPortalClient, TouchPortalClient>();
            serviceCollection.AddSingleton<ITouchPortalSocket, TouchPortalSocket>();

            //Build service provider:
            return serviceCollection.BuildServiceProvider(true);
        }
    }
}
