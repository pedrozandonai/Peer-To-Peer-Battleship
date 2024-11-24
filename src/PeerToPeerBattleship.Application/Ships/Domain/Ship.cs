using PeerToPeerBattleship.Application.Ships.Model;
using PeerToPeerBattleship.Core.Helpers;
using System.Text.Json;

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
            => new(
                Name.RemoveAccent().ToLower(),
                Positions.Select(position => new List<int> { position.X, position.Y }).ToList());

        public static List<Ship> DeserializeShips(string json)
        {
            // Desserializa diretamente a lista de ShipDto
            var shipDtos = JsonSerializer.Deserialize<List<ShipDto>>(json);

            if (shipDtos == null || !shipDtos.Any())
                throw new InvalidOperationException("Failed to deserialize JSON or empty ships list.");

            // Converte os DTOs para objetos Ship
            var ships = shipDtos.Select(dto =>
            {
                var ship = new Ship(GetShipName(dto.Tipo), dto.Posicoes.Count);
                ship.Place(dto.Posicoes.Select(pos => (pos[0], pos[1])));
                return ship;
            }).ToList();

            return ships;
        }

        private static string GetShipName(string tipo)
        {
            return tipo switch
            {
                "porta-avioes" => "Porta-aviões",
                "encouracado" => "Encouraçado",
                "cruzador" => "Cruzador",
                "destroier" => "Destróier",
                _ => throw new ArgumentException($"Tipo de navio inválido: {tipo}")
            };
        }
    }
}
