using PeerToPeerBattleship.Application.Boards.Domain;
using PeerToPeerBattleship.Application.Ships.Domain;
using PeerToPeerBattleship.Core.Inputs.Abstractions;
using System.Collections.Generic;

namespace PeerToPeerBattleship.Application.Matches
{
    public class Match
    {
        public Guid Id { get; set; }
        public string? LocalMachineIp { get; set; }
        public string? RemoteMachineIp { get; set; }
        public List<Ship> UserShips { get; set; }
        public List<Ship> EnemyShips { get; set; }
        public Board UserBoard { get; set; }
        public Board EnemyBoard { get; set; }

        public void CreateMatchShips()
        {
            UserShips =
            [
                new("Porta-aviões", 5),
                new("Encouraçado", 4),
                new("Cruzador 1", 3),
                new("Cruzador 2", 3),
                new("Destróier 1", 2),
                new("Destróier 2", 2)
            ];

            EnemyShips =
            [
                new("Porta-aviões", 5),
                new("Encouraçado", 4),
                new("Cruzador 1", 3),
                new("Cruzador 2", 3),
                new("Destróier 1", 2),
                new("Destróier 2", 2)
            ];
        }

        public void CreateTestMatchBoards()
        {
            UserBoard = new Board();

            var random = new Random();
            foreach (var ship in UserShips)
            {
                bool placed = false;

                while (!placed)
                {
                    // Escolhe uma posição inicial aleatória
                    int startX = random.Next(0, 10);
                    int startY = random.Next(0, 10);

                    // Define a direção: 0 = horizontal, 1 = vertical
                    bool isHorizontal = random.Next(0, 2) == 0;

                    // Calcula as posições que o navio ocupará
                    var positions = Enumerable.Range(0, ship.Size)
                        .Select(i => isHorizontal
                            ? (X: startX, Y: startY + i) // Horizontal
                            : (X: startX + i, Y: startY)) // Vertical
                        .ToList();

                    // Verifica se todas as posições são válidas e livres
                    if (positions.All(pos =>
                        pos.X >= 0 && pos.X < 10 &&
                        pos.Y >= 0 && pos.Y < 10 &&
                        UserBoard.GetCell(pos.X, pos.Y) == null))
                    {
                        // Posiciona o navio
                        UserBoard.PlaceShip(ship, positions);
                        placed = true;
                    }
                }
            }
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

            var indexedShips = new Dictionary<int, Ship>();
            while(count != UserShips.Count)
            {
                DisplayBoard(UserBoard);

                Console.WriteLine("*------------------------------------------------------------------*");
                Console.WriteLine("|     Selecione um dos navios disponíveis para ser posicionado     |");
                foreach (var ship in UserShips)
                {
                    if (ship.Positions.Count > 0) continue;

                    Console.WriteLine(string.Format("{0} - {1}", count, ship.Name));
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

                var axisPosition = PositionAxisSelectedShip(userInputHandler);
                if (axisPosition == 9) continue;
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

        public List<List<(int X, int Y)>> PositionSelectedShip(IUserInputHandler userInputHandler)
        {
            Console.WriteLine("*-----------------------------------------------------------------------------*");
            Console.WriteLine("|   Escreva a primeira e a última coordenada de onde deseja inserir o navio   |");
            Console.WriteLine("|                                                                             |");
            Console.WriteLine("|   Exemplo:                                                                  |");
            Console.WriteLine("*-----------------------------------------------------------------------------*");

            
            var firstPosition = new List<(int X, int Y)>();

            throw new NotImplementedException();
        }
    }
}
