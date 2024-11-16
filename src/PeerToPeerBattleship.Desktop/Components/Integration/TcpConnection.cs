using PeerToPeerBattleship.Desktop.Components.Integration.CustomLogger;
using PeerToPeerBattleship.Desktop.Components.Integration.CustomLogger.Abstraction;
using Serilog;
using System.Net.Sockets;

namespace PeerToPeerBattleship.Desktop.Components.Integration
{
    public class TcpConnection
    {
        private readonly ILogger _logger;

        public TcpConnection(IContextualLogger<TcpConnection> contextualLogger)
        {
            _logger = contextualLogger.Logger;
        }

        public void ConnectToBackEnd(string messageToSend)
        {
            try
            {
                _logger.Information("Iniciando conexão com o back-end.");
                TcpClient client = new TcpClient("localhost", 6000);
                using var stream = client.GetStream();
                using var writer = new StreamWriter(stream) { AutoFlush = true };
                writer.WriteLine(messageToSend);
                _logger.Information("Mensagem enviada com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogExceptionError("Conexão com o back-end falhou.", ex);
            }
        }
    }
}
