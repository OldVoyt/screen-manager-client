using System;
using System.Threading.Tasks;

namespace ScreenManagerClient.Logic;

public class WaitForValueHelper
{
    public static async Task WaitUntil(int millisecondPeriod, Func<bool> check)
    {
        while (true)
        {
            if (check())
            {
                return;
            }
            await Task.Delay(millisecondPeriod);
        }
    }

}