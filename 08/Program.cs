var s = await AOC.GetData();

var test = """
............
........0...
.....0......
.......0....
....0.......
......A.....
............
............
........A...
.........A..
............
............
""";

//s = test;

var data = s.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToArray()).Where(x => x.Any()).ToArray();

var size = (sX: data[0].Length, sY: data.Length);

var frequencyIndex=BuildFrequencyIndex(data, size);

Resolve(data, size, frequencyIndex, GetAntinodesPart1);
Resolve(data, size, frequencyIndex, GetAntinodesPart2);

void Resolve(char[][] data, (int sX, int sY) size, Dictionary<char, List<(int X, int Y)>> frequencyIndex, Func<(int X, int Y), (int X, int Y), (int sX, int sY), IEnumerable<(int X, int Y)>> getAntinodes)
{
    var antiNodesMap = data.Select(x=>x.ToArray()).ToArray();
    var allAntiNodes = new HashSet<(int X, int Y)>();

    foreach (var (c, list) in frequencyIndex)
    {
        Console.WriteLine($"{c}: {list.Count}");
        var antiNodes=GetPairs(list)
                            .Index()
                            .SelectMany(x=>getAntinodes(x.Item.A,x.Item.B,size).Select(y=>(x.Index,Item:y)))
                            .Where(x=>FilterOutOfBounds(x.Item))
                            .ToArray();
        foreach (var (index,item) in antiNodes)
        {
            allAntiNodes.Add(item);
            antiNodesMap[item.Y][item.X]='#';
            //Console.WriteLine($"{index}: {item}");
            //antiNodesMap.Dump(x=>x.AsString(separator:""));
            //Console.WriteLine();
            //Console.ReadKey();
        }
    }
    antiNodesMap.Dump(x=>x.AsString(separator:""));
    Console.WriteLine(allAntiNodes.Count);
}

IEnumerable<(int X, int Y)> GetAntinodesPart1((int X, int Y) A, (int X, int Y) B, (int sX, int sY) bounds)
{
    int dX = A.X - B.X;
    int dY = A.Y - B.Y;
    yield return (A.X + dX, A.Y + dY);
    yield return (B.X - dX, B.Y - dY);
}

IEnumerable<(int X, int Y)> GetAntinodesPart2((int X, int Y) A, (int X, int Y) B, (int sX, int sY) bounds)
{
    int dX = A.X - B.X;
    int dY = A.Y - B.Y;
    if (dY == 0)
    {
        for (int x = 0; x < bounds.sX; x++)
        {
            yield return (x, A.Y);
        }
        yield break;
    }
    if (dX == 0)
    {
        for (int y = 0; y < bounds.sY; y++)
        {
            yield return (A.X, y);
        }
        yield break;
    }
    double r = 1.0*dX/dY;
    if (r>1) {
        for (int x = -bounds.sX; x < 2*bounds.sX; x++)
        {
            double y=A.Y+(x-A.X)/r;
            if (Math.Round(y)==y && FilterOutOfBounds((x,(int)y)))
                yield return (x,(int)y);
        }
        yield break;
    } else {
        for (int y = -bounds.sY; y < 2*bounds.sY; y++)
        {
            double x=A.X+r*(y-A.Y);
            if (Math.Round(x)==x && FilterOutOfBounds(((int)x,y)))
                yield return ((int)x,y);
        }
        yield break;
    }
}


bool FilterOutOfBounds((int X, int Y) pos) => pos.X >= 0 && pos.X < size.sX && pos.Y >= 0 && pos.Y < size.sY;


IEnumerable<((int X, int Y) A, (int X, int Y) B)> GetPairs(List<(int X, int Y)> list)
{
    for (int i = 0; i < list.Count; i++)
        for (int j = i + 1; j < list.Count; j++)
            yield return (list[i], list[j]);
}

Dictionary<char, List<(int X, int Y)>> BuildFrequencyIndex(char[][] data, (int sX, int sY) size)
{
    var frequencyIndex = new Dictionary<char, List<(int X, int Y)>>();
    foreach (var y in Enumerable.Range(0, size.sY))
        foreach (var x in Enumerable.Range(0, size.sX))
        {
            var c = data[y][x];
            if (c != '.')
            {
                if (!frequencyIndex.TryGetValue(c, out var list))
                {
                    list = [];
                    frequencyIndex[c] = list;
                }
                list.Add((x, y));
            }
        }
    return frequencyIndex;
}