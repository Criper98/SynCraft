using System.Net.Sockets;

namespace SynCraftClient.Models
{
    internal class Server
    {
        public TcpClient? Client;
        public NetworkStream DataStream;
        public string? IpPort;
    }
}
