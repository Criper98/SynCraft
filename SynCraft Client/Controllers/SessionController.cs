using SynCraftClient.Models;
using System;
using System.IO;

namespace SynCraftClient.Controllers
{
	internal class SessionController
	{
		private readonly Session SessionView = new();

		public SessionController()
		{
			SessionView.ModsPath = Settings.MinecraftPath + "\\mods";
			SessionView.ServerManager = new ServerController();
		}

		public void Start()
		{
			try { SessionView.ServerManager.Connect(); }
			catch (Exception ex)
			{ 
				LogsController.Error("Error while connecting to the server: " + ex.Message);
				return;
			}

			if (Settings.SyncMods)
				RecieveMods();

			SessionView.ServerManager.SendString("Disconnect");
			//Thread.Sleep(500);

			SessionView.ServerManager.Disconnect();
		}

		private void RecieveMods()
		{
			SessionView.ServerManager.SendString("GetMods");
			SessionView.ServerManager.RecvString(out string buff);

			if (buff != "OK")
			{
				LogsController.Info("Skipping mods synchronization, disabled by the server");
				return;
			}

			if (Directory.Exists(SessionView.ModsPath))
				Directory.Delete(SessionView.ModsPath, true);

			Directory.CreateDirectory(SessionView.ModsPath);

			while (true)
			{
				SessionView.ServerManager.RecvString(out buff);

				if (buff == "END")
					break;

				LogsController.Info("Downloading mod " + buff);

				SessionView.ServerManager.RecvBytes(out byte[] fileContent);

				File.WriteAllBytes(SessionView.ModsPath + "\\" + buff, fileContent);

				SessionView.ServerManager.SendString("OK");
			}
		}
	}
}
