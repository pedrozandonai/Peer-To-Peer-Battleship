using PeerToPeerBattleship.Application.Games.Abstractions;
using PeerToPeerBattleship.Application.Games.Strategy;
using PeerToPeerBattleship.Application.Games.Strategy.Abstractions;
using PeerToPeerBattleship.Application.Games.Strategy.Strategies;
using PeerToPeerBattleship.Application.Matches;
using PeerToPeerBattleship.Application.Ships.Domain;
using PeerToPeerBattleship.Application.UsersSettings.Domain;
using PeerToPeerBattleship.Core.Configurations;
using PeerToPeerBattleship.Core.CustomLogger;
using PeerToPeerBattleship.Core.CustomLogger.Abstraction;
using PeerToPeerBattleship.Core.Extensions;
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
        private readonly UserSettings _userSettings;
        private bool ConnectionClosed { get; set; }

        public Match Match { get; set; }

        public Game(IContextualLogger<Game> contextualLogger, IUserInputHandler userInputHandler, ISock sock, ApplicationSettings applicationSettings, UserSettings userSettings)
        {
            _logger = contextualLogger.Logger;
            _userInputHandler = userInputHandler;
            _sock = sock;
            _applicationSettings = applicationSettings;
            _userSettings = userSettings;
        }

        public async Task Create()
        {
            var creationType = SelectCreationMode();

            while (creationType != 9)
            {
                switch (creationType)
                {
                    case 1:
                        await StartNewGameAsync();
                        break;

                    case 2:
                        await JoinExistingGameAsync();
                        break;

                    case 3:
                        await ReconnectToGameAsync();
                        break;

                    case 9:
                        break;

                    default:
                        throw new InvalidOperationException("Operação não reconhecida pelo programa.");
                }
            }
        }

        private short SelectCreationMode()
        {
            ConsoleExtension.Clear();
            // Procura por partidas não finalizadas para carregar no objeto Match novamente.
            _logger.Information("Procurando por partidas não finalizadas...");
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string folderPath = Path.Combine(documentsPath, "PeerToPeerBattleShip");

            Match? unfinishedMatch = Match.FindAndLoadUnfinishedMatch(folderPath, _userSettings);

            Console.WriteLine("*-----------------------------------*");
            Console.WriteLine("|      Como você deseja jogar?      |");
            Console.WriteLine("|    1 - Criar uma nova partida     |");
            Console.WriteLine("|2 - Juntar a uma partida existente |");
            if (unfinishedMatch != null)
            {
                Console.WriteLine("|3 - Reconectar a partida encerrada |");
            }
            Console.WriteLine("|             9 - Voltar            |");
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
            await ConsoleExtension.ClearAsync();

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

        private async Task JoinExistingGameAsync()
        {
            await ConsoleExtension.ClearAsync();

            string remoteIp = string.Empty;
            short port = 0;

            remoteIp = _userInputHandler.ReadIpAddress("Digite o IP do servidor: ");
            port = _userInputHandler.ReadShort("Digite a porta do servidor: ");

            if (!_applicationSettings.GameTestMode)
            {
                await _sock.ConnectToServerAsync(remoteIp, port);

                _sock.MessageReceived += OnMessageReceived;
                _sock.ConnectionClosed += OnConnectionClosed;
            }

            _logger.Information("Conectado ao jogo. Pronto para jogar.");
            await GameLoop(2);
        }

        private async Task ReconnectToGameAsync()
        {
            var timeout = TimeSpan.FromMinutes(2);
            using var cts = new CancellationTokenSource(timeout);
            int maxRetries = _userSettings.Connection.MaxRetriesAmount;

            Match.SaveToFile();

            //TODO: Validar se esse trecho do código funciona.
            if (Match.Host.Equals(Match.RemoteMachineIp) && await TryReconnectToServerAsync(cts.Token, maxRetries))
                return;

            if (Match.Host.Equals(Match.LocalMachineIp) && await TryStartNewServerAsync(cts.Token, maxRetries))
                return;

            LogFinalFailure(maxRetries);
        }

        private async Task<bool> TryReconnectToServerAsync(CancellationToken token, int maxRetries)
        {
            int attempt = 0;
            int delay = 1000;

            while (attempt < maxRetries && !token.IsCancellationRequested)
            {
                attempt++;
                _logger.Information("Tentativa {0} de reconexão ao servidor...", attempt);

                try
                {
                    if (!_applicationSettings.GameTestMode)
                        await ReinitializeSocketAsync(() => _sock.ConnectToServerAsync(Match.RemoteMachineIp!, Match.SelectedPort));

                    await GameLoop(3);
                    _logger.Information("Reconexão bem-sucedida na tentativa {0}.", attempt);
                    return true;
                }
                catch (Exception ex)
                {
                    LogReconnectFailure(attempt, maxRetries, ex);
                }

                delay = await DelayWithExponentialBackoffAsync(delay, token);
            }

            return false;
        }

        private async Task<bool> TryStartNewServerAsync(CancellationToken token, int maxRetries)
        {
            int attempt = 0;
            int delay = 1000;

            while (attempt < maxRetries && !token.IsCancellationRequested)
            {
                attempt++;
                _logger.Information("Tentativa {0} de iniciar um novo servidor...", attempt);

                try
                {
                    if (!_applicationSettings.GameTestMode)
                        await ReinitializeSocketAsync(() => _sock.StartServerAsync(Match.SelectedPort));

                    await GameLoop(3);
                    _logger.Information("Servidor iniciado com sucesso na tentativa {0}.", attempt);
                    return true;
                }
                catch (Exception ex)
                {
                    LogStartServerFailure(attempt, maxRetries, ex);
                }

                delay = await DelayWithExponentialBackoffAsync(delay, token);
            }

            return false;
        }

        private async Task ReinitializeSocketAsync(Func<Task> connectOrStartAsync)
        {
            _sock.MessageReceived -= OnMessageReceived;
            _sock.ConnectionClosed -= OnConnectionClosed;

            await connectOrStartAsync();

            _sock.MessageReceived += OnMessageReceived;
            _sock.ConnectionClosed += OnConnectionClosed;
        }

        private async static Task<int> DelayWithExponentialBackoffAsync(int delay, CancellationToken token)
        {
            await Task.Delay(Math.Min(delay, 30000), token);
            return delay * 2;
        }

        private void LogReconnectFailure(int attempt, int maxRetries, Exception ex)
        {
            _logger.LogExceptionError($"Falha ao tentar reconectar ao servidor {Match.RemoteMachineIp!} na porta {Match.SelectedPort}. Tentativa {attempt} de {maxRetries}.", ex);
        }

        private void LogStartServerFailure(int attempt, int maxRetries, Exception ex)
        {
            _logger.LogExceptionError($"Falha ao iniciar o servidor na porta {Match.SelectedPort}. Tentativa {attempt} de {maxRetries}.", ex);
        }

        private void LogFinalFailure(int maxRetries)
        {
            _logger.Error("Não foi possível reconectar ou iniciar um novo servidor após {0} tentativas.", maxRetries);
            ConnectionClosed = true;
        }

        private async Task GameLoop(short creationType)
        {
            await ConsoleExtension.ClearAsync();

            if (creationType != 3)
            {
                if (!_applicationSettings.PeerToPeerTestMode)
                {
                    Match = Match.Create(_sock.LocalMachineIP, _sock.RemoteMachineIp, _userInputHandler, _userSettings);
                    await Match.ShipsCreationMethod();
                    Match.DisplayBoard(Match.UserBoard);
                }
            }

            Match.SelectedPort = _sock.SelectedPort;

            if (creationType == 1)
            {
                Match.IpTurn = Match.LocalMachineIp!;
                Match.Host = Match.LocalMachineIp!;
            }

            // Verificar a necessidade dessa verificação e espera.
            if (creationType == 2)
            {
                Match.IpTurn = Match.RemoteMachineIp!;
                Match.Host = Match.RemoteMachineIp!;

                Console.WriteLine("Aguardando até que o oponente envie os navios.");

                while (Match.EnemyBoard == null) if (Match.EnemyBoard != null) break;
            }

            await _sock.SendMessageAsync(await Match.SerializeShipsDto(Match.UserBoard.Ships, CancellationToken.None));

            while (!ConnectionClosed)
            {
                if (!_applicationSettings.GameTestMode && Match.IpTurn.Equals(Match.LocalMachineIp))
                {
                    await ConsoleExtension.ClearAsync();

                    Match.DisplayBoards(Match.UserBoard, Match.EnemyBoard);

                    var attackPosition = Match.AttackEnemyShip();

                    await ConsoleExtension.ClearAsync();

                    Console.WriteLine(Match.EnemyBoard.Attack(attackPosition.X, attackPosition.Y));

                    Match.DisplayBoards(Match.UserBoard, Match.EnemyBoard);

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

                    if (Match.UserBoard.AllShipsSunk)
                    {
                        Match.IsMatchOver = true;

                        Match.MatchWinnerIp = Match.RemoteMachineIp!;

                        Console.WriteLine("Você perdeu, infelizmente não foi dessa vez! 😞");
                    }

                    return;
                }

                try
                {
                    var enemyShipsDto = Ship.DeserializeShips(message);
                    if (enemyShipsDto != null
                        && enemyShipsDto.Any())
                    {
                        strategy = new ReceiveShipsStrategy(_logger, _applicationSettings);
                    }
                }
                catch(Exception)
                {
                    //Não precisa tratar a exceção por que se o código não entrou até aqui é por que ele não é do strategy
                }

                strategyContext = new GameStrategyContext(strategy);
                Match = strategyContext.ExecuteStrategy(message, Match);

                return;
            }
            catch (Exception ex)
            {
                _logger.Error("Uma exceção foi lançada: {0}. Mensagem recebida pelo peer: {1}", ex.Message, message);
            }
        }

        private async void OnConnectionClosed()
        {
            await ReconnectToGameAsync();
        }
    }
}
