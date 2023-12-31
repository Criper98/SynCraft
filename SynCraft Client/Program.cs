using SynCraftClient.Controllers;
using SynCraftClient.Models;
using System;
using System.Windows.Forms;

namespace SynCraftClient
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SettingsController.Read();
            Shared.Logger.LogToFile = Settings.LogToFile;

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}