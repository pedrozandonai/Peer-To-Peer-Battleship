using PeerToPeerBattleship.Application.Ships.Model;
using PeerToPeerBattleship.Core.Helpers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PeerToPeerBattleship.Application.Ships.Domain
{
    public class Ship
    {
        public string Name { get; }
        public EShipType ShipType { get; }
        public int Size { get; }

        [JsonIgnore]
        public List<(int X, int Y)> Positions { get; private set; }
        [JsonIgnore]
        public HashSet<(int X, int Y)> Hits { get; private set; }

        // Propriedades para serialização
        public List<List<int>> SerializablePositions => Positions
            .Select(position => new List<int> { position.X, position.Y })
            .ToList();

        public List<List<int>> SerializableHits => Hits
            .Select(hit => new List<int> { hit.X, hit.Y })
            .ToList();

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

        public static List<Ship> DeserializeShipsDto(string json)
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

        // Método para salvar os dados no JSON (com posições e hits serializáveis)
        public string SerializeShip()
        {
            return JsonSerializer.Serialize(new
            {
                Name,
                ShipType,
                Size,
                Positions = SerializablePositions,
                Hits = SerializableHits,
                IsSunk
            });
        }

        // Método para desserializar diretamente para a classe Ship
        public static Ship DeserializeShip(string json)
        {
            var data = JsonSerializer.Deserialize<SerializedShip>(json);
            if (data == null)
                throw new InvalidOperationException("Failed to deserialize JSON.");

            var ship = new Ship(data.Name, data.Size)
            {
                Positions = data.Positions.Select(pos => (pos[0], pos[1])).ToList(),
                Hits = new HashSet<(int X, int Y)>(data.Hits.Select(hit => (hit[0], hit[1])))
            };

            return ship;
        }

        sealed class SerializedShip
        {
            public string Name { get; set; }
            public EShipType ShipType { get; set; }
            public int Size { get; set; }
            public List<List<int>> Positions { get; set; }
            public List<List<int>> Hits { get; set; }
        }
    }
}
