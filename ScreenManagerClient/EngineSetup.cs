using System.IO;
using System.Windows.Controls;
using System.Windows.Shapes;
using Newtonsoft.Json;
using ScreenManagerClient.Logic;
using ScreenManagerClient.Models;
using ScreenManagerClient.Models.RemoteConfig;

namespace ScreenManagerClient;

public static class EngineSetup
{

    private static Synchronizer? _synchronizer;
    public static void Start(Grid maiGrid, Grid rectangle, MediaElement mediaElement)
    {
        Logger.GetLogger().LogInfo("Starting the engine");
        var mediaPlayer = new MediaPlayer(maiGrid, rectangle, mediaElement);
        var scheduleSelector = new ScheduleSelectorV3();
        var schedulePlayer = new SchedulePlayer(mediaPlayer);
        var remoteConfigRepo = new RemoteConfigRepoV3();
        var localConfigRepo = new LocalConfigRepoV3();
        var prepareScheduleTask = new PrepareScheduleTask(mediaPlayer,localConfigRepo);
        _synchronizer ??=
            new Synchronizer(schedulePlayer, remoteConfigRepo, localConfigRepo, scheduleSelector, prepareScheduleTask);
        _synchronizer.Run();
    }
}