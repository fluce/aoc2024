var s=await AOC.GetData().AsLines();

var test="""
47|53
97|13
97|61
97|47
75|29
61|13
75|53
29|13
97|29
53|29
61|53
97|53
61|29
47|13
75|47
97|75
47|61
75|61
47|29
75|13
53|13

75,47,61,53,29
97,61,53,29,13
75,29,13
75,97,47,61,53
61,13,29
97,13,75,29,47
"""
.Split("\r\n");

//s=test;

var pairs=s.TakeWhile(x=>x.Length>0).Select(x=>x.Split('|')).ToArray();
var lists=s.Skip(pairs.Length+1).Where(x=>x.Length>0).Select(x=>x.Split(',').ToArray()).ToArray();

pairs.Dump(x=>x.AsString());
lists.Dump(x=>x.AsString());

var map=pairs.GroupBy(x=>x[0]).ToDictionary(x=>x.Key,x=>x.Select(y=>y[1]).ToArray());
var unsorted=new List<string[]>();
var sum=0;
foreach(var list in lists)
{
    bool ok=true;
    for(int i=0;i<=list.Length;i++)
    {
        for(int j=i+1;j<list.Length;j++)
        {
            if (!(map.TryGetValue(list[i],out var arr) && arr.Contains(list[j])))
            {
                ok=false;
                break;
            }
        }
        if (!ok)
            break;
    }
    if (ok) {
        Console.WriteLine(list.AsString());
        sum+=int.Parse(list[(list.Length-1)/2]);
    } else {
        unsorted.Add(list);
    }
}

Console.WriteLine(sum);

Comparer<string> comparer=Comparer<string>.Create((x,y)=>map.TryGetValue(x, out var xx) && xx.Contains(y)?-1:
                                                         map.TryGetValue(y,out var yy) && yy.Contains(x)?1:0);
var sum2=0;
foreach(var list in unsorted)
{
    var sorted=list.Order(comparer).ToArray();
    sum2+=int.Parse(sorted[(sorted.Length-1)/2]);
}

Console.WriteLine(sum2);
