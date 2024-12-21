var s=await AOC.GetData().AsLines();

var test="""
r, wr, b, g, bwu, rb, gb, br

brwrr
bggr
gbbr
rrbgbr
ubwu
bwurrg
brgr
bbrgwb
""";

//s=test.Split('\n').Select(x=>x.Trim()).ToArray();

var data=s.Select(x=>x.Trim()).ToArray();
var availablePatterns=data[0].Split(", ");
var designs=data.Skip(2).Where(x=>x.Length>0).ToArray();
var count=0;

foreach(var design in designs.Select(x=>(x,Possible:IsDesignPossible(x,availablePatterns))))
{
    Console.WriteLine($"{design.x} => {design.Possible}");
    if (design.Possible) count++;
}

Console.WriteLine(count);

Dictionary<string,long> cache=new();
Dictionary<string,long>.AlternateLookup<ReadOnlySpan<char>> lookup = cache.GetAlternateLookup<ReadOnlySpan<char>>();


long sum=0L;
foreach(var design in designs.Select(x=>(x,Possible:GetPossibleDesigns(x,availablePatterns))))
{
    Console.WriteLine($"{design.x} => {design.Possible}");
    sum+=design.Possible;
}
Console.WriteLine(sum);
Console.WriteLine($"cache size: {cache.Count}");


bool IsDesignPossible(string design, string[] patterns)
{
    if (design.Length==0) return true;
    foreach(var pattern in patterns)
    {
        if (design.StartsWith(pattern))
        {
            if (IsDesignPossible(design.Substring(pattern.Length),patterns)) 
                return true;
        }
    }
    return false;
}


long GetPossibleDesigns(ReadOnlySpan<char> design, string[] patterns, int depth=0)
{
    if (design.Length==0) return 1;    
    if (depth<10) Console.WriteLine($"{new string(' ',depth)}{design.ToString()}");    
    if (lookup.TryGetValue(design, out var result))
    {
        return result;
    }
    result=0L;
    foreach(var pattern in patterns)
    {
        if (design.StartsWith(pattern))
        {
            var p=GetPossibleDesigns(design.Slice(pattern.Length),patterns, depth+1);
            result+=p;
        }
    }
    cache[design.ToString()]=result;
    return result;
}