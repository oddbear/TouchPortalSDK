using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;
using TouchPortalSDK.Interfaces;

namespace TouchPortalSDK.Clients
{
    public class TouchPortalSocket : ITouchPortalSocket
    {
        public bool IsConnected { get => _socket?.Connected ?? false; }

        private readonly TouchPortalOptions _options;
        private readonly IMessageHandler _messageHandler;
        private readonly ILogger<TouchPortalSocket> _logger;
        private readonly Socket _socket;
        private readonly Thread _listenerThread;

        private StreamWriter _streamWriter;
        private StreamReader _streamReader;

        public TouchPortalSocket(TouchPortalOptions options,
                                 IMessageHandler messageHandler,
                                 ILoggerFactory loggerFactory = null)
        {
            _options = options;
            _messageHandler = messageHandler;
            _logger = loggerFactory?.CreateLogger<TouchPortalSocket>();

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listenerThread = new Thread(ListenerThreadSync) { IsBackground = false };
        }

        /// <inheritdoc cref="ITouchPortalSocket" />
        bool ITouchPortalSocket.Connect()
        {
            try
            {
                //Connect
                var ipAddress = IPAddress.Parse(_options.IpAddress);
                var socketAddress = new IPEndPoint(ipAddress, _options.Port);

                _socket.Connect(socketAddress);

                _logger?.LogInformation("TouchPortal connected.");

                //The encoder needs to be without a BOM / Utf8 Identifier:
                var encoder = new UTF8Encoding(false);

                //Setup streams:
                _streamWriter = new StreamWriter(new NetworkStream(_socket), encoder) {AutoFlush = true, };
                _streamReader = new StreamReader(new NetworkStream(_socket), encoder);

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
                //Could not connect to Touch Portal, ex. Touch Portal is not running.
                //Ex. No connection could be made because the target machine actively refused it. 127.0.0.1:12136
                _logger?.LogWarning(exception, "Could not connect to Touch Portal, connection refused. Touch Portal might not be running.");

                return false;
            }
            catch (SocketException exception)
            {
                _logger?.LogWarning(exception, $"Could not connect to Touch Portal with error code: '{exception.SocketErrorCode}'.");

                return false;
            }
        }

        /// <inheritdoc cref="ITouchPortalSocket" />
        bool ITouchPortalSocket.Listen()
        {
            _logger?.LogInformation("Callback method set.");

            //Create listener thread:
            _listenerThread.Start();
            _logger?.LogInformation("Listener started.");

            return _listenerThread.IsAlive;
        }

        /// <inheritdoc cref="ITouchPortalSocket" />
        bool ITouchPortalSocket.SendMessage(string jsonMessage)
        {
            if (!_socket.Connected)
            {
                _logger?.LogWarning("Socket not connected to Touch Portal.");
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

        /// <inheritdoc cref="ITouchPortalSocket" />
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

            try
            {
                while (true)
                {
                    try
                    {
                        var message = _streamReader.ReadLine()
                                      ?? throw new IOException("Socket closed.");

                        _logger?.LogDebug(message);

                        _messageHandler.OnMessage(message);
                    }
                    catch (IOException exception)
                    {
                        _messageHandler.Close("Connection Terminated.", exception);
                        return;
                    }
                    catch (Exception exception)
                    {
                        _logger?.LogDebug(exception, "Something went wrong on the Listener Thread.");
                    }
                }
            }
            catch (ThreadInterruptedException exception)
            {
                _logger?.LogDebug(exception, "The Listener Thread was interrupted.");
            }
        }
    }
}
