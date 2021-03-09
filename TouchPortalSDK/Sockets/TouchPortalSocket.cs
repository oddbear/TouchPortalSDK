using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Logging;
using TouchPortalSDK.Models;

namespace TouchPortalSDK.Sockets
{
    public class TouchPortalSocket : ITouchPortalSocket
    {
        private readonly ILogger<TouchPortalSocket> _logger;
        private readonly TouchPortalOptions _options;
        private readonly Socket _socket;
        private readonly Thread _listenerThread;

        private StreamReader _streamReader;
        private StreamWriter _streamWriter;

        private Action<string> _onMessageCallBack;

        public TouchPortalSocket(TouchPortalOptions options,
                                 ILogger<TouchPortalSocket> logger = null)
        {
            if (string.IsNullOrWhiteSpace(options.PluginId))
                throw new InvalidOperationException($"{nameof(options.PluginId)} on {nameof(TouchPortalOptions)} cannot be null.");

            _logger = logger;
            _options = options;

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listenerThread = new Thread(ListenerThreadSync) { IsBackground = false };
        }
        
        public bool Connect()
        {
            try
            {
                //Connect
                var ipAddress = IPAddress.Parse(_options.IpAddress);
                var socketAddress = new IPEndPoint(ipAddress, _options.Port);

                _socket.Connect(socketAddress);
                _logger?.LogInformation("TouchPortal connected.");

                //Setup streams:
                _streamWriter = new StreamWriter(new NetworkStream(_socket), Encoding.ASCII) {AutoFlush = true};
                _streamReader = new StreamReader(new NetworkStream(_socket), Encoding.UTF8);
                _logger?.LogInformation("Streams created.");

                return _socket.Connected;
            }
            catch (IOException exception)
            {
                //Ex. System.IO.IOException: The operation is not allowed on non-connected sockets
                _logger?.LogWarning(exception, "Socket was not open, stream creation failed.");
                return false;
            }
            //Warning: SocketErrors in .Net might depend on OS and Runtime: https://blog.jetbrains.com/dotnet/2020/04/27/socket-error-codes-depend-runtime-operating-system/
            catch (SocketException exception)
                when (exception.SocketErrorCode == SocketError.ConnectionRefused)
            {
                //Could not connect to TouchPortal, ex. TouchPortal is not running.
                //Ex. No connection could be made because the target machine actively refused it. 127.0.0.1:12136
                _logger?.LogWarning(exception, "Could not connect to TouchPortal, connection refused. TouchPortal might not be running.");

                return false;
            }
            catch (SocketException exception)
            {
                _logger?.LogWarning(exception, $"Could not connect to TouchPortal with error code: '{exception.SocketErrorCode}'.");

                return false;
            }
        }
        
        public bool Listen(Action<string> onMessageCallBack)
        {
            _onMessageCallBack = onMessageCallBack;
            _logger?.LogInformation("Callback method set.");

            //Create listener thread:
            _listenerThread.Start();
            _logger?.LogInformation("Listener started.");

            return _listenerThread.IsAlive;
        }
        
        public bool SendMessage(string jsonMessage)
        {
            if (!_socket.Connected)
            {
                _logger?.LogWarning("Socket not connected to TouchPortal.");
                return false;
            }

            if (_streamWriter is null)
            {
                _logger?.LogWarning("StreamWriter was not initialized. Message cannot be sent.");
                return false;
            }

            try
            {
                _streamWriter.WriteLine(jsonMessage);
                _logger?.LogDebug("Message sent.");
                return true;
            }
            catch (SocketException exception)
            {
                _logger?.LogWarning(exception, "Socket exception");
                return false;
            }
        }

        void ITouchPortalSocket.CloseSocket()
        {
            _listenerThread.Interrupt();
            _streamWriter?.Close();
            _streamReader?.Close();
            _socket.Close();
        }

        private void ListenerThreadSync()
        {
            if (_streamReader is null)
            {
                _logger?.LogWarning("StreamReader was not initialized. Listener thread cannot be started.");
                return;
            }

            _logger?.LogInformation("Listener thread created and started.");

            while (true)
            {
                try
                {
                    var message = _streamReader.ReadLine();
                    _logger?.LogDebug(message);

                    _onMessageCallBack?.Invoke(message);
                }
                catch (Exception exception)
                {
                    _logger?.LogDebug(exception, "Something went wrong on the Listener Thread.");
                }
            }
        }
    }
}
