using PeerToPeerBattleship.Application.Games.Strategy.Abstractions;
using PeerToPeerBattleship.Application.Matches;
using PeerToPeerBattleship.Application.Ships.Domain;
using PeerToPeerBattleship.Core.CustomLogger;
using Serilog;

namespace PeerToPeerBattleship.Application.Games.Strategy.Strategies
{
    public class ReceiveShipsStrategy : IGameStrategy
    {
        private readonly ILogger _logger;

        public ReceiveShipsStrategy(ILogger logger)
        {
            _logger=logger;
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

                Console.WriteLine("Navios do inimigo posicionados com sucesso.");

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
