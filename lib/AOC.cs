using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace lib;

public static class AOC
{
    public static Task<string> GetData(int day)
    {
        HttpClient client = new HttpClient();
        string url = $"https://adventofcode.com/2024/day/{day}/input";
        client.DefaultRequestHeaders.Add("Cookie", $"session={SessionId}");
        return client.GetStringAsync(url);
    }

    public static Task<string> GetData()
    {
        int day = int.Parse(Assembly.GetEntryAssembly().GetName().Name);
        return GetData(day);
    }

    public static Task<string> GetData(string testData)
    {
        return Task.FromResult(testData);
    }


    private static string SessionId { get; set; }

    static AOC()
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddUserSecrets<AOCMarker>()
            .Build();        
        SessionId = config["session"];
    }

    public static async Task<string[]> AsLines(this Task<string> task)
    {
        return (await task).Split("\n");
    }

    public static void Dump<T>(this IEnumerable<T> items, Func<T, string>? selector = null)
    {
        foreach (var item in items)
        {
            Console.WriteLine(selector?.Invoke(item) ?? item?.ToString() ?? "null");
        }
    }

    public static string AsString<T>(this IEnumerable<T> items, Func<T, string>? selector = null, string separator = " ")
    {
        return string.Join(separator, items.Select(x => selector?.Invoke(x) ?? x?.ToString() ?? "null"));
    }
}

internal class AOCMarker {}