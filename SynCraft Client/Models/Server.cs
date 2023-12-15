using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SynCraftClient.Models
{
    internal class Server
    {
        public TcpClient? Client;
        public NetworkStream? DataStream;
        public string? IpPort;
    }
}
