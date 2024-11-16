namespace PeerToPeerBattleship.Desktop.Components.Integration.Protocols
{
    public static class Connect
    {
        public static string CreateConnectionProtocol(string ip, int port)
        {
            return $"CONNECT|{ip}|{port}";
        }
    }
}
