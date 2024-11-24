namespace PeerToPeerBattleship.Application.Ships.Model
{
    public class ShipDto
    {
        public string Tipo { get; set; }
        public List<List<int>> Posicoes { get; set; }

        public ShipDto(string type, List<List<int>> positions)
        {
            Tipo = type;
            Posicoes = positions;
        }
    }
}
