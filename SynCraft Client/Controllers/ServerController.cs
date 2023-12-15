using Microsoft.VisualBasic.Logging;
using SynCraftClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SynCraftClient.Controllers
{
    internal class ServerController
    {
        private Server ServerView = new();

        public void Connect()
        {
            LogsController.Info("Trying to connect to the server: " + Settings.ServerAddress + ":" + Settings.ServerPort);

            try
            {
                ServerView.Client = new TcpClient(Settings.ServerAddress, Settings.ServerPort);
                ServerView.DataStream = ServerView.Client.GetStream();

                ServerView.IpPort = ((IPEndPoint)ServerView.Client.Client.RemoteEndPoint).Address.ToString() + 
                    ":" + ((IPEndPoint)ServerView.Client.Client.RemoteEndPoint).Port.ToString();

                LogsController.Info("Connected to server: " + ServerView.IpPort);
            }
            catch (Exception ex)
            {
                LogsController.Error("Error while connecting to the server: " + ex.Message);
            }
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
            data = "";

            try
            {
                byte[] bytes = new byte[4];

                int byteSize = ServerView.DataStream.Read(bytes, 0, bytes.Length);
                int stringSize = BitConverter.ToInt32(bytes, 0);

                bytes = new byte[stringSize];
                byteSize = ServerView.DataStream.Read(bytes, 0, bytes.Length);

                data = Encoding.UTF8.GetString(bytes, 0, byteSize);
            }
            catch (Exception ex)
            {
                LogsController.Error("Error while receiving data from the server: " + ex.Message);
            }
        }

        public void RecvBytes(out byte[] data)
        {
            data = new byte[4];

            try
            {
                ServerView.DataStream.Read(data, 0, data.Length);
                int dataSize = BitConverter.ToInt32(data, 0);

                data = new byte[dataSize];
                ServerView.DataStream.Read(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                LogsController.Error("Error while receiving data from the server: " + ex.Message);
            }
        }
    }
}
