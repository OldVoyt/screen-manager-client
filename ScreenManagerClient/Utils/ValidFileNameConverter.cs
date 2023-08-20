using System.IO;
using System.Linq;

namespace ScreenManagerClient.Logic;

public class ValidFileNameConverter
{  
    static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();

    public static string GetValidFileName(string s)
    {
       return new string(s.Where(ch => !InvalidFileNameChars.Contains(ch)).ToArray());
    }
}