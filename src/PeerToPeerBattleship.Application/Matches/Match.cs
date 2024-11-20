using PeerToPeerBattleship.Application.Boards.Domain;
using PeerToPeerBattleship.Application.Ships.Domain;
using PeerToPeerBattleship.Core.Inputs.Abstractions;

namespace PeerToPeerBattleship.Application.Matches
{
    public class Match
    {
        public Guid Id { get; set; }
        public string? LocalMachineIp { get; set; }
        public string? RemoteMachineIp { get; set; }
        public Board UserBoard { get; set; }
        public Board EnemyBoard { get; set; }
        public List<Ship> AvailableShips { get; set; }

        public Match(string localMachineIp, string remoteMachineIp)
        {
            Id = new Guid();
            LocalMachineIp = localMachineIp;
            RemoteMachineIp = remoteMachineIp;
            AvailableShips =
            [
                new("Porta-aviões", 5),
                new("Encouraçado", 4),
                new("Cruzador", 3),
                new("Cruzador", 3),
                new("Destróier", 2),
                new("Destróier", 2)
            ];
        }

        public void CreateMatchShips()
        {
            //UserShips =
            //[
            //    new("Porta-aviões", 5),
            //    new("Encouraçado", 4),
            //    new("Cruzador", 3),
            //    new("Cruzador", 3),
            //    new("Destróier", 2),
            //    new("Destróier", 2)
            //];
        }

        public void CreateTestMatchBoards()
        {
            //UserBoard = new Board();

            //var random = new Random();
            //foreach (var ship in UserShips)
            //{
            //    bool placed = false;

            //    while (!placed)
            //    {
            //        // Escolhe uma posição inicial aleatória
            //        int startX = random.Next(0, 10);
            //        int startY = random.Next(0, 10);

            //        // Define a direção: 0 = horizontal, 1 = vertical
            //        bool isHorizontal = random.Next(0, 2) == 0;

            //        // Calcula as posições que o navio ocupará
            //        var positions = Enumerable.Range(0, ship.Size)
            //            .Select(i => isHorizontal
            //                ? (X: startX, Y: startY + i) // Horizontal
            //                : (X: startX + i, Y: startY)) // Vertical
            //            .ToList();

            //        // Verifica se todas as posições são válidas e livres
            //        if (positions.All(pos =>
            //            pos.X >= 0 && pos.X < 10 &&
            //            pos.Y >= 0 && pos.Y < 10 &&
            //            UserBoard.GetCell(pos.X, pos.Y) == null))
            //        {
            //            // Posiciona o navio
            //            UserBoard.PlaceShip(ship, positions);
            //            placed = true;
            //        }
            //    }
            //}
        }

        public static void DisplayBoard(Board board)
        {
            Console.WriteLine("   " + string.Join(" ", Enumerable.Range(0, 10))); // Cabeçalho das colunas

            for (int x = 0; x < 10; x++)
            {
                Console.Write($"{x,2} "); // Índice da linha
                for (int y = 0; y < 10; y++)
                {
                    var cell = board.GetCell(x, y);
                    string displayChar = cell switch
                    {
                        null => " ", // Vazio
                        Ship ship when ship.Hits.Contains((x, y)) => "X", // Navio atingido
                        Ship => "O", // Navio não atingido
                    };

                    Console.Write($"|{displayChar}");
                }
                Console.WriteLine("|");
            }
            Console.WriteLine("   " + new string('-', 21)); // Linha inferior
        }

        public void CreateUserBoard(IUserInputHandler userInputHandler)
        {
            UserBoard = new Board();

            int count = 1;
            int selectedShipsCount = AvailableShips.Count;
            var indexedShips = new Dictionary<int, Ship>();

            while(AvailableShips.Count != UserBoard.Ships.Count)
            {
                DisplayBoard(UserBoard);

                Console.WriteLine("*------------------------------------------------------------------*");
                Console.WriteLine("|     Selecione um dos navios disponíveis para ser posicionado     |");
                foreach (var ship in AvailableShips)
                {
                    if (ship.Positions.Count > 0) continue;

                    //TODO: Arrumar a saída aqui:
                    Console.WriteLine(string.Format("{0} - {1}. (Tamanho do navio: {2})", count, ship.Name, ship.Size));
                    indexedShips.Add(count, ship);

                    count++;
                }
                Console.WriteLine("*------------------------------------------------------------------*");
                var selectedShip = userInputHandler.ReadShort("Selecione dentre as opções disponíveis: ");

                if (!indexedShips.ContainsKey(selectedShip))
                {
                    Console.WriteLine("Valor inserido não corresponde a nenhuma das opções disponíveis, por favor, selecione uma opção válida.");
                    continue;
                }

                //Acho que não vai precisar dessa parte
                //var axisPosition = PositionAxisSelectedShip(userInputHandler);
                //if (axisPosition == 9) continue;

                var shipSelectedPosition = new List<(int X, int Y)>();

                while (true)
                {
                    shipSelectedPosition = PositionSelectedShip(userInputHandler);

                    if (ShipPositionIsValid(shipSelectedPosition, indexedShips[selectedShip].Size)) break;
                    
                    Console.WriteLine("Posição inválida, por favor, digite a posição novamente.");
                }

                UserBoard.PlaceShip(indexedShips[selectedShip], shipSelectedPosition);
                Console.WriteLine(string.Format("Navio {0} adicionado com sucesso ao tabuleiro!", indexedShips[selectedShip].Name));
                selectedShipsCount--;
                count = 1;
                indexedShips = [];
            }
        }

