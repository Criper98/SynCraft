using SynCraftClient.Models;
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace SynCraftClient.Controllers
{
    internal class ServerController
    {
        private readonly Server ServerView = new();

        public void Connect()
        {
            LogsController.Info("Trying to connect to the server: " + Settings.ServerAddress + ":" + Settings.ServerPort);

            ServerView.Client = new TcpClient(Settings.ServerAddress, Settings.ServerPort);
            ServerView.DataStream = ServerView.Client.GetStream();

            ServerView.IpPort = ((IPEndPoint)ServerView.Client.Client.RemoteEndPoint).Address.ToString() + 
                ":" + ((IPEndPoint)ServerView.Client.Client.RemoteEndPoint).Port.ToString();

            LogsController.Info("Connected to server: " + ServerView.IpPort);
        }

        public void Disconnect()
        {
            ServerView.DataStream.Close();
            ServerView.Client.Close();

            LogsController.Info("Disconnected from server");
        }

        public void SendString(string message)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(message);

                ServerView.DataStream.Write(BitConverter.GetBytes(bytes.Length), 0, 4);

                ServerView.DataStream.Write(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                LogsController.Error("Error while sending data to the server: " + ex.Message);
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
				ServerView.DataStream.Read(data, 0, data.Length);
                int arraySize = BitConverter.ToInt32(data, 0);

                data = new byte[arraySize];

				int recvBytes = ServerView.DataStream.Read(data, 0, data.Length);

				while (arraySize != recvBytes)
				{
					arraySize -= recvBytes;

					recvBytes = ServerView.DataStream.Read(data, recvBytes, data.Length - recvBytes);
				}
            }
            catch (Exception ex)
            {
                LogsController.Error("Error while receiving data from the server: " + ex.Message);
            }
        }
    }
}
