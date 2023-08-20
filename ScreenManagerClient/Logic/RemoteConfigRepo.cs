using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ScreenManagerClient.Models;
using ScreenManagerClient.Models.RemoteConfig;

namespace ScreenManagerClient.Logic;


public class FileSaveResult {
    public string sha { get; set; }
}

public class ISettingFileInfo {
    public string sha { get; set; }
    public string name { get; set; }
}

public class ISettingFileContent {
    public string sha { get; set; }
    public string content { get; set; }
}

public class RemoteConfigRepo
{
    public RemoteConfig[] GetRemoteConfigList()
    {
        return new RemoteConfig[0];
    }

    public Task<RemoteConfig> GetRemoteConfig(string repoName, string fileName)
    {
        return ErrorBoundary.ExecuteWithLoggingAsync(async () =>
        {
            Logger.GetLogger().LogDebug($"Going to obtain remote config from {repoName}. File name: {fileName}");
            var file = await (new RemoteConfigRepoHelper(repoName).GetFile(fileName));
            Logger.GetLogger().LogDebug($"Remote config file contents: {file.content}");
            return JsonConvert.DeserializeObject<RemoteConfig>(file.content);
        });
    }
}

class RemoteConfigRepoHelper
{
    public RemoteConfigRepoHelper(string repo)
    {
        _repo = repo;
    }
    private readonly string _owner = "OldVoyt";
    private readonly string _repo;
    private readonly string _path = "settings";
    private readonly string _personalAccessTokenEncoded = "Z2hwX0hiWVBjSmtkZHJ0aGJndXVIQWFBT29GbW1TUW1pNDBBY2JiSA==";

    private string PersonalAccessTokenDecoded() {
        byte[] data = Convert.FromBase64String(_personalAccessTokenEncoded);
        return Encoding.UTF8.GetString(data);
    }

    public async Task<FileSaveResult> UpdateFile(string fileName, string newContent, string sha) {
        string message = "Update test.json";
        string content = Convert.ToBase64String(Encoding.UTF8.GetBytes(newContent));

        using (HttpClient client = new HttpClient()) {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", PersonalAccessTokenDecoded());
            client.DefaultRequestHeaders.Add("Content-Type", "application/json");

            string url = $"https://api.github.com/repos/{_owner}/{_repo}/contents/{_path}/{fileName}.json";
            string bodyJson = JsonConvert.SerializeObject(new { path = _path, message, content, sha });
            StringContent body = new StringContent(bodyJson, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PutAsync(url, body);
            string responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<FileSaveResult>(responseContent);
        }
    }

    public async Task<FileSaveResult> CreateFile(string fileName, string content)
    {
        using (HttpClient client = new HttpClient())
        {

            var message = $"Create {fileName}";
            var encodedContent = Convert.ToBase64String(Encoding.UTF8.GetBytes(content));
            var requestUri = $"https://api.github.com/repos/{_owner}/{_repo}/contents/{_path}/{fileName}.json";
            var requestBody = new
            {
                path = $"{_path}/{fileName}.json",
                message = message,
                content = encodedContent,
                branch = "main"
            };
            var json = JsonConvert.SerializeObject(requestBody);

            var request = new HttpRequestMessage(HttpMethod.Put, requestUri);
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", PersonalAccessTokenDecoded());
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<FileSaveResult>(responseContent);
            return result;
        }
    }

    public async Task<ISettingFileContent> GetFile(string fileName)
    {
        using (HttpClient client = new HttpClient())
        {

            var requestUri = $"https://api.github.com/repos/{_owner}/{_repo}/contents/{_path}/{fileName}";
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

public class DeleteRequestBody
{
    public string Message { get; set; }
    public string Branch { get; set; }
    public string Sha { get; set; }
}
