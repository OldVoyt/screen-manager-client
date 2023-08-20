using System.Linq;
using System.Threading.Tasks;
using ScreenManagerClient.Models;

namespace ScreenManagerClient.Logic;

public class Synchronizer
{
    private readonly SchedulePlayer _schedulePlayer;
    private readonly RemoteConfigRepoV3 _remoteConfigRepo;
    private readonly LocalConfigRepoV3 _localConfigRepo;
    private readonly ScheduleSelectorV3 _scheduleSelector;
    private readonly Timer _timer;
    private readonly PrepareScheduleTask _prepareScheduleTask;

        public Synchronizer(
        SchedulePlayer schedulePlayer, 
        RemoteConfigRepoV3 remoteConfigRepo, 
        LocalConfigRepoV3 localConfigRepo,
        ScheduleSelectorV3 scheduleSelector, 
        PrepareScheduleTask prepareScheduleTask)
    {
        _schedulePlayer = schedulePlayer;
        _remoteConfigRepo = remoteConfigRepo;
        _localConfigRepo = localConfigRepo;
        _scheduleSelector = scheduleSelector;
        _prepareScheduleTask = prepareScheduleTask;
        _timer = new Timer();
    }
    private bool _syncFlag = false;
    public async Task Run()
    {
        Logger.GetLogger().LogInfo("Starting synchronizer");
        var localConfig = _localConfigRepo.GetConfig();
        _timer.StartTimer(new UpdatePeriod(localConfig.UpdatePeriod.Seconds), OnTimerTick);
        OnTimerTick();
    }

    private async void OnTimerTick()
    {
        try
        {
            if (_syncFlag) return;
            _syncFlag = true;
            await ErrorBoundary.ExecuteWithLoggingAsync(async () =>
            {
                _timer.StopTimer();

                Logger.GetLogger().LogDebug("Timer tick happened");
                var localConfig = _localConfigRepo.GetConfig();
                if (CheckIfScheduleCanBeUpdated(localConfig))
                {
                    await UpdateSchedule(localConfig);
                }

                Logger.GetLogger().LogDebug("Timer tick processed");

                _timer.StartTimer(new UpdatePeriod(localConfig.UpdatePeriod.Seconds), OnTimerTick);
            });
        }
        finally
        {
            _syncFlag = false;
        }

    }

    private static bool CheckIfScheduleCanBeUpdated(LocalConfigV3 localConfig)
    {
        if (!InternetAvailability.IsInternetAvailable())
        {
            return false;
        }
        return localConfig.CurrentScreenInfo != null;
    }

    private async Task UpdateSchedule(LocalConfigV3 localConfig)
    {
        var remoteConfig =
            await _remoteConfigRepo.GetRemoteConfig(localConfig.CurrentScreenInfo.ConnectionId);
        var currentSchedule = _scheduleSelector.SelectCurrentSchedule(remoteConfig);
        await _prepareScheduleTask.PrepareScheduleForPlaying(currentSchedule);
        _schedulePlayer.PlaySchedule(currentSchedule);
    }
}