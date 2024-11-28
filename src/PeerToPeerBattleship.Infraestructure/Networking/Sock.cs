using PeerToPeerBattleship.Core.CustomLogger;
using PeerToPeerBattleship.Core.CustomLogger.Abstraction;
using PeerToPeerBattleship.Infraestructure.Networking.Abstractions;
using Serilog;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PeerToPeerBattleship.Infraestructure.Networking
{
    public class Sock : ISock
    {
        public string LocalMachineIP { get; set; }
        public string RemoteMachineIp { get; set; }
        public short SelectedPort { get; set; }
        public event Action<string> MessageReceived;
        public event Action ConnectionClosed;

        private TcpClient _client;
        private NetworkStream _stream;

        private readonly ILogger _logger;

        public Sock(IContextualLogger<Sock> contextualLogger)
        {
            _logger = contextualLogger.Logger;
            LocalMachineIP = GetLocalMachineIp();
        }

        public async Task StartServerAsync(short port)
        {
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                _logger.Information("Servidor iniciado no IP: {0} na porta {1}. Aguardando por conexões...", LocalMachineIP, port);

                _client = await listener.AcceptTcpClientAsync();
                RemoteMachineIp = ((IPEndPoint)_client.Client.RemoteEndPoint!).Address.ToString();
                SelectedPort = port;

                _logger.Information("Cliente conectado. Endereço IP: {0}", RemoteMachineIp);

                _stream = _client.GetStream();
                _ = ReceiveMessagesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogExceptionError("Ocorreu um erro ao tentar iniciar o servidor.", ex);
            }
        }

        public async Task ConnectToServerAsync(string serverIp, short port)
        {
            RemoteMachineIp = serverIp;
            SelectedPort = port;

            _client = new TcpClient();
            await _client.ConnectAsync(serverIp, port);
            _logger.Information("Conectado ao IP {0} na Porta: {1}.", serverIp, port);

            _stream = _client.GetStream();
            _ = ReceiveMessagesAsync();
        }

        public async Task SendMessageAsync(string message)
        {
            if (_stream == null) throw new InvalidOperationException("Não há conexão ativa.");
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await _stream.WriteAsync(buffer, 0, buffer.Length);
        }

        private async Task ReceiveMessagesAsync()
        {
            byte[] buffer = new byte[1024];

            try
            {
                while (true)
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        ConnectionClosed?.Invoke();
                        break;
                    }

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    MessageReceived?.Invoke(message);
                }
            }
            catch
            {
                ConnectionClosed?.Invoke();
            }
        }

        public void CloseConnection()
        {
            _stream?.Close();
            _client?.Close();
            ConnectionClosed?.Invoke();
        }

        private string GetLocalMachineIp()
        {
            try
            {
                // Obtém o IP local da máquina
                var host = Dns.GetHostEntry(Dns.GetHostName());
                var localIp = Array.Find(host.AddressList, ip => ip.AddressFamily == AddressFamily.InterNetwork);
                return localIp?.ToString() ?? "No IPv4 Address Found";
            }
            catch (Exception ex)
            {
                _logger.LogExceptionError("Erro ao tentar recuperar endereço de Ip da máquina local.", ex);
                return "Error";
            }
        }
    }
}
