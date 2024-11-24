using PeerToPeerBattleship.Application.Boards.Domain;
using PeerToPeerBattleship.Application.Ships.Domain;
using PeerToPeerBattleship.Application.Ships.Model;
using PeerToPeerBattleship.Core.Inputs.Abstractions;
using System.Text.Json;

namespace PeerToPeerBattleship.Application.Matches
{
    public class Match
    {
        public Guid Id { get; set; }
        public string IpTurn { get; set; } = string.Empty;
        public string? LocalMachineIp { get; set; }
        public string? RemoteMachineIp { get; set; }
        public Board UserBoard { get; set; } = new();
        public Board EnemyBoard { get; set; } = new();
        public IUserInputHandler? UserInputHandler { get; set; }
        public bool IsMatchOver { get; set; }
        public DateTime CreationDateTime { get; set; }
        public string MatchWinnerIp { get; set; }
        public short SelectedPort { get; set; }
        private List<Ship> AvailableShips { get; set; } = [];

        private Match(string? localMachineIp, string? remoteMachineIp, IUserInputHandler userInputHandler, bool isMatchOver)
        {
            Id = Guid.CreateVersion7();
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
            UserInputHandler = userInputHandler;
            IsMatchOver = isMatchOver;
            CreationDateTime = DateTime.Now;
        }

        public static Match Create(string localMachineIp, string remoteMachineIp, IUserInputHandler userInputHandler)
            => new(localMachineIp, remoteMachineIp, userInputHandler, false);

        public Board GenerateRandomPositions()
        {
            UserBoard = new Board();

            var random = new Random();
            foreach (var ship in AvailableShips)
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

            return UserBoard;
        }

        public static void DisplayBoard(Board board)
        {
            int cellWidth = 3; // Largura de cada célula

            // Linha superior (delimitador horizontal)
            Console.WriteLine("   " + new string('-', 10 * cellWidth + 1));

            // Linhas do tabuleiro
            for (int y = 9; y >= 0; y--) // De cima (9) para baixo (0)
            {
                Console.Write($"{y,2} "); // Índice da linha (eixo Y)
                for (int x = 0; x < 10; x++)
                {
                    var cell = board.GetCell(x, y);
                    string displayChar = cell switch
                    {
                        null => " ", // Célula vazia
                        Ship ship when ship.Hits.Contains((x, y)) => "X", // Navio atingido
                        Ship => "O", // Navio não atingido
                    };

                    Console.Write($"|{displayChar.PadLeft(cellWidth / 2).PadRight(cellWidth - 1)}");
                }
                Console.WriteLine("|");
            }

            // Linha inferior (delimitador horizontal)
            Console.WriteLine("   " + new string('-', 10 * cellWidth + 1));

            // Cabeçalho das colunas (eixo X)
            Console.Write("   ");
            for (int x = 0; x < 10; x++)
            {
                Console.Write(x.ToString().PadLeft(cellWidth / 2 + 1).PadRight(cellWidth));
            }
            Console.WriteLine();
        }

        public Board ShipsCreationMethod()
        {
            string header = "*------------------------------------------------------------------*";

            Console.WriteLine(header);
            Console.WriteLine("|               Como você deseja criar os navios?                  |");
            Console.WriteLine("|                   1 - Gerar aleatoriamente                       |");
            Console.WriteLine("|                   2 - Escolher as posições                       |");
            Console.WriteLine(header);
            var creationMethod = UserInputHandler!.ReadShort("Digite a sua opção: ");


            switch(creationMethod)
            {
                case 1:
                    return GenerateRandomPositions();

                case 2:
                    return CreateUserBoard();

                default:
                    Console.WriteLine("Opção não reconhecida pelo programa...");
                    return ShipsCreationMethod();
            }
        }

