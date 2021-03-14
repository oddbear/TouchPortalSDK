using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TouchPortalSDK.Models;
using TouchPortalSDK.Sockets;

namespace TouchPortalSDK.Configuration
{
    /// <summary>
    /// Factory interface for creating a TouchPortal client.
    /// </summary>
    public interface ITouchPortalClientFactory
    {
        /// <summary>
        /// Create a TouchPortal Client
        /// </summary>
        /// <param name="eventHandler">Handler the events from TouchPortal, normally the plugin instance.</param>
        /// <returns>TouchPortal Client</returns>
        ITouchPortalClient Create(ITouchPortalEventHandler eventHandler);
    }

    /// <summary>
    /// Factory interface for creating a TouchPortal socket.
    /// </summary>
    public interface ITouchPortalSocketFactory
    {
        /// <summary>
        /// Create a TouchPortal Socket
        /// </summary>
        /// <param name="messageHandler">Handler the json events from the Socket, normally the client instance.</param>
        /// <returns>TouchPortal Socket</returns>
        ITouchPortalSocket Create(IMessageHandler messageHandler);
    }

    /// <summary>
    /// Factories are a pattern that works well with callbacks.
    /// </summary>
    public class TouchPortalFactory : ITouchPortalSocketFactory, ITouchPortalClientFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly TouchPortalOptions _options;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Constructor used if registered through AddTouchPortalSdk.
        /// </summary>
        public TouchPortalFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        /// <summary>
        /// Private so we don't expose the socket factory.
        /// </summary>
        private TouchPortalFactory(TouchPortalOptions options, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _options = options ?? new TouchPortalOptions();
        }

        /// <summary>
        /// Factory for creating the TouchPortal client.
        /// </summary>
        /// <param name="options">Optional options, if null, default values are selected.</param>
        /// <param name="loggerFactory">Optional logger factory, if null, no logger is created.</param>
        /// <returns></returns>
        public static ITouchPortalClientFactory Create(TouchPortalOptions options = null, ILoggerFactory loggerFactory = null)
            => new TouchPortalFactory(options, loggerFactory);
        
        /// <inheritdoc cref="ITouchPortalSocketFactory" />
        ITouchPortalSocket ITouchPortalSocketFactory.Create(IMessageHandler messageHandler)
        {
            if (messageHandler is null)
                throw new ArgumentNullException(nameof(messageHandler));
            
            return _serviceProvider is null
                //Manual:
                ? new TouchPortalSocket(_options, messageHandler, _loggerFactory)
                //Through ServiceProvider:
                : ActivatorUtilities.CreateInstance<TouchPortalSocket>(_serviceProvider, messageHandler);
        }

        /// <inheritdoc cref="ITouchPortalClientFactory" />
        ITouchPortalClient ITouchPortalClientFactory.Create(ITouchPortalEventHandler eventHandler)
        {
            if (eventHandler is null)
                throw new ArgumentNullException(nameof(eventHandler));
            
            return _serviceProvider is null
                ? new TouchPortalClient(eventHandler, this, _loggerFactory)
                : ActivatorUtilities.CreateInstance<TouchPortalClient>(_serviceProvider, eventHandler);
        }
    }
}