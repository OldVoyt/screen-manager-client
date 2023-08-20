using Newtonsoft.Json;

namespace ScreenManagerClient.Logic;

public class JsonEqualityChecker
{
    public static bool IsEqual(object o1, object o2)
    {
        return JsonConvert.SerializeObject(o1) == JsonConvert.SerializeObject(o2);
    }
}