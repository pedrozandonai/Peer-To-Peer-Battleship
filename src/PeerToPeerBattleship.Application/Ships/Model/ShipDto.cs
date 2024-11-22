namespace PeerToPeerBattleship.Application.Ships.Model
{
    public class ShipDto
    {
        public string Type { get; set; }
        public List<List<int>> Positions { get; set; }

        public ShipDto(string type, List<List<int>> positions)
        {
            Type = type;
            Positions = positions;
        }
    }
}
