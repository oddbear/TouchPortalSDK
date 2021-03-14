﻿using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TouchPortalSDK.Configuration;

namespace TouchPortalSDK.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            //Build configuration:
            var configurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json")
                .Build();

            //Standard method for build a ServiceProvider in .Net,
            // you can use any other IoC container, or no container.
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(configure => configure.AddSimpleConsole(options => options.TimestampFormat = "[yyyy.MM.dd HH:mm:ss] "));
            
            //Registering the Plugin to the IoC container:
            serviceCollection.AddTouchPortalSdk<SamplePlugin>(configurationRoot);

            var serviceProvider = serviceCollection.BuildServiceProvider(true);

            //Use your IoC framework to resolve the plugin with it's dependencies,
            // or you can use 'TouchPortalFactory.Create' to get started manually:
            var plugin = serviceProvider.GetRequiredService<SamplePlugin>();
            plugin.Run();
        }
    }
}
