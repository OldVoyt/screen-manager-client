using ScreenManagerClient.Models.RemoteConfig;

namespace ScreenManagerClient.Models;

public record LocalConfigV3(
    UpdatePeriod UpdatePeriod, 
    ScreenInfo? CurrentScreenInfo,
    RemoteConfigCopy RemoteConfigCopy);