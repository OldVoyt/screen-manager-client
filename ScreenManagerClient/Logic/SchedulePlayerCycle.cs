using System;
using System.Threading;
using System.Threading.Tasks;
using ScreenManagerClient.Models.RemoteConfig;

namespace ScreenManagerClient.Logic;

public class SchedulePlayerCycle
{
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly MediaPlayer _mediaPlayer;

    public SchedulePlayerCycle(Schedule? schedule, MediaPlayer mediaPlayer)
    {
        _mediaPlayer = mediaPlayer;
        _cancellationTokenSource = new CancellationTokenSource();
        StartSchedule(schedule);
    }

    private async void StartSchedule(Schedule? schedule)
    {
        if (schedule==null)
        {
            return;
        }
        await ErrorBoundary.ExecuteWithLoggingAsync(async () =>
        {
            var index = 0;
            while (true)
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }
                if (index == schedule.MediaList.Length)
                {
                    index = 0;
                }

                await PlayMedia(schedule.MediaList[index]);
                index++;
            }
        });
    }

    private async Task PlayMedia(Media media)
    {
        switch (media.MediaType.Type)
        {
            case "website":
                await PlayWebsite(media);
                break;
            case "googleDiskVideo":
                await PlayFromGoogleDisk(media,"mp4");
                break;
            case "googleDiskImage":
                await PlayFromGoogleDisk(media,"jpg");
                break;
        }
    }

  

    private async Task PlayFromGoogleDisk(Media media, string extension)
    {
        try
        {
            if (_cancellationTokenSource.Token.IsCancellationRequested) return;
            await _mediaPlayer.ShowMediaFromDisk(media.MediaUri.Uri, extension, media.MediaDuration.DurationInSeconds, _cancellationTokenSource.Token);
        }
        catch (TaskCanceledException e)
        {
            //it's ok
        }
    }
    
    private async Task PlayWebsite(Media media)
    {
        try
        {
            if(!InternetAvailability.IsInternetAvailable()) return;
            if (_cancellationTokenSource.Token.IsCancellationRequested) return;
            _mediaPlayer.ShowWebsite(media.MediaUri.Uri);
            await Task.Delay(media.MediaDuration.DurationInSeconds * 1000, _cancellationTokenSource.Token);
        }
        catch (TaskCanceledException e)
        {
            //it's ok
        }
    }
    
    public void Stop()
    {
        _cancellationTokenSource.Cancel();
    }
}