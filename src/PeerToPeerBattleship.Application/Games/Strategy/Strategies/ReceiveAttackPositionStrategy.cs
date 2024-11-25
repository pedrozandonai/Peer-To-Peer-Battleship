using PeerToPeerBattleship.Application.Games.Strategy.Abstractions;
using PeerToPeerBattleship.Application.Matches;
using Serilog;

namespace PeerToPeerBattleship.Application.Games.Strategy.Strategies
{
    public class ReceiveAttackPositionStrategy : IGameStrategy
    {
        private readonly ILogger _logger;

        public ReceiveAttackPositionStrategy(ILogger logger)
        {
            _logger=logger;
        }

        public Match ExecuteGameStrategy(string message, Match gameMatch)
        {
            if (string.IsNullOrWhiteSpace(message) || message.Length != 2)
            {
                _logger.Error("A mensagem deve conter 2 dígitos indicando as posições onde o navio deve ser atacado.");

                throw new ArgumentException("A mensagem deve conter 2 dígitos indicando as posições onde o navio deve ser atacado.");
            }

            int x = int.Parse(message[0].ToString());
            int y = int.Parse(message[1].ToString());

            Console.WriteLine(gameMatch.UserBoard.Attack(x, y));

            gameMatch.IpTurn = gameMatch.LocalMachineIp!;

            gameMatch.SaveToFile();

            return gameMatch;
        }
    }
}
