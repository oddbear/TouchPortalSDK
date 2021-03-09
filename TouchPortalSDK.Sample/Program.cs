using System;
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
            //Add configuration:
            var configurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json")
                .Build();

            //Standard method for build a ServiceProvider in .Net,
            // you can use any other IoC container, or no at all if you want:
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(configure => configure.AddSimpleConsole(options => options.TimestampFormat = "[yyyy.MM.dd HH:mm:ss] "));
            serviceCollection.AddTouchPortalSdk<SamplePlugin>(configurationRoot);
            
            var serviceProvider = serviceCollection.BuildServiceProvider(true);

            var plugin = serviceProvider.GetRequiredService<SamplePlugin>();
            plugin.Connect();
            plugin.SendMessages();
        }
    }
}
