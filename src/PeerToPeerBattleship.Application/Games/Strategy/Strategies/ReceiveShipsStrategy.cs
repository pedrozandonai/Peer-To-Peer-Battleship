using PeerToPeerBattleship.Application.Games.Strategy.Abstractions;
using PeerToPeerBattleship.Application.Matches;
using PeerToPeerBattleship.Application.Ships.Domain;
using PeerToPeerBattleship.Core.Configurations;
using PeerToPeerBattleship.Core.CustomLogger;
using Serilog;

namespace PeerToPeerBattleship.Application.Games.Strategy.Strategies
{
    public class ReceiveShipsStrategy : IGameStrategy
    {
        private readonly ILogger _logger;
        private readonly ApplicationSettings _applicationSettings;

        public ReceiveShipsStrategy(ILogger logger, ApplicationSettings applicationSettings)
        {
            _logger=logger;
            _applicationSettings=applicationSettings;
        }

        public Match ExecuteGameStrategy(string message, Match gameMatch)
        {
            try
            {
                var enemyShipsDto = Ship.DeserializeShips(message);
                gameMatch.EnemyBoard = new Boards.Domain.Board();

                foreach(var ship in enemyShipsDto)
                {
                    gameMatch.EnemyBoard.PlaceShip(ship, ship.Positions);
                }

                if (!_applicationSettings.IsProductionEnvironment)
                {
                    Console.WriteLine("Navios do inimigo posicionados com sucesso.");
                }

                gameMatch.SaveToFile();

                return gameMatch;
            }
            catch (Exception ex)
            {
                _logger.LogExceptionError("Erro ao tentar desserializar o arquivo JSON.", ex);

                gameMatch.SaveToFile();

                return gameMatch;
            }
        }
    }
}
