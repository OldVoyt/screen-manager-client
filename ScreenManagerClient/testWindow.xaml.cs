using System;
using System.Windows;
using System.Windows.Controls;

namespace ScreenManagerClient;

public partial class testWindow : Window
{
    public testWindow()
    {
        InitializeComponent();
        MediaElement.Source =
            new Uri(
                @"C:\learning\screen-manager\ScreenManagerClient\ScreenManagerClient\bin\Debug\net48\FilesCache\httpsdrive.google.comucid=1LR8xZEGGUYStgJNrRPQZdujYtG3Wz-Km.mp4");
        MediaElement.LoadedBehavior = MediaState.Manual
            ;
        MediaElement.Play();
    }
    
}