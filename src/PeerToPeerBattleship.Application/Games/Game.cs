using PeerToPeerBattleship.Application.Games.Abstractions;
using PeerToPeerBattleship.Application.Matches;
using PeerToPeerBattleship.Core.Configurations;
using PeerToPeerBattleship.Core.CustomLogger.Abstraction;
using PeerToPeerBattleship.Core.Inputs.Abstractions;
using PeerToPeerBattleship.Infraestructure.Networking.Abstractions;
using Serilog;

namespace PeerToPeerBattleship.Application.Games
{
    public class Game : IGame
    {
        private readonly ILogger _logger;
        private readonly IUserInputHandler _userInputHandler;
        private readonly ISock _sock;
        private readonly ApplicationSettings _applicationSettings;

        public Game(IContextualLogger<Game> contextualLogger, IUserInputHandler userInputHandler, ISock sock, ApplicationSettings applicationSettings)
        {
            _logger = contextualLogger.Logger;
            _userInputHandler = userInputHandler;
            _sock = sock;
            _applicationSettings = applicationSettings;
        }

        public async Task Create()
        {
            var creationType = SelectCreationMode();

            switch (creationType)
            {
                case 1:
                    await StartNewGameAsync();
                    break;
                case 2:
                    await JoinExistingGameAsync();
                    break;
                case 9:
                    return;
                default:
                    throw new InvalidOperationException("Operação não reconhecida pelo programa.");
            }
        }

        private short SelectCreationMode()
        {
            Console.WriteLine("*-----------------------------------*");
            Console.WriteLine("|    Como você deseja começar?      |");
            Console.WriteLine("|    1 - Criar uma nova partida     |");
            Console.WriteLine("|2 - Juntar a uma partida existente |");
            Console.WriteLine("|       9 - Encerrar programa       |");
            Console.WriteLine("*-----------------------------------*");
            var creationOption = _userInputHandler.ReadShort("Seleciona uma das opções: ");

            switch (creationOption)
            {
                case 1 or 2 or 9:
                    return creationOption;
                default:
                    _logger.Error("Opção não reconhecida pelo programa, por favor, digite uma opção válida.");
                    return SelectCreationMode();
            }
        }

        private async Task StartNewGameAsync()
        {
            short port = _userInputHandler.ReadShort("Digite a porta para iniciar o servidor: ");

            if (!_applicationSettings.GameTestMode)
            {
                await _sock.StartServerAsync(port);

                _sock.MessageReceived += OnMessageReceived;
                _sock.ConnectionClosed += OnConnectionClosed;
            }

            _logger.Information("Servidor iniciado e pronto para jogar.");
            GameLoop();
        }

        private async Task JoinExistingGameAsync()
        {
            string serverIp = _userInputHandler.ReadIpAddress("Digite o IP do servidor: ");
            short port = _userInputHandler.ReadShort("Digite a porta do servidor: ");

            if (!_applicationSettings.GameTestMode)
            {
                await _sock.ConnectToServerAsync(serverIp, port);

                _sock.MessageReceived += OnMessageReceived;
                _sock.ConnectionClosed += OnConnectionClosed;
            }

            _logger.Information("Conectado ao jogo. Pronto para jogar.");
            GameLoop();
        }

        private void GameLoop()
        {
            while (true)
            {
                var match = new Match();
                match.DisplayBoard(match.UserBoard);
                Thread.Sleep(10000);

                string input = "a fazer ainda";
                if (!_applicationSettings.GameTestMode)
                {
                    _sock.SendMessageAsync(input).Wait();

                    if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    {
                        _sock.CloseConnection();
                        break;
                    }
                }
            }
        }

        private void OnMessageReceived(string message)
        {
            _logger.Information("Mensagem recebida: {0}", message);
            // TODO: Ver como isso irá funcionar, provavelmente um padrão strategy faria sentido aqui.
        }

        private void OnConnectionClosed()
        {
            _logger.Warning("Conexão encerrada.");
        }
    }
}