        public Board CreateUserBoard()
        {
            UserBoard = new Board();

            int count = 1;
            int selectedShipsCount = AvailableShips.Count;
            var indexedShips = new Dictionary<int, Ship>();

            while(AvailableShips.Count != UserBoard.Ships.Count)
            {
                DisplayBoard(UserBoard);

                string header = "*------------------------------------------------------------------*";
                Console.WriteLine(header);
                Console.WriteLine("|     Selecione um dos navios disponíveis para ser posicionado     |");

                indexedShips.Clear(); // Limpa o dicionário a cada iteração.

                foreach (var ship in AvailableShips)
                {
                    if (ship.Positions.Count > 0) continue;

                    // Formata a linha para garantir alinhamento.
                    string shipSelection = $"{count} - {ship.Name}. (Tamanho do navio: {ship.Size})";
                    string formattedLine = $"| {shipSelection.PadRight(header.Length - 3)}|"; // Ajusta para caber dentro do tamanho da linha.

                    Console.WriteLine(formattedLine);
                    indexedShips.Add(count, ship);
                    count++;
                }

                Console.WriteLine(header);

                var selectedShip = UserInputHandler!.ReadShort("    Selecione dentre as opções disponíveis: ");
                if (!indexedShips.ContainsKey(selectedShip))
                {
                    Console.WriteLine("Valor inserido não corresponde a nenhuma das opções disponíveis, por favor, selecione uma opção válida.");
                    continue;
                }

                (int X, int Y) shipSelectedPosition;
                short axisPosition;
                List<(int X, int Y)> shipFinalPositions;

                while (true)
                {
                    shipSelectedPosition = PositionSelectedShip();
                    axisPosition = PositionAxisSelectedShip();
                    shipFinalPositions = GeneratePositions(shipSelectedPosition, axisPosition, indexedShips[selectedShip].Size);

                    if (ShipPositionIsValid(shipFinalPositions, axisPosition, indexedShips[selectedShip].Size)) break;
                    
                    Console.WriteLine("Posição inválida, por favor, digite a posição novamente.");
                }

                UserBoard.PlaceShip(indexedShips[selectedShip], shipFinalPositions);
                Console.WriteLine();
                Console.WriteLine(string.Format("Navio {0} adicionado com sucesso ao tabuleiro!", indexedShips[selectedShip].Name));
                Console.WriteLine();
                selectedShipsCount--;
                count = 1;
            }

            return UserBoard;
        }

        public short PositionAxisSelectedShip()
        {
            Console.WriteLine("*------------------------------------------------------------------*");
            Console.WriteLine("|      Deseja posicionar o navio na vertical ou na horizontal?     |");
            Console.WriteLine("|                      1 - Vertical                                |");
            Console.WriteLine("|                      2 - Horizontal                              |");
            Console.WriteLine("*------------------------------------------------------------------*");
            var selectedAxis = UserInputHandler!.ReadShort("    Selecione dentre as opções disponíveis: ");

            if (selectedAxis != 1 &&
                selectedAxis != 2)
            {
                Console.WriteLine("Orientação inválida, por favor, selecione uma das opções ou aperte 9 para sair do menu.");
                return PositionAxisSelectedShip();
            }

            return selectedAxis;
        }

        public (int X, int Y) PositionSelectedShip()
        {
            Console.WriteLine("*------------------------------------------------------------------*");
            Console.WriteLine("|   Escreva a primeira coordenada de onde deseja inserir o navio   |");
            Console.WriteLine("|                                                                  |");
            Console.WriteLine("|   Exemplo:                                                       |");
            Console.WriteLine("|       Digite a coordenada inicial do seu navio: 5,5              |");
            Console.WriteLine("*------------------------------------------------------------------*");

            var startPosition = UserInputHandler!.ReadPositions("    Digite a coordenada inicial do seu navio (formato X,Y): ");
            
            return startPosition;
        }

        private static List<(int X, int Y)> GeneratePositions((int X, int Y) start, short axisPosition, int shipSize)
        {
            var positions = new List<(int X, int Y)>();

            // Validação do tamanho do navio.
            if (shipSize <= 0)
                throw new ArgumentException("O tamanho do navio deve ser maior que zero.", nameof(shipSize));

            // Definição do deslocamento com base no eixo.
            if (axisPosition == 1)
            {
                // Colocação horizontal (eixo Y varia).
                for (int i = 0; i < shipSize; i++)
                {
                    positions.Add((start.X, start.Y + i));
                }
            }
            else if (axisPosition == 2)
            {
                // Colocação vertical (eixo X varia).
                for (int i = 0; i < shipSize; i++)
                {
                    positions.Add((start.X + i, start.Y));
                }
            }
            else
            {
                throw new ArgumentException("O eixo deve ser 1 (horizontal) ou 2 (vertical).", nameof(axisPosition));
            }

            return positions;
        }

