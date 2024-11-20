namespace PeerToPeerBattleship.Common.Configurations
{
    public class ApplicationConfigurations
    {
        public bool EnableDesktopApp { get; set; }
        public Ships Ships { get; set; }
    }

    public class Ships
    {
        public Ship AircraftCarrier { get; set; }
        public Ship Battleship { get; set; }
        public Ship Cruizer { get; set; }
        public Ship Destroyer { get; set; }
    }

    public class Ship
    {
        public int Amount { get; set; }
        public int Size { get; set; }
    }
}
