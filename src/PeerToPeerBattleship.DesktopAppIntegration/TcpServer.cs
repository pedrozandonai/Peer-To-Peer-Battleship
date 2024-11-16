using PeerToPeerBattleship.Core.CustomLogger.Abstraction;
using Serilog;
using System.Net;
using System.Net.Sockets;

namespace PeerToPeerBattleship.DesktopAppIntegration
{
    public class TcpServer
    {
        private readonly ILogger _logger;

        public TcpServer(IContextualLogger<TcpServer> contextualLogger)
        {
            _logger = contextualLogger.Logger;
        }

        public void ConnectToDesktopApp()
        {
            _logger.Information("Iniciando Tcp Listener");

            TcpListener server = new(IPAddress.Any, 6000);
            server.Start();

            _logger.Information("Iniciando conexão com a aplicação desktop");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                using var stream = client.GetStream();
                using var reader = new StreamReader(stream);
                _logger.Information(reader.ReadToEnd());
            }
        }
    }
}
