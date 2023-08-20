using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ScreenManagerClient.Logic;

public class ErrorBoundary
{
    public static async Task<T> ExecuteWithLoggingAsync<T>(Func<Task<T>> func, [CallerMemberName] string source="unknown")
    {
        try
        {
            return await func();
        }
        catch (Exception e)
        {
            Logger.GetLogger().LogError($"[source: {source}]", e);
            throw;
        }
    }
    public static async Task ExecuteWithLoggingAsync(Func<Task> func, bool rethrow = true, [CallerMemberName] string source="unknown")
    {
        try
        {
            await func();
        }
        catch (Exception e)
        {
            Logger.GetLogger().LogError($"[source: {source}]", e);
            if(rethrow)throw;
        }
    }
    public static T ExecuteWithLogging<T>(Func<T> func, [CallerMemberName] string source="unknown")
    {
        try
        {
            return func();
        }
        catch (Exception e)
        {
            Logger.GetLogger().LogError($"[source: {source}]", e);
            throw;
        }
    }
    public static void ExecuteWithLogging(Action func, [CallerMemberName] string source="unknown")
    {
        try
        {
            func();
        }
        catch (Exception e)
        {
            Logger.GetLogger().LogError($"[source: {source}]", e);
            throw;
        }
    }
}