using System.Collections.Immutable;

var s=await AOC.GetData();

var test="125 17";

//s=test;

var data=s.TrimEnd().Split(' ').Select(long.Parse).ToArray();

var list=new LinkedList<long>(data);
for(int i=0;i<25;i++)
{
    CalcIter(list);
}
var resultPart1=list.Count;
Console.WriteLine(list.Count);

var memory=new Dictionary<(long,int),long>();

var result=CalcItersList(data,75);

Console.WriteLine($"memory used : {memory.Count}");

Console.WriteLine(result);


long CalcItersList(IEnumerable<long> starts, int iterCount)
{
    return starts.Sum(x=>CalcIters(x,iterCount));
}

long CalcIters(long start, int iterCount)
{
    if (iterCount==75) {
        Console.WriteLine($"Start: {start}");
    }
    if (iterCount==0) return 1;
    if (memory.TryGetValue((start,iterCount), out var result))
    {
        return result;
    }
    var list=new LinkedList<long>();
    list.AddFirst(start);
    CalcIter(list);
    //Console.WriteLine($"{iterCount} {start} -> {string.Join(" ",list)}");
    var res=list.Sum(x=>CalcIters(x,iterCount-1));
    memory[(start,iterCount)]=res;
    return res;
}

static void CalcIter(LinkedList<long> list)
{
    var current = list.First;
    while (current != null)
    {
        var next = current.Next;

        switch (current.Value)
        {
            case 0:
                {
                    current.Value = 1;
                    break;
                }
            case long x when x.ToString().Length % 2 == 0:
                {
                    var t = x.ToString();
                    var half = t.Length / 2;
                    current.Value = long.Parse(t[..half]);
                    list.AddAfter(current, new LinkedListNode<long>(long.Parse(t[half..])));
                    break;
                }
            default:
                {
                    if (current.Value >= long.MaxValue / 2024)
                    {
                        throw new Exception("Too big");
                    }
                    else
                        current.Value *= 2024;
                    break;
                }
        }

        if (next == null) break;
        current = next;
    }
}