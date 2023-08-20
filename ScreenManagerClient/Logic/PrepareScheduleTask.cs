using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ScreenManagerClient.Models.RemoteConfig;

namespace ScreenManagerClient.Logic;

public class PrepareScheduleTask
{
    private readonly MediaPlayer _mediaPlayer;
    private readonly LocalConfigRepoV3 _localConfigRepo;
    private Schedule? _currentSchedule;

    public PrepareScheduleTask(MediaPlayer mediaPlayer, LocalConfigRepoV3 localConfigRepo)
    {
        _mediaPlayer = mediaPlayer;
        _localConfigRepo = localConfigRepo;
    }

    public async Task PrepareScheduleForPlaying(Schedule? schedule)
    {
        await ErrorBoundary.ExecuteWithLoggingAsync(async () =>
        {
            if (schedule == null ||
                (_currentSchedule != null && JsonEqualityChecker.IsEqual(_currentSchedule, schedule)))
            {
                return;
            }

            _currentSchedule = schedule;
            // _mediaPlayer.ShowWebsite(_localConfigRepo.GetConfig().DefaultUriWhileLoading.Uri);
            foreach (var media in schedule.MediaList)
            {
                await PrepareMediaToShow(media);
            }
        });

    }

    private async Task PrepareMediaToShow(Media media)
    {
       await ErrorBoundary.ExecuteWithLoggingAsync(async () =>
       {
           switch (media.MediaType.Type)
           {
               case "website":
                   return;
               case "googleDiskVideo":
                   await DownloadFileFromGoogleDriveIfNeeded(media.MediaUri.Uri,"mp4");
                   break;
               case "googleDiskImage":
                   await DownloadFileFromGoogleDriveIfNeeded(media.MediaUri.Uri,"jpg");
                   break;
           }
       });
    }
    private async Task DownloadFileFromGoogleDriveIfNeeded(string uri, string extension)
    {
        Directory.CreateDirectory("./FilesCache");

        var validFilename = ValidFileNameConverter.GetValidFileName(uri);
        var filePath = $"./FilesCache/{validFilename}.{extension}";
        if (!File.Exists(filePath))
        {
            Logger.GetLogger().LogInfo($"Downloading file: {filePath}.");
            await GoogleDiskFileDownloader.DownloadFileFromGoogleDisk(uri, filePath);
        }
        else
        {
            Logger.GetLogger().LogDebug($"Skip file download: {filePath}. Already downloaded");
        }
    }
}