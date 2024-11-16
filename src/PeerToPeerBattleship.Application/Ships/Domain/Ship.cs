namespace PeerToPeerBattleship.Application.Ships.Domain
{
    public abstract class Ship
    {
        public string Name { get; } = string.Empty;
        public int Size { get; }
        public List<(int X, int Y)> Positions { get; private set; }
        public HashSet<(int X, int Y)> Hits { get; private set; }

        public Ship(string name, int size)
        {
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
    }
}
