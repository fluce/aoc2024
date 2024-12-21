using System.Numerics;
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

    public static void Dump<T>(this T[,] data, Func<T, string>? selector = null, string separator = " ")
    {
        for (int y = 0; y < data.GetLength(1); y++)
        {
            for (int x = 0; x < data.GetLength(0); x++)
            {
                Console.Write(selector?.Invoke(data[x, y]) ?? data[x, y]?.ToString() ?? "null");
                Console.Write(separator);
            }
            Console.WriteLine();
        }        
    }

    public static void ColoredDump<T>(this T[,] data, Func<T, (string? s, ConsoleColor? c)>? selector = null, string separator = " ")
    {
        var color=Console.ForegroundColor;
        for (int y = 0; y < data.GetLength(1); y++)
        {
            for (int x = 0; x < data.GetLength(0); x++)
            {
                var s=selector?.Invoke(data[x, y]);
                if (s?.c!=null)
                {
                    Console.ForegroundColor=s.Value.c.Value;
                }
                Console.Write(s?.s ?? data[x, y]?.ToString() ?? "null");
                Console.Write(separator);
            }
            Console.WriteLine();
        }        
        Console.ForegroundColor=color;
    }

    public static void ColoredDump<T>(this T[][] data, Func<T, (string? s, ConsoleColor? c)>? selector = null, string separator = " ")
    {
        var color=Console.ForegroundColor;
        for (int y = 0; y < data.Length; y++)
        {
            for (int x = 0; x < data[y].Length; x++)
            {
                var s=selector?.Invoke(data[y][x]);
                if (s?.c!=null)
                {
                    Console.ForegroundColor=s.Value.c.Value;
                }
                Console.Write(s?.s ?? data[y][x]?.ToString() ?? "null");
                Console.Write(separator);
            }
            Console.WriteLine();
        }        
        Console.ForegroundColor=color;
    }

    public static string AsString<T>(this IEnumerable<T> items, Func<T, string>? selector = null, string separator = " ")
    {
        return string.Join(separator, items.Select(x => selector?.Invoke(x) ?? x?.ToString() ?? "null"));
    }

    public static (int X, int Y) Find<T>(this T[][] data, Func<T, bool> predicate)
    {
        for (int y = 0; y < data.Length; y++)
        {
            for (int x = 0; x < data[y].Length; x++)
            {
                if (predicate(data[y][x]))
                {
                    return (x, y);
                }
            }
        }
        return (-1, -1);
    }

    public static IEnumerable<(int X, int Y)> FindAll<T>(this T[][] data, Func<T, (int X, int Y), bool> predicate)
    {
        for (int y = 0; y < data.Length; y++)
        {
            for (int x = 0; x < data[y].Length; x++)
            {
                if (predicate(data[y][x],(x,y)))
                {
                    yield return (x, y);
                }
            }
        }        
    }

    public static IEnumerable<((int X, int Y) C,T Item)> Enum<T>(this T[][] data)
    {
        for (int y = 0; y < data.Length; y++)
        {
            for (int x = 0; x < data[y].Length; x++)
            {
                {
                    yield return ((x, y),data[y][x]);
                }
            }
        }        
    }

    public static IEnumerable<((int X, int Y) C,T Item)> Enum<T>(this T[,] data)
    {
        for (int y = 0; y < data.GetLength(1); y++)
        {
            for (int x = 0; x < data.GetLength(0); x++)
            {
                {
                    yield return ((x, y),data[x,y]);
                }
            }
        }        
    }

    public static List<(N Node,C Cost)> ApplyAStar<N,C>(N start, N goal, Func<N,IEnumerable<N>> getNeighbours, Func<N,N,C> getCost, Func<N,N,C> getHeuristic, Func<N,N,bool> isGoal) where N: notnull where C: struct,IComparable<C>,ISignedNumber<C>, IEquatable<C>
    {
        var closedSet=new HashSet<N>();
        var openSet=new PriorityQueue<N,C>();
        var openSetAsSet=new HashSet<N>();
        var cameFrom=new Dictionary<N,N>();
        var gScore=new Dictionary<N,C>{{start,default(C)}};

        openSet.Enqueue(start,getHeuristic(start,goal));
        openSetAsSet.Add(start);

        while(openSet.Count>0)
        {
            var current=openSet.Dequeue();
            if (isGoal(current,goal))
            {
                var result=new List<(N,C)>();
                while(current!=null)
                {
                    var prev=cameFrom.GetValueOrDefault(current);
                    result.Add((current, gScore[current]));
                    current=prev;
                }
                result.Reverse();
                return result;
            }
            closedSet.Add(current);
            openSetAsSet.Remove(current);
            foreach(var neighbour in getNeighbours(current))
            {
                if (closedSet.Contains(neighbour)) continue;
                var tentativeGScore=gScore[current]+getCost(current,neighbour);
                if (!gScore.TryGetValue(neighbour,out var neighbourGScore) || tentativeGScore.CompareTo(neighbourGScore) < 0)
                {
                    cameFrom[neighbour]=current;
                    gScore[neighbour]=tentativeGScore;
                    
                    if (!openSetAsSet.Contains(neighbour)) {
                        openSet.Enqueue(neighbour,gScore[neighbour]+getHeuristic(neighbour,goal));
                        openSetAsSet.Add(neighbour);
                    }
                }
            }
        }
        return new List<(N,C)>();
    }


}

internal class AOCMarker {}