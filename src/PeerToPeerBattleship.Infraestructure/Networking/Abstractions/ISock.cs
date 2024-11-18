namespace PeerToPeerBattleship.Infraestructure.Networking.Abstractions
{
    public interface ISock
    {
        public string LocalMachineIP { get; set; }
        public string RemoteMachineIp { get; set; }
        event Action<string> MessageReceived;
        event Action ConnectionClosed;

        Task StartServerAsync(short port);
        Task ConnectToServerAsync(string serverIp, short port);
        Task SendMessageAsync(string message);
        void CloseConnection();
    }
}
