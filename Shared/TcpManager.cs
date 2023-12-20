using System;
using System.Net.Sockets;
using System.Net;
using System.Reflection;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Shared
{
    public class TcpManager
    {
        public string IpPort = "";
        public TcpClient Client = new();
        public NetworkStream DataStream;

        public void ConnectToServer(string serverAddress, int serverPort)
        {
            Logger.Info("Trying to connect to the server: " + serverAddress + ":" + serverPort);

            Client = new TcpClient(serverAddress, serverPort);
            DataStream = Client.GetStream();

            try
            {
                JObject json = JObject.FromObject(Assembly.GetEntryAssembly().GetName().Version);

                SendString(json.ToString());
                string data = RecvString();

                json = JObject.Parse(data);

                if ((bool)json["IsCompatible"])
                {
                    Logger.Error("This version of SynCraft it's not compatible with the server. " +
                        "Please download SynCraft " + json["CompatibleVersion"] + " to fix this error.");

                    //TODO: Implement auto-update/downgrade

                    Disconnect();
                }
            }
            catch { Disconnect(); }

            IpPort = ((IPEndPoint)Client.Client.RemoteEndPoint).Address.ToString() +
                ":" + ((IPEndPoint)Client.Client.RemoteEndPoint).Port.ToString();

            Logger.Info("Connected to server: " + IpPort);
        }

        public void Disconnect()
        {
            if (!Client.Connected)
                return;

            DataStream.Close();
            Client.Close();

            Logger.Info("Disconnected from the server.");
        }

        public void SendString(string message)
        {
            SendBytes(Encoding.UTF8.GetBytes(message));
        }

        public void SendBytes(byte[] data)
        {
            try
            {
                DataStream.Write(BitConverter.GetBytes(data.Length), 0, 4);

                DataStream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Logger.Error("Error while sending data to the server: " + ex.Message);
            }
        }

        public string RecvString()
        {
            return Encoding.UTF8.GetString(RecvBytes());
        }

        public byte[] RecvBytes()
        {
            byte[] data = new byte[4];

            try
            {
                DataStream.Read(data, 0, data.Length);
                int arraySize = BitConverter.ToInt32(data, 0);

                data = new byte[arraySize];

                int recvBytes = DataStream.Read(data, 0, data.Length);

                while (arraySize != recvBytes)
                {
                    arraySize -= recvBytes;

                    recvBytes = DataStream.Read(data, recvBytes, data.Length - recvBytes);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error while receiving data from the server: " + ex.Message);
            }

            return data;
        }
    }
}