        public short PositionAxisSelectedShip(IUserInputHandler userInputHandler)
        {
            Console.WriteLine("*------------------------------------------------------------------*");
            Console.WriteLine("|      Deseja posicionar o navio na vertical ou na horizontal?     |");
            Console.WriteLine("|                      1 - Vertical                                |");
            Console.WriteLine("|                      2 - Horizontal                              |");
            Console.WriteLine("|                      9 - Sair                                    |");
            Console.WriteLine("*------------------------------------------------------------------*");
            var selectedAxis = userInputHandler.ReadShort("Selecione dentre as opções disponíveis: ");

            if (selectedAxis != 1 ||
                selectedAxis != 2 ||
                selectedAxis != 9)
            {
                Console.WriteLine("Orientação inválida, por favor, selecione uma das opções ou aperte 9 para sair do menu.");
                return PositionAxisSelectedShip(userInputHandler);
            }

            return selectedAxis;
        }

        public List<(int X, int Y)> PositionSelectedShip(IUserInputHandler userInputHandler)
        {
            Console.WriteLine("*-----------------------------------------------------------------------------*");
            Console.WriteLine("|   Escreva a primeira e a última coordenada de onde deseja inserir o navio   |");
            Console.WriteLine("|                                                                             |");
            Console.WriteLine("|   Exemplo:                                                                  |");
            Console.WriteLine("|       1 - Digite a coordenada inicial do seu navio: 5,5                     |");
            Console.WriteLine("|       2 - Digite a coordenada final do seu navio: 5,9                       |");
            Console.WriteLine("*-----------------------------------------------------------------------------*");

            var shipPositions = new List<(int X, int Y)>();

            try
            {
                var startPosition = userInputHandler.ReadPositions("Digite a coordenada inicial do seu navio (formato X,Y): ");
                var endPosition = userInputHandler.ReadPositions("Digite a coordenada final do seu navio (formato X,Y): ");
                var positions = GeneratePositions(startPosition, endPosition);

                shipPositions.AddRange(positions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}. Por favor, tente novamente.");
                return PositionSelectedShip(userInputHandler);
            }

            return shipPositions;
        }

        private List<(int X, int Y)> GeneratePositions((int X, int Y) start, (int X, int Y) end)
        {
            var positions = new List<(int X, int Y)>();

            if (start.X != end.X && start.Y != end.Y)
                throw new InvalidOperationException("Os navios devem ser posicionados horizontalmente ou verticalmente.");

            if (start.X == end.X)
            {
                // Horizontal ship placement.
                for (int y = Math.Min(start.Y, end.Y); y <= Math.Max(start.Y, end.Y); y++)
                {
                    positions.Add((start.X, y));
                }
            }
            else
            {
                // Vertical ship placement.
                for (int x = Math.Min(start.X, end.X); x <= Math.Max(start.X, end.X); x++)
                {
                    positions.Add((x, start.Y));
                }
            }

            return positions;
        }

        private bool ShipPositionIsValid(List<(int X, int Y)> shipPositions, int shipLength)
        {
            if (shipPositions.Count != shipLength)
                return false; // O tamanho do navio não corresponde ao número de posições fornecidas.

            // Verifica se as posições formam uma linha horizontal ou vertical.
            bool isHorizontal = true;
            bool isVertical = true;

            // Valores iniciais para comparação.
            int xFirstPosition = shipPositions[0].X;
            int yFirstPosition = shipPositions[0].Y;

            // Verifica consistência das posições.
            foreach (var shipPosition in shipPositions)
            {
                if (shipPosition.Y != yFirstPosition)
                    isHorizontal = false;
                if (shipPosition.X != xFirstPosition)
                    isVertical = false;

                // Se não for nem horizontal nem vertical, já é inválido.
                if (!isHorizontal && !isVertical)
                    return false;
            }

            if (isHorizontal)
            {
                // Ordena as posições para verificar continuidade.
                var xPositions = shipPositions.Select(pos => pos.X).OrderBy(x => x).ToArray();
                for (int i = 1; i < xPositions.Length; i++)
                {
                    if (xPositions[i] != xPositions[i - 1] + 1)
                        return false; // Não está em sequência.
                }
            }
            else if (isVertical)
            {
                // Ordena as posições para verificar continuidade.
                var yPositions = shipPositions.Select(pos => pos.Y).OrderBy(y => y).ToArray();
                for (int i = 1; i < yPositions.Length; i++)
                {
                    if (yPositions[i] != yPositions[i - 1] + 1)
                        return false; // Não está em sequência.
                }
            }
            else
            {
                return false; // Não é horizontal nem vertical.
            }

            return true; // Todas as validações passaram.
        }
    }
}
