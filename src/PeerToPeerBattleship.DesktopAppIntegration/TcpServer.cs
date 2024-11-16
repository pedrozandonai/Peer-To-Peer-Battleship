using PeerToPeerBattleship.Core.CustomLogger.Abstraction;
using Serilog;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PeerToPeerBattleship.DesktopAppIntegration
{
    public class TcpServer
    {
        private readonly ILogger _logger;
        private readonly BlockingCollection<string> _messageQueue = new();

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

            // Inicia uma thread separada para processar as mensagens da fila
            Task.Run(ProcessMessages);

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                using var stream = client.GetStream();
                using var reader = new StreamReader(stream, Encoding.UTF8);

                string message = reader.ReadToEnd();
                _logger.Information($"Mensagem recebida: {message}");

                // Adiciona a mensagem na fila para processamento posterior
                _messageQueue.Add(message);
            }
        }

        private void ProcessMessages()
        {
            foreach (var message in _messageQueue.GetConsumingEnumerable())
            {
                // Processa cada mensagem fora do loop do servidor TCP
                _logger.Information($"Processando mensagem: {message}");

                // Aqui você pode executar qualquer lógica necessária com a mensagem
                UseMessage(message);
            }
        }

        private void UseMessage(string message)
        {
            // Exemplo de uso da mensagem
            _logger.Information($"Mensagem em uso: {message}");
        }
    }
}
