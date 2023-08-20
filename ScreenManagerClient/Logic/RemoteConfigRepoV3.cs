using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ScreenManagerClient.Models.RemoteConfigV2;
using ScreenManagerClient.Models.RemoteConfigV3;

namespace ScreenManagerClient.Logic;

public class RemoteConfigRepoV3
{
    public Task<CachedPlaylist> GetRemoteConfig(string connectionId)
    {
        return ErrorBoundary.ExecuteWithLoggingAsync(async () =>
        {
            Logger.GetLogger().LogDebug($"Going to get remote config with connection {connectionId}.");

            var client = new HttpClient();
            var message = new HttpRequestMessage(HttpMethod.Get,
                "https://screen-manager-back.vercel.app/cachedPlaylist/"+connectionId);
            var result = await client.SendAsync(message);
            result.EnsureSuccessStatusCode();
        
            var model = await result.Content.ReadAsStringAsync();
            Logger.GetLogger().LogDebug($"Remote config file contents: {model}");
            return JsonConvert.DeserializeObject<CachedPlaylist>(model);
        });
    }
}