        private static bool ShipPositionIsValid(List<(int X, int Y)> shipPositions, short axisPosition, int shipLength)
        {
            // Validação básica do tamanho do navio.
            if (shipPositions.Count != shipLength)
                return false; // O tamanho do navio não corresponde ao número de posições fornecidas.

            // Verifica se o eixo informado é válido.
            if (axisPosition != 1 && axisPosition != 2)
                throw new ArgumentException("O eixo deve ser 1 (horizontal) ou 2 (vertical).", nameof(axisPosition));

            // Determinação inicial de alinhamento.
            bool isHorizontal = axisPosition == 1;
            bool isVertical = axisPosition == 2;

            // Valores iniciais para verificação.
            int fixedCoordinate = isHorizontal ? shipPositions[0].X : shipPositions[0].Y;

            // Verifica consistência do alinhamento e continuidade.
            var variableCoordinates = isHorizontal
                ? shipPositions.Select(pos => pos.Y).OrderBy(c => c).ToArray()
                : shipPositions.Select(pos => pos.X).OrderBy(c => c).ToArray();

            foreach (var position in shipPositions)
            {
                if ((isHorizontal && position.X != fixedCoordinate) ||
                    (isVertical && position.Y != fixedCoordinate))
                {
                    return false; // Não está alinhado ao eixo indicado.
                }
            }

            // Valida continuidade das posições.
            for (int i = 1; i < variableCoordinates.Length; i++)
            {
                if (variableCoordinates[i] != variableCoordinates[i - 1] + 1)
                    return false; // As posições não estão em sequência.
            }

            return true; // Todas as validações passaram.
        }

        public (int X, int Y) AttackEnemyShip()
        {
            Console.WriteLine("*------------------------------------------------------------------*");
            Console.WriteLine("|       Digite a posição que deseja atacar na grade do inimigo     |");
            Console.WriteLine("*------------------------------------------------------------------*");
            var attackPosition = UserInputHandler!.ReadPositions("    Digite a coordenada (formato X,Y): ");

            return attackPosition;
        }

        public async Task<string> SerializeShipsDto(List<Ship> ships, CancellationToken cancellationToken)
        {
            // Converte a lista de objetos Ship para ShipDto
            List<ShipDto> shipsDtos = ships.Select(ship => ship.CreateShipDto()).ToList();

            // Serializa diretamente a lista de ShipDto
            using var memoryStream = new MemoryStream();
            await JsonSerializer.SerializeAsync(memoryStream, shipsDtos, cancellationToken: cancellationToken);

            memoryStream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(memoryStream);
            return await reader.ReadToEndAsync();
        }

        public void SaveToFile()
        {
            if (string.IsNullOrEmpty(RemoteMachineIp))
            {
                throw new InvalidOperationException("O endereço remoto não pode estár em branco ou ser nulo durante a criação do arquivo da partida.");
            }

            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string folderPath = Path.Combine(documentsPath, "PeerToPeerBattleShip");

            Directory.CreateDirectory(folderPath);

            string fileName = $"{CreationDateTime:yyyy-MM-dd_HH-mm-ss}_{RemoteMachineIp}.txt";
            string filePath = Path.Combine(folderPath, fileName);

            string jsonContent = JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            if (File.Exists(filePath)) File.Delete(filePath);

            File.WriteAllText(filePath, jsonContent);
            Console.WriteLine($"Arquivo da partida salvo em: {filePath}.");
        }

        public static Match LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("O arquivo especificado não existe.", filePath);
            }

            string jsonContent = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<Match>(jsonContent) ?? throw new InvalidOperationException("Erro ao tentar desesserializar o arquivo indicado.");
        }

        public static Match? FindAndLoadUnfinishedMatch(string directoryPath)
        {
            string[] files;
            try
            {
                files = Directory.GetFiles(directoryPath, "*.txt");
            }
            catch (Exception ex)
            {
                return null;
            }

            foreach (string file in files)
            {
                try
                {
                    string jsonContent = File.ReadAllText(file);
                    Match? match = JsonSerializer.Deserialize<Match>(jsonContent);

                    if (match != null && !match.IsMatchOver)
                    {
                        Console.WriteLine($"Partida não finalizada encontrada no arquivo: {file}");
                        return match;
                    }
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Erro ao deserializar o arquivo {file}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro inesperado ao processar o arquivo {file}: {ex.Message}");
                }
            }

            Console.WriteLine("Nenhuma partida não finalizada foi encontrada.");
            return null;
        }
    }
}
