using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using CefSharp;
using ScreenManagerClient.Logic;
using ScreenManagerClient.UIComponents;
using YoutubeExtractor;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace ScreenManagerClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            /*
            WebBrowser.FrameLoadEnd+= WebBrowserOnFrameLoadEnd;
        */
            /*try
            {
                DownloadVideo("https://www.youtube.com/watch?v=b598TqLzBb4","1.mp4");

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }*/
            KeyDown+=  OnKeyDown;
            EngineSetup.Start(MainGrid, FadingGrid, MediaElement);
        }


        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.L)
            {
                if (_isFullScreen)
                {
                    WindowState = WindowState.Normal;
                    WindowStyle = WindowStyle.ThreeDBorderWindow;
                    _isFullScreen = false;
                }
                else
                {    
                    WindowState = WindowState.Maximized;
                    WindowStyle = WindowStyle.None;
                    ResizeMode = ResizeMode.NoResize;
                    ShowInTaskbar = false;
                    _isFullScreen = true;
                }
            }

            if (e.Key == Key.S)
            {
                var dialog = new SettingsWindow();
                dialog.ShowDialog();
                if (dialog.IsOkClicked)
                {
                    EngineSetup.Start(MainGrid, FadingGrid, MediaElement);
                }
            }
        } 
        private bool _isFullScreen = false;
    }
}