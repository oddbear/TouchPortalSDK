using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SampleUtils;
using TouchPortalSDK.Configuration;
using TouchPortalSDK.Events;

//Used in debug to copy the entry.tp file if changed, and restart Touch Portal:
EntryCopy.RefreshEntryFile();

//Build configuration:
var configurationRoot = new ConfigurationBuilder()
    .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
    .AddJsonFile("appsettings.json")
    .Build();

//Standard method for build a ServiceProvider in .Net:
var serviceCollection = new ServiceCollection();
serviceCollection.AddLogging(configure =>
{
    configure.AddSimpleConsole(options => options.TimestampFormat = "[yyyy.MM.dd HH:mm:ss] ");
    configure.AddConfiguration(configurationRoot.GetSection("Logging"));
});

//Registering the Plugin to the IoC container:
serviceCollection.AddTouchPortalSdk(configurationRoot);
serviceCollection.AddSingleton<EventsPlugin>();

var serviceProvider = serviceCollection.BuildServiceProvider(true);

//Use your IoC framework to resolve the plugin with it's dependencies,
// or you can use 'TouchPortalFactory.CreateClient' to get started manually:
var plugin = serviceProvider.GetRequiredService<EventsPlugin>();
plugin.Run();
