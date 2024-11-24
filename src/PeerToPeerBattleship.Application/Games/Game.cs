using PeerToPeerBattleship.Application.Games.Abstractions;
using PeerToPeerBattleship.Application.Games.Strategy;
using PeerToPeerBattleship.Application.Games.Strategy.Abstractions;
using PeerToPeerBattleship.Application.Games.Strategy.Strategies;
using PeerToPeerBattleship.Application.Matches;
using PeerToPeerBattleship.Application.Ships.Domain;
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
        private bool ConnectionClosed { get; set; }

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
                    await JoinExistingGameAsync(2);
                    break;

                case 3:
                    await JoinExistingGameAsync(3);
                    break;

                case 9:
                    return;

                default:
                    throw new InvalidOperationException("Operação não reconhecida pelo programa.");
            }
        }

        private short SelectCreationMode()
        {
            // Procura por partidas não finalizadas para carregar no objeto Match novamente.
            _logger.Information("Procurando por partidas não finalizadas...");
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string folderPath = Path.Combine(documentsPath, "PeerToPeerBattleShip");

            Match? unfinishedMatch = Match.FindAndLoadUnfinishedMatch(folderPath);

            Console.WriteLine("*-----------------------------------*");
            Console.WriteLine("|      Como você deseja jogar?      |");
            Console.WriteLine("|    1 - Criar uma nova partida     |");
            Console.WriteLine("|2 - Juntar a uma partida existente |");
            if (unfinishedMatch != null)
            {
                Console.WriteLine("|3 - Reconectar a partida encerrada |");
            }
            Console.WriteLine("|       9 - Encerrar programa       |");
            Console.WriteLine("*-----------------------------------*");
            var creationOption = _userInputHandler.ReadShort("Selecione uma das opções: ");

            switch (creationOption)
            {
                case 1 or 2 or 9:
                    return creationOption;
                case 3:
                    if (unfinishedMatch != null) Match = unfinishedMatch;

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
            await GameLoop(1);
        }

        private async Task JoinExistingGameAsync(short creationOption)
        {
            string remoteIp = string.Empty;
            short port = 0;
            if (creationOption != 3)
            {
                remoteIp = _userInputHandler.ReadIpAddress("Digite o IP do servidor: ");
                port = _userInputHandler.ReadShort("Digite a porta do servidor: ");
            }
            else
            {
                remoteIp = Match.RemoteMachineIp!;
                port = Match.SelectedPort;
            }

            if (!_applicationSettings.GameTestMode)
            {
                await _sock.ConnectToServerAsync(remoteIp, port);

                _sock.MessageReceived += OnMessageReceived;
                _sock.ConnectionClosed += OnConnectionClosed;
            }

            _logger.Information("Conectado ao jogo. Pronto para jogar.");
            await GameLoop(creationOption);
        }

        private async Task GameLoop(short creationType)
        {
            if (creationType != 3)
            {
                if (!_applicationSettings.PeerToPeerTestMode)
                {
                    Match = Match.Create(_sock.LocalMachineIP, _sock.RemoteMachineIp, _userInputHandler);
                    Match.ShipsCreationMethod();
                    Match.DisplayBoard(Match.UserBoard);
                }
            }

            Match.SelectedPort = _sock.SelectedPort;

            if (creationType == 1)
                Match.IpTurn = Match.LocalMachineIp!;

            // Verificar a necessidade dessa verificação e espera.
            if (creationType == 2)
            {
                Match.IpTurn = Match.RemoteMachineIp!;
                Console.WriteLine("Aguardando até que o oponente envie os navios.");
                while (Match.EnemyBoard == null) if (Match.EnemyBoard != null) break;
            }

            await _sock.SendMessageAsync(await Match.SerializeShipsDto(Match.UserBoard.Ships, CancellationToken.None));

            while (true || !ConnectionClosed)
            {
                if (!_applicationSettings.GameTestMode)
                {
                    if (Match.IpTurn.Equals(Match.LocalMachineIp))
                    {
                        Match.DisplayBoards(Match.UserBoard, Match.EnemyBoard);
                        var attackPosition = Match.AttackEnemyShip();
                        Match.EnemyBoard.Attack(attackPosition.X, attackPosition.Y);

                        await _sock.SendMessageAsync(string.Format("{0}{1}", attackPosition.X, attackPosition.Y));

                        if (Match.EnemyBoard.AllShipsSunk)
                        {
                            Match.IsMatchOver = true;

                            Match.MatchWinnerIp = Match.LocalMachineIp;

                            _sock.CloseConnection();

                            Console.WriteLine("Parabéns, você é o ganhador da partida! ☺️");
                        }

                        Match.IpTurn = Match.RemoteMachineIp!;

                        Match.SaveToFile();
                    }
                }
            }
        }

        private void OnMessageReceived(string message)
        {
            if (!_applicationSettings.IsProductionEnvironment)
            {
                _logger.Information("Mensagem recebida: {0}", message);
            }

            try
            {
                GameStrategyContext strategyContext;
                IGameStrategy? strategy = new DataNotRecognizedStrategy(_logger);

                if (message.Length == 2)
                {
                    strategy = new ReceiveAttackPositionStrategy(_logger);

                    strategyContext = new GameStrategyContext(strategy);
                    Match = strategyContext.ExecuteStrategy(message, Match);

                    return;
                }

                try
                {
                    var enemyShipsDto = Ship.DeserializeShips(message);
                    if (enemyShipsDto != null
                        && enemyShipsDto.Any() == true)
                    {
                        strategy = new ReceiveShipsStrategy(_logger);
                    }

                    strategyContext = new GameStrategyContext(strategy);
                    Match = strategyContext.ExecuteStrategy(message, Match);

                    return;
                }
                catch(Exception){ }
            }
            catch (Exception ex)
            {
                _logger.Error("Uma exceção foi lançada: {0}. Mensagem recebida pelo peer: {1}", ex.Message, message);
            }
        }

        private async void OnConnectionClosed()
        {
            const int maxRetries = 3;
            int attempt = 0;

            Match.SaveToFile();

            _logger.Warning("Conexão encerrada. Tentando reconectar...");

            while (attempt < maxRetries)
            {
                attempt++;
                try
                {
                    _logger.Information("Tentativa {0} de reconexão...", attempt);

                    // Tenta reconectar ao servidor
                    await _sock.ConnectToServerAsync(Match.RemoteMachineIp!, Match.SelectedPort);

                    _logger.Information("Reconexão bem-sucedida.");
                    return;
                }
                catch (Exception ex)
                {
                    _logger.Warning("Tentativa de reconexão {0} falhou: {1}", attempt, ex.Message);
                }

                await Task.Delay(3000); // Aguarda 3 segundos antes de tentar novamente
            }

            _logger.Error("Não foi possível reconectar após {0} tentativas.", maxRetries);
            ConnectionClosed = true;
        }
    }
}
