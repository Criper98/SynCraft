using SynCraftClient.Models;
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Shared;

namespace SynCraftClient.Controllers
{
    internal class ServerController
    {
        private readonly Server ServerModel = new();

        public void Connect()
        {
            Shared.Logger.Info("Trying to connect to the server: " + Settings.ServerAddress + ":" + Settings.ServerPort);

            ServerModel.Client = new TcpClient(Settings.ServerAddress, Settings.ServerPort);
            ServerModel.DataStream = ServerModel.Client.GetStream();

            try
            {
                JObject json = JObject.FromObject(Assembly.GetEntryAssembly().GetName().Version);

                SendString(json.ToString());
                RecvString(out string data);

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

            ServerModel.IpPort = ((IPEndPoint)ServerModel.Client.Client.RemoteEndPoint).Address.ToString() + 
                ":" + ((IPEndPoint)ServerModel.Client.Client.RemoteEndPoint).Port.ToString();

            Shared.Logger.Info("Connected to server: " + ServerModel.IpPort);
        }

        public void Disconnect()
        {
            ServerModel.DataStream.Close();
            ServerModel.Client.Close();

            Shared.Logger.Info("Disconnected from server");
        }

        public void SendString(string message)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(message);

                ServerModel.DataStream.Write(BitConverter.GetBytes(bytes.Length), 0, 4);

                ServerModel.DataStream.Write(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                Shared.Logger.Error("Error while sending data to the server: " + ex.Message);
            }
        }

        public void RecvString(out string data)
        {
            RecvBytes(out byte[] bytes);

            data = Encoding.UTF8.GetString(bytes);
        }

        public void RecvBytes(out byte[] data)
        {
            data = new byte[4];

            try
            {
                ServerModel.DataStream.Read(data, 0, data.Length);
                int arraySize = BitConverter.ToInt32(data, 0);

                data = new byte[arraySize];

				int recvBytes = ServerModel.DataStream.Read(data, 0, data.Length);

				while (arraySize != recvBytes)
				{
					arraySize -= recvBytes;

					recvBytes = ServerModel.DataStream.Read(data, recvBytes, data.Length - recvBytes);
				}
            }
            catch (Exception ex)
            {
                Shared.Logger.Error("Error while receiving data from the server: " + ex.Message);
            }
        }
    }
}
