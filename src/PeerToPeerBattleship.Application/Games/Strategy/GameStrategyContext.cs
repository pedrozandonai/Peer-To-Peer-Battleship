using PeerToPeerBattleship.Application.Games.Strategy.Abstractions;
using PeerToPeerBattleship.Application.Matches;

namespace PeerToPeerBattleship.Application.Games.Strategy
{
    //Strategy
    public class GameStrategyContext
    {
        private IGameStrategy GameStrategy { get; set; }

        public GameStrategyContext(IGameStrategy gameStrategy)
        {
            GameStrategy = gameStrategy;
        }

        public Match ExecuteStrategy(string message, Match gameMatch) => GameStrategy.ExecuteGameStrategy(message, gameMatch);
    }
}
