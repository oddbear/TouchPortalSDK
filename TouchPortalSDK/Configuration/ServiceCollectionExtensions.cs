using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TouchPortalSDK.Models;

namespace TouchPortalSDK.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static void AddTouchPortalSdk(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            //Add configuration:
            if (configuration != null)
                serviceCollection.Configure<TouchPortalOptions>(touchPortalOptions => configuration.GetSection("TouchPortalOptions").Bind(touchPortalOptions));
            serviceCollection.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IOptions<TouchPortalOptions>>().Value);
            
            //Add services, only expose Interfaces:
            serviceCollection.AddTransient(serviceProvider => new TouchPortalFactory(serviceProvider));
            serviceCollection.AddTransient<ITouchPortalSocketFactory>(serviceProvider => serviceProvider.GetRequiredService<TouchPortalFactory>());
            serviceCollection.AddTransient<ITouchPortalClientFactory>(serviceProvider => serviceProvider.GetRequiredService<TouchPortalFactory>());
        }
    }
}
