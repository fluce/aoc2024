using System.Collections.Immutable;

var s=await AOC.GetData("""
7 6 4 2 1
1 2 7 8 9
9 7 6 2 1
1 3 2 4 5
8 6 4 4 1
1 3 6 7 9
"""
).AsLines();

s=await AOC.GetData().AsLines();

var data=s.Select(x=>x.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray()).Where(x=>x.Any()).ToArray();

data.Dump( x=>x.AsString()+ " : " + IsSafe(x));

var safes=data.Select(IsSafe);

var count=safes.Count(x=>x);

Console.WriteLine(count);

Console.WriteLine(data.Count(IsReallySafe));


bool IsSafe(IEnumerable<int> x)
{
    var deltaList=x.Aggregate<int,ImmutableList<(int prev,int cur)>>(
                                        [(-1,-1)],
                                        (ImmutableList<(int prev,int cur)> acc, int cur)=>acc.Add((acc.Last().cur,cur))
                                    ).Skip(2);
    var res=(deltaList.All(z=>z.prev<z.cur) || deltaList.All(z=>z.prev>z.cur)) && deltaList.All(z=>Math.Abs(z.prev-z.cur)<=3);
    Console.WriteLine(deltaList.AsString(y=>$"{y.prev}->{y.cur}[{Math.Abs(y.prev-y.cur)}]")+" : "+res);
    return res;
}

bool IsReallySafe(IEnumerable<int> x)
{
    Console.WriteLine("## "+x.AsString());
    return Enumerable.Range(0,x.Count()).Any(i=>IsSafe(WithoutIndex(x,i)));    
}

IEnumerable<T> WithoutIndex<T>(IEnumerable<T> x, int index)
{
    return x.Take(index).Concat(x.Skip(index+1));
}
