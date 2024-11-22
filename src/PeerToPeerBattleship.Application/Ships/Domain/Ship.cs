using PeerToPeerBattleship.Application.Ships.Model;
using PeerToPeerBattleship.Core.Helpers;

namespace PeerToPeerBattleship.Application.Ships.Domain
{
    public class Ship
    {
        public string Name { get; }
        public EShipType ShipType { get; }
        public int Size { get; }
        public List<(int X, int Y)> Positions { get; private set; }
        public HashSet<(int X, int Y)> Hits { get; private set; }

        public Ship(string name, int size)
        {
            ShipType = GetShipTypeByName(name);
            Name = name;
            Size = size;
            Positions = [];
            Hits = [];
        }

        // Adiciona posições ao navio (durante o posicionamento no tabuleiro)
        public void Place(IEnumerable<(int X, int Y)> positions)
        {
            if (positions.Count() != Size)
                throw new InvalidOperationException($"O navio {Name} deve ocupar exatamente {Size} células.");

            Positions = positions.ToList();
        }

        // Registra um ataque em uma posição
        public bool TakeHit(int x, int y)
        {
            if (Positions.Contains((x, y)))
            {
                Hits.Add((x, y));
                return true; // Foi atingido
            }

            return false; // Ataque falhou
        }

        // Verifica se o navio foi destruído
        public bool IsSunk => Hits.Count == Size;

        public static EShipType GetShipTypeByName(string shipName)
        {
            return shipName switch
            {
                "Porta-aviões" => EShipType.AircraftCarrier,
                "Encouraçado" => EShipType.Battleship,
                "Cruzador" => EShipType.Cruiser,
                "Destróier" => EShipType.Destroyer,
                _ => throw new ArgumentException($"Nome de barco inválido: {shipName}")
            };
        }
        public ShipDto CreateShipDto()
            => new ShipDto(
                Name.RemoveAccent().ToUpper(),
                Positions.Select(position => new List<int> { position.X, position.Y }).ToList()
            );
    }
}
