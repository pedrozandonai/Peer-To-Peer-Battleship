﻿using System.Net.Sockets;

namespace PeerToPeerBattleship.Desktop.Components.Integration
{
    public class TcpConnection
    {
        public void ConnectToBackEnd()
        {
            TcpClient client = new TcpClient("localhost", 6000);
            using var stream = client.GetStream();
            using var writer = new StreamWriter(stream) { AutoFlush = true };
            writer.WriteLine("Hello from Blazor Desktop");
        }
    }
}
