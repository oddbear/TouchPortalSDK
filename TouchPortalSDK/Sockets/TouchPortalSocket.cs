using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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

        public Func<string, Task> OnMessage { get; set; }
        public Action<Exception> OnClose { get; set; }

        public TouchPortalSocket(ILogger<TouchPortalSocket> logger,
                                 IOptions<TouchPortalOptions> options)
        {
            _logger = logger;
            _options = options.Value;

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listenerThread = new Thread(ListenerThreadSync) { IsBackground = false };
        }
        
        public async Task<bool> Connect()
        {
            try
            {
                //Connect
                var ipAddress = IPAddress.Parse(_options.IpAddress);
                var socketAddress = new IPEndPoint(ipAddress, _options.Port);
                _logger?.LogInformation("Connecting to TouchPortal.");
                await _socket.ConnectAsync(socketAddress);
                _logger?.LogInformation("TouchPortal connected.");

                //Setup streams:
                _streamWriter = new StreamWriter(new NetworkStream(_socket), Encoding.ASCII) {AutoFlush = true};
                _streamReader = new StreamReader(new NetworkStream(_socket), Encoding.UTF8);

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

        public async Task<string> Pair()
        {
            //Send pair message:
            var json = JsonSerializer.Serialize(new Dictionary<string, object>
            {
                ["type"] = "pair",
                ["id"] = _options.PluginId
            });

            _logger?.LogInformation("Sending pair message.");
            await SendMessage(json);

            //Wait for pairing response:
            var pairMessage = await _streamReader.ReadLineAsync();
            _logger?.LogInformation("Received pair response.");

            return pairMessage;
        }

        public bool Listen()
        {
            //Create listener thread:
            _logger?.LogInformation("Create listener.");
            _listenerThread.Start();
            _logger?.LogInformation("Listener created.");

            return _listenerThread.IsAlive;
        }

        public async Task<bool> SendMessage(Dictionary<string, object> message)
        {
            var jsonMessage = JsonSerializer.Serialize(message);
            return await SendMessage(jsonMessage);
        }

        public async Task<bool> SendMessage(string jsonMessage)
        {
            if (_streamWriter is null)
            {
                _logger?.LogWarning("StreamWriter was not initialized. Message cannot be sent.");
                return false;
            }

            try
            {
                await _streamWriter.WriteLineAsync(jsonMessage);
                return true;
            }
            catch (SocketException)
            {
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
            => ListenerThreadAsync().GetAwaiter().GetResult();

        private async Task ListenerThreadAsync()
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
                    var message = await _streamReader.ReadLineAsync()
                                  ?? throw new IOException("Server Socket Closed.");

                    _logger?.LogDebug(message);

                    await (OnMessage?.Invoke(message) ?? Task.CompletedTask);
                }
                catch (IOException exception)
                {
                    OnClose?.Invoke(exception);
                    return;
                }
                catch (Exception exception)
                {
                    _logger?.LogDebug(exception, "Something went wrong on the Listener Thread.");
                }
            }
        }
    }
}
