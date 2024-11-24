using PeerToPeerBattleship.Application.Games.Strategy.Abstractions;
using PeerToPeerBattleship.Application.Matches;
using Serilog;

namespace PeerToPeerBattleship.Application.Games.Strategy.Strategies
{
    public class DataNotRecognizedStrategy : IGameStrategy
    {
        private readonly ILogger _logger;

        public DataNotRecognizedStrategy(ILogger logger)
        {
            _logger=logger;
        }

        public Match ExecuteGameStrategy(string message, Match gameMatch)
        {
            _logger.Error("Mensagem recebida pelo peer não reconhecida pelo programa. Mensagem: {0}", message);

            return gameMatch;
        }
    }
}
