using System;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IServiceProvider _serviceProvider;

        public TouchPortalFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc cref="ITouchPortalSocketFactory" />
        public ITouchPortalSocket Create(IMessageHandler messageHandler)
        {
            if (messageHandler is null)
                throw new ArgumentNullException(nameof(messageHandler));

            return ActivatorUtilities.CreateInstance<TouchPortalSocket>(_serviceProvider, messageHandler);
        }

        /// <inheritdoc cref="ITouchPortalClientFactory" />
        public ITouchPortalClient Create(ITouchPortalEventHandler eventHandler)
        {
            if (eventHandler is null)
                throw new ArgumentNullException(nameof(eventHandler));

            return ActivatorUtilities.CreateInstance<TouchPortalClient>(_serviceProvider, eventHandler);
        }
    }
}