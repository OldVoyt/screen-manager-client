using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;
using CefSharp;
using CefSharp.Wpf;
using Path = System.IO.Path;

namespace ScreenManagerClient.Logic;

public class MediaPlayer
{
    private readonly Grid _mainGrid;
    private readonly Grid _rectangle;
    private readonly MediaElement _mediaElement;

    public MediaPlayer(Grid mainGrid, Grid rectangle, MediaElement mediaElement)
    {
        _mainGrid = mainGrid;
        _rectangle = rectangle;
        _mediaElement = mediaElement;
    }

    public void ShowWebsite(string url)
    {
        ErrorBoundary.ExecuteWithLogging(() =>
        {
            _mainGrid.Dispatcher.Invoke(async () =>
            {
                _mediaElement.Visibility = Visibility.Hidden;
                _rectangle.Visibility = Visibility.Visible;
                var browser = GetWebBrowser();
                if (browser.Address == url)
                {
                    return;
                }
                _mainGrid.Visibility = Visibility.Visible;
                await FadeOut();
                browser.Address = url;
            });
        });
    }

    public async Task ShowMediaFromDisk(string url, string extension, int mediaDurationDurationInSeconds, CancellationToken cancellationToken)
    {
        await ErrorBoundary.ExecuteWithLoggingAsync(async () =>
        {
            var fileName = ValidFileNameConverter.GetValidFileName(url);
            var duration = TimeSpan.Zero;
            await _mainGrid.Dispatcher.Invoke(async () =>
            {
                await FadeOut();
                _mainGrid.Visibility = Visibility.Hidden;
                _mediaElement.Visibility = Visibility.Visible;
                _mediaElement.Source = new Uri(@$"./FilesCache/{fileName}.{extension}", UriKind.Relative);

                _mediaElement.LoadedBehavior = MediaState.Manual;
                _mediaElement.UnloadedBehavior = MediaState.Manual;

                _mediaElement.Play();
                await WaitForValueHelper.WaitUntil(200, () => !_mediaElement.IsBuffering);
                await Task.Delay(500, cancellationToken);
                await FadeIn();
                if (_mediaElement.NaturalDuration.HasTimeSpan)
                {
                    duration = _mediaElement.NaturalDuration.TimeSpan;
                }
                else
                {
                    duration = TimeSpan.FromSeconds(mediaDurationDurationInSeconds);
                }
            });

            await Task.WhenAny(
                Task.Delay(duration.Subtract(TimeSpan.FromMilliseconds(500)), cancellationToken),
                Task.Delay(mediaDurationDurationInSeconds * 1000, cancellationToken)
            );

            await _mainGrid.Dispatcher.Invoke(async () =>
            {
                await FadeOut();
                _mediaElement.Stop();
            });
        });
    }

    private ChromiumWebBrowser? GetWebBrowser()
    {
        if (_mainGrid.Children.Count == 1 && _mainGrid.Children[0] is ChromiumWebBrowser)
        {
            return _mainGrid.Children[0] as ChromiumWebBrowser;
        }

        var browser = new ChromiumWebBrowser();
        browser.FrameLoadEnd += BrowserOnFrameLoadEnd;
        _mainGrid.Children.Add(browser);
        return browser;
    }

    private void BrowserOnFrameLoadEnd(object sender, FrameLoadEndEventArgs args)
    {
        if (args.Frame.IsMain)
        {
            args
                .Browser
                .MainFrame
                .ExecuteJavaScriptAsync(
                    "document.body.style.overflow = 'hidden'");
            _mainGrid.Dispatcher.Invoke(async () =>
            {
                Thread.Sleep(1000);
                await FadeIn();
            });
        }
    }

    private async Task FadeOut()
    {
        if (_rectangle.Opacity == 1)
        {
            return;
        }

        for (double opacity = 0; opacity <= 1; opacity += 0.1)
        {
            _rectangle.Opacity = opacity;
            await Task.Delay(50);
        }

        _rectangle.Opacity = 1;
    }

    private async Task FadeIn()
    {
        if (_rectangle.Opacity == 0)
        {
            return;
        }

        for (double opacity = 1; opacity >= 0; opacity -= 0.1)
        {
            _rectangle.Opacity = opacity;
            await Task.Delay(50);
        }

        _rectangle.Opacity = 0;
    }
}