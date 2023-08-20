using System.IO;
using Newtonsoft.Json;
using ScreenManagerClient.Models;

namespace ScreenManagerClient.Logic;

public class LocalConfigRepoV3
{
    private static LocalConfigV3? _cachedLocalConfig = null;
    public void SaveConfig(LocalConfigV3 localConfig)
    {
        ErrorBoundary.ExecuteWithLogging(() =>
        {
            Logger.GetLogger().LogDebug("Saving local config");
            File.WriteAllText(@"LocalConfig\configV3.json", JsonConvert.SerializeObject(localConfig, Formatting.Indented));
            _cachedLocalConfig = localConfig;
        });
    }

    public LocalConfigV3 GetConfig()
    {
       return ErrorBoundary.ExecuteWithLogging(() =>
        {
            if (_cachedLocalConfig != null) return _cachedLocalConfig;
            Logger.GetLogger().LogDebug("Getting local config");
            var fileContent = File.ReadAllText(@"LocalConfig\configV3.json");
            _cachedLocalConfig = JsonConvert.DeserializeObject<LocalConfigV3>(fileContent);
            return _cachedLocalConfig;
        });
    }
}