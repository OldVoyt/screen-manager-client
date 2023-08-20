using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ScreenManagerClient.Models.RemoteConfig;

namespace ScreenManagerClient.Logic;

public class SchedulePlayer
{
    private readonly MediaPlayer _mediaPlayer;
    private Schedule? _currentSchedule;
    private SchedulePlayerCycle? _schedulePlayerCycle;

    public SchedulePlayer(MediaPlayer mediaPlayer)
    {
        _mediaPlayer = mediaPlayer;
    }

    public void PlaySchedule(Schedule? schedule)
    {
        if (_currentSchedule != null && JsonEqualityChecker.IsEqual(_currentSchedule,schedule))
        {
            return;
        }
        _currentSchedule = schedule;
        _schedulePlayerCycle?.Stop();
        _schedulePlayerCycle = new SchedulePlayerCycle(schedule, _mediaPlayer);
    }


}