using PeerToPeerBattleship.Application.Games.Abstractions;
using PeerToPeerBattleship.Application.Games.Strategy.Strategies;
using PeerToPeerBattleship.Application.Games.Strategy;
using PeerToPeerBattleship.Application.Matches;
using PeerToPeerBattleship.Application.Ships.Model;
using PeerToPeerBattleship.Core.Configurations;
using PeerToPeerBattleship.Core.CustomLogger.Abstraction;
using PeerToPeerBattleship.Core.Inputs.Abstractions;
using PeerToPeerBattleship.Infraestructure.Networking.Abstractions;
using Serilog;
using System.Text.Json;
using PeerToPeerBattleship.Application.Games.Strategy.Abstractions;

namespace PeerToPeerBattleship.Application.Games
{
    public class Game : IGame
    {
        private readonly ILogger _logger;
        private readonly IUserInputHandler _userInputHandler;
        private readonly ISock _sock;
        private readonly ApplicationSettings _applicationSettings;

        public Match Match { get; set; }

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
            GameLoop(1);
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
            GameLoop(2);
        }

        private void GameLoop(short creationType)
        {
            if (!_applicationSettings.PeerToPeerTestMode)
            {
                Match = Match.Create(_sock.LocalMachineIP, _sock.RemoteMachineIp, _userInputHandler);
                Match.CreateUserBoard();
                Match.DisplayBoard(Match.UserBoard);
            }

            while (true)
            {
                string input = Console.ReadLine()!;
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

            try
            {
                GameStrategyContext strategyContext;
                IGameStrategy? strategy;

                var shipsDto = JsonSerializer.Deserialize<ShipsDto>(message);
                if (shipsDto != null
                    && shipsDto.Ships?.Any() == true)
                {
                    strategy = new ReceiveShipsStrategy();
                }
                else
                {
                    strategy = new DataNotRecognizedStrategy();
                }

                strategyContext = new GameStrategyContext(strategy);
                strategyContext.ExecuteStrategy(message, Match);
            }
            catch (JsonException ex)
            {
                _logger.Error("Erro ao desserializar mensagem: {0}. Mensagem: {1}", ex.Message, message);
            }
        }

        private void OnConnectionClosed()
        {
            _logger.Warning("Conexão encerrada.");
        }
    }
}
