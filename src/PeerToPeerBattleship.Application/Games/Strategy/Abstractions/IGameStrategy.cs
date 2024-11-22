using PeerToPeerBattleship.Application.Matches;

namespace PeerToPeerBattleship.Application.Games.Strategy.Abstractions
{
    public interface IGameStrategy
    {
        Match ExecuteGameStrategy(string message, Match gameMatch);
    }
}
