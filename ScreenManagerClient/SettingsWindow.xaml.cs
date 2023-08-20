using System;
using System.Linq;
using System.Windows;
using ScreenManagerClient.Logic;
using ScreenManagerClient.Models;
using ScreenManagerClient.Models.RemoteConfigV3;

namespace ScreenManagerClient;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
        Loaded+=   OnLoaded;
        OkButton.Click+=OkButtonOnClick;
        RetryInternetButton.Click+=RetryInternetButtonOnClick;
    }

    private void RetryInternetButtonOnClick(object sender, RoutedEventArgs e)
    {
        Initialize();
    }

    public bool IsOkClicked = false;
    public CachedPlaylist? Result = null;
    private async void OkButtonOnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            Overlay.Visibility = Visibility.Visible;
            var connectionId = ScreenConnectionIdInput.Text;
            var period = int.Parse(PeriodTextBox.Text);
            var localConfig = new LocalConfigRepoV3().GetConfig();


            var remoteConfig = await new RemoteConfigRepoV3().GetRemoteConfig(connectionId);


            var localConfigUpdated = localConfig with
            {
                UpdatePeriod = new UpdatePeriod(period),
                CurrentScreenInfo = new ScreenInfo(remoteConfig.ConnectionId),
                RemoteConfigCopy = new RemoteConfigCopy(remoteConfig)
            };

            new LocalConfigRepoV3().SaveConfig(localConfigUpdated);
            IsOkClicked = true;
            Overlay.Visibility = Visibility.Collapsed;
            this.Close();
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.ToString());
        }
        finally
        {
            Overlay.Visibility = Visibility.Collapsed;
        }

    }
    
    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        Initialize();
    }

    private void Initialize()
    {
        try
        {
            ScreenConnectionIdInput.IsEnabled = false;
            InternetFailurePanel.Visibility = Visibility.Hidden;
            var localConfig = new LocalConfigRepoV3().GetConfig();
            PeriodTextBox.Text = localConfig.UpdatePeriod.Seconds.ToString();
            if (localConfig.CurrentScreenInfo?.ConnectionId != null)
            {
                ScreenConnectionIdInput.Text = localConfig.CurrentScreenInfo.ConnectionId;
            }

            if (!InternetAvailability.IsInternetAvailable())
            {
                InternetFailurePanel.Visibility = Visibility.Visible;
                return;
            }
            InternetFailurePanel.Visibility = Visibility.Hidden;
            ScreenConnectionIdInput.IsEnabled = true;
        }
        finally
        {

        }
    }
}