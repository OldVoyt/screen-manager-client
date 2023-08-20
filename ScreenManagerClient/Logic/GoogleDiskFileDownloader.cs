using System.Threading.Tasks;

namespace ScreenManagerClient.Logic;

public class GoogleDiskFileDownloader
{
    public GoogleDiskFileDownloader()
    {
        
    }

    public static async Task DownloadFileFromGoogleDisk(string uri, string fileName)
    {
        var downloder = new FileDownloader();
        await Task.Run(() => downloder.DownloadFile(uri, fileName));
    }
}