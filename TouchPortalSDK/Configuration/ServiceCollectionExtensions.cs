using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TouchPortalSDK.Models;

namespace TouchPortalSDK.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static void AddTouchPortalSdk<TTouchPortalEventHandler>(this IServiceCollection serviceCollection, IConfiguration configuration)
            where TTouchPortalEventHandler : class, ITouchPortalEventHandler
        {
            //Add configuration:
            serviceCollection.Configure<TouchPortalOptions>(touchPortalOptions => configuration.GetSection("TouchPortalOptions").Bind(touchPortalOptions));
            serviceCollection.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IOptions<TouchPortalOptions>>().Value);

            //Force ITouchPortalPlugin to be the same as the TTouchPortalPlugin singleton:
            serviceCollection.AddSingleton<TTouchPortalEventHandler>();
            serviceCollection.AddSingleton<ITouchPortalEventHandler>(serviceProvider => serviceProvider.GetRequiredService<TTouchPortalEventHandler>());

            //Add services, only expose Interfaces:
            serviceCollection.AddTransient<TouchPortalFactory>();
            serviceCollection.AddTransient<ITouchPortalSocketFactory>(serviceProvider => serviceProvider.GetRequiredService<TouchPortalFactory>());
            serviceCollection.AddTransient<ITouchPortalClientFactory>(serviceProvider => serviceProvider.GetRequiredService<TouchPortalFactory>());
        }
    }
}
