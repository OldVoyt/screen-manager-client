using System;
using System.Linq;
using ScreenManagerClient.Models.RemoteConfig;
using ScreenManagerClient.Models.RemoteConfigV2;
using ScreenManagerClient.Models.RemoteConfigV3;

namespace ScreenManagerClient.Logic;

public class ScheduleSelector
{
    public Schedule? SelectCurrentSchedule(AdminSettingsModel adminSettingsModel, string screenId)
    {
        var screen= adminSettingsModel.ScreenModel.Screens.FirstOrDefault(definition => definition.Id==screenId);
        if (screen == null)
        {
            return null;
        }
        var playlist = adminSettingsModel.PlaylistModel.Playlists.First(playlist1 => playlist1.Id == screen.PlaylistId);

        var storageLookup = adminSettingsModel.StorageModel.StorageCategory
            .SelectMany(category => category.StorageItems).ToDictionary(item => item.Id);

        var storageItems = playlist.PlaylistItems.Select(item => storageLookup[item.StorageItemId]);

        Schedule schedule = new Schedule(
            MediaList: storageItems.Select(item =>
                    new Media(new MediaType(item.Type), new MediaUri(item.Url), new MediaDuration(item.Duration)))
                .ToArray());
        return schedule;
    }
}
public class ScheduleSelectorV3
{
    public Schedule? SelectCurrentSchedule(CachedPlaylist cachedPlaylist)
    {
        Schedule schedule = new Schedule(
            MediaList: cachedPlaylist.PlaylistItems.Select(item =>
                    new Media(new MediaType(item.Type), new MediaUri(item.Url), new MediaDuration(item.Duration)))
                .ToArray());
        return schedule;
    }
}