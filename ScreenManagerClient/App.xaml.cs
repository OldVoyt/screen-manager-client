using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NLog;
using Squirrel;
using Logger = ScreenManagerClient.Logic.Logger;

namespace ScreenManagerClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
       /*     using (var mgr = new UpdateManager("C:\\learning\\screen-manager\\ScreenManagerClient\\ScreenManagerClient\\Releases"))
            {
                await mgr.UpdateApp();
            }*/
            
            NLog.LogManager.Setup().LoadConfiguration(builder =>
            {
                builder.ForLogger().WriteToFile(fileName: "Logs.txt", archiveAboveSize: 1000000, maxArchiveFiles: 20);
            });
            base.OnStartup(e);
            Current.DispatcherUnhandledException += (sender, args) =>
            {
                string errorMessage = string.Format("An unhandled exception occurred: {0}", args.Exception.Message);
                Logger.GetLogger().LogError("Unhandled exception", args.Exception);
            };
        }
        
    }
}