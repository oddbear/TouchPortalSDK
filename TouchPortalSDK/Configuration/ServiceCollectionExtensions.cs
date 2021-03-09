using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TouchPortalSDK.Models;
using TouchPortalSDK.Sockets;

namespace TouchPortalSDK.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static void AddTouchPortalPlugin<TTouchPortalPlugin>(this IServiceCollection serviceCollection, IConfiguration configuration)
            where TTouchPortalPlugin : class, ITouchPortalPlugin
        {
            //Add configuration:
            serviceCollection.Configure<TouchPortalOptions>(touchPortalOptions => configuration.GetSection("TouchPortalClientSettings").Bind(touchPortalOptions));
            serviceCollection.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IOptions<TouchPortalOptions>>().Value);
            
            //Force ITouchPortalPlugin to be the same as the TTouchPortalPlugin singleton:
            serviceCollection.AddSingleton<ITouchPortalPlugin>(serviceProvider => serviceProvider.GetRequiredService<TTouchPortalPlugin>());

            //Creates a singleton of TTouchPortalPlugin, and registering it on the singleton of ITouchPortalClient
            serviceCollection.AddSingleton(serviceProvider =>
            {
                //Resolve ITouchPortalClient:
                var touchPortalClient = serviceProvider.GetRequiredService<ITouchPortalClient>();

                //If ITouchPortalClient is a parameter we must ensure that the ctor parameter is the same instance as the one we register the plugin on:
                //The ActivatorUtilities will automatically pick the best constructor.
                //If we do this for socket, then the behaviour would be different to what Transient mean in ServiceProvider.
                //Other containers might have Transient and Unique that are not the same behaviour.
                var parameters = typeof(TTouchPortalPlugin)
                    .GetConstructors()
                    .SelectMany(constructorInfo => constructorInfo.GetParameters())
                    .Any(parameterInfo => parameterInfo.ParameterType == typeof(ITouchPortalPlugin))
                    ? new object[] { touchPortalClient }
                    : Array.Empty<object>();

                //This makes a new unique instance, since TTouchPortalPlugin is registered as singleton, this will only happen once:
                var touchPortalPlugin = ActivatorUtilities.CreateInstance<TTouchPortalPlugin>(serviceProvider, parameters);
                
                //Make a two way binding between the client and the plugin:
                touchPortalClient.RegisterPlugin(touchPortalPlugin);

                //Return the resolved plugin:
                return touchPortalPlugin;
            });

            //Add services, only expose Interfaces:
            serviceCollection.AddSingleton<ITouchPortalSocket, TouchPortalSocket>();
            serviceCollection.AddSingleton<ITouchPortalClient, TouchPortalClient>();
        }
    }
}
