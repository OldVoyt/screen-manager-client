using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ScreenManagerClient.Models;
using ScreenManagerClient.Models.RemoteConfig;
using ScreenManagerClient.Models.RemoteConfigV2;

namespace ScreenManagerClient.Logic;


public class RemoteConfigRepoV2
{
    public Task<AdminSettingsModel> GetRemoteConfig(string repoName)
    {
        return ErrorBoundary.ExecuteWithLoggingAsync(async () =>
        {
            Logger.GetLogger().LogDebug($"Going to obtain remote config from {repoName}. File name: AllSettings.json");
            var file = await (new RemoteConfigRepoHelperV2(repoName).GetFile());
            Logger.GetLogger().LogDebug($"Remote config file contents: {file.content}");
            return JsonConvert.DeserializeObject<AdminSettingsModel>(file.content);
        });
    }
}

class RemoteConfigRepoHelperV2
{
    public RemoteConfigRepoHelperV2(string repo)
    {
        _repo = repo;
    }
    private readonly string _owner = "OldVoyt";
    private readonly string _repo;
    private readonly string _path = "admin";
    private readonly string _personalAccessTokenEncoded = "Z2hwX0hiWVBjSmtkZHJ0aGJndXVIQWFBT29GbW1TUW1pNDBBY2JiSA==";

    private string PersonalAccessTokenDecoded() {
        byte[] data = Convert.FromBase64String(_personalAccessTokenEncoded);
        return Encoding.UTF8.GetString(data);
    }

    

    public async Task<ISettingFileContent> GetFile()
    {
        using (HttpClient client = new HttpClient())
        {

            var requestUri = $"https://api.github.com/repos/{_owner}/{_repo}/contents/{_path}/AllSettings.json";
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");

            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "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");
            request.Headers.Add("User-Agent", "Screen-Manager-Client");
            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            var file = JsonConvert.DeserializeObject<ISettingFileContent>(responseContent);

            file.content = Encoding.UTF8.GetString(Convert.FromBase64String(file.content));

            return file;
        }
    }

    public async Task<bool> DeleteFile(string fileName, string sha)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {

                string message = "Delete " + fileName;

                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri($"https://api.github.com/repos/{_owner}/{_repo}/contents/{_path}/{fileName}.json")
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", PersonalAccessTokenDecoded());
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));

                DeleteRequestBody body = new DeleteRequestBody
                {
                    Message = message,
                    Branch = "main",
                    Sha = sha
                };
                request.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                Console.WriteLine($"File deleted: {fileName}");
                return true;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to delete {fileName}: {e}");
            return false;
        }
    
    }

    public async Task<List<ISettingFileInfo>> GetFileList()
    {
        using (HttpClient client = new HttpClient())
        {

            List<ISettingFileInfo> fileList = new List<ISettingFileInfo>();
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://api.github.com/repos/{_owner}/{_repo}/contents/{_path}")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", PersonalAccessTokenDecoded());
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));

            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            fileList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ISettingFileInfo>>(responseBody);

            return fileList;
        }
    }
}

