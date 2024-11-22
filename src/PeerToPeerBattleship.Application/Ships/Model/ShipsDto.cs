namespace PeerToPeerBattleship.Application.Ships.Model
{
    public class ShipsDto
    {
        public List<ShipDto> Ships { get; set; }

        public ShipsDto(List<ShipDto> ships)
        {
            Ships = ships;
        }
    }
}
