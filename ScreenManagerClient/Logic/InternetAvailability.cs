using System;
using System.Net;
using System.Runtime;
using System.Runtime.InteropServices;
 
public class InternetAvailability
{
 
    public static bool IsInternetAvailable()
    {
        return CheckConnection("https://google.com");
    }


    private static bool CheckConnection(String URL)
    {
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Timeout = 5000;
            request.Credentials = CredentialCache.DefaultNetworkCredentials;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
                return true;
            else
                return false;
        }
        catch
        {
            return false;
        }
    }
}