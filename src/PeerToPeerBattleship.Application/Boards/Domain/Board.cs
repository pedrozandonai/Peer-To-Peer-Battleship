using PeerToPeerBattleship.Application.Ships.Domain;

namespace PeerToPeerBattleship.Application.Boards.Domain
{
    public class Board
    {
        private readonly Ship?[,] grid = new Ship?[10, 10];

        // Lista de navios posicionados
        public List<Ship> Ships { get; } = [];

        public List<(int X, int Y)> ShotsFired { get; } = new List<(int X, int Y)>();

        // Posiciona um navio no tabuleiro
        public void PlaceShip(Ship ship, IEnumerable<(int X, int Y)> positions)
        {
            foreach (var (x, y) in positions)
            {
                if (grid[x, y] != null)
                    throw new InvalidOperationException("As posições não podem se sobrepor.");

                grid[x, y] = ship;
            }

            ship.Place(positions);
            Ships.Add(ship);
        }

        // Realiza um ataque em uma posição
        public string Attack(int x, int y)
        {
            if (x < 0 || x >= 10 || y < 0 || y >= 10)
                throw new ArgumentOutOfRangeException("Coordenadas fora dos limites do tabuleiro.");

            // Registra o tiro
            ShotsFired.Add((x, y));

            var ship = GetCell(x, y);
            if (ship != null)
            {
                if (ship.TakeHit(x, y))
                    return ship.IsSunk ? $"{ship.Name} afundou!" : $"{ship.Name} foi atingido!";
            }

            return "Tiro na água!";
        }

        // Verifica se todos os navios foram destruídos
        public bool AllShipsSunk => Ships.All(ship => ship.IsSunk);

        // Método auxiliar para retornar a célula específica
        public Ship? GetCell(int x, int y) => grid[x, y];
    }
}
