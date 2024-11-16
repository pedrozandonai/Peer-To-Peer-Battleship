using PeerToPeerBattleship.Application.Boards.Domain;

namespace PeerToPeerBattleship.Application.Games.Domain
{
    public static class Game
    {
        //Cria o tabuleiro
        public static void Create()
        {
            var board = new Board();
        }
    }
}
