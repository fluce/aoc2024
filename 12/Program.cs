var s=await AOC.GetData().AsLines();

var test1="""
AAAA
BBCD
BBCC
EEEC
""";

var test2="""
RRRRIICCFF
RRRRIICCCF
VVRRRCCFFF
VVRCCCJFFF
VVVVCJJCFE
VVIVCCJJEE
VVIIICJJEE
MIIIIIJJEE
MIIISIJEEE
MMMISSJEEE
""";

var test3="""
EEEEE
EXXXX
EEEEE
EXXXX
EEEEE
""";

var test4="""
AAAAAA
AAABBA
AAABBA
ABBAAA
ABBAAA
AAAAAA
""";

//s=test2.Split('\n').Select(x=>x.Trim()).ToArray();

var data=s.Select(x=>x.Select(c=>new Cell(c)).ToArray()).ToArray();








var dataByRegion=new Dictionary<int,Data>();

var getData=(int i, int j)=>i>=0 && i<data.Length && j>=0 && j<data[i].Length ? data[i][j] : Cell.Empty;
var checkRegion=(int i, int j, Cell c)=>{
    var cell=getData(i,j);
    return cell==Cell.Empty || cell.Region!=c.Region;
};

var nextRegionId=0;
for(int i=0;i<data.Length;i++)
{
    for(int j=0;j<data[i].Length;j++)
    {
        var c=getData(i,j);
        if (c.Region==null) 
        {
            FillRegion(i,j,nextRegionId++,c.C);
        }
    }
}

for(int i=0;i<data.Length;i++)
{
    for(int j=0;j<data[i].Length;j++)
    {
        var c=getData(i,j);

        if (!dataByRegion.TryGetValue(c.Region.Value, out var d))
        {
            d=new Data();
            d.C=c.C;
            dataByRegion[c.Region.Value]=d;
        }
        d.Area++;
        if (checkRegion(i-1,j,c)) { d.Perimeter++; c.Sides|=Sides.Top; }
        if (checkRegion(i+1,j,c)) { d.Perimeter++; c.Sides|=Sides.Bottom; }
        if (checkRegion(i,j-1,c)) { d.Perimeter++; c.Sides|=Sides.Left; }
        if (checkRegion(i,j+1,c)) { d.Perimeter++; c.Sides|=Sides.Right; }
        Console.WriteLine($"{i},{j}:{c} {d.Area} * {d.Perimeter}");
        /*data.Dump(x=>x.AsString(x=>$"{x.C} {(x.Region==null?"  ":$"{x.Region:00}")} "));
        Console.ReadKey();*/
    }
}

data.Dump(x=>x.AsString(x=>$"{x.C} {x.Region:00} "));

var totalPrice=dataByRegion.Sum(x=>x.Value.Area*x.Value.Perimeter);
foreach(var x in dataByRegion)
{
    Console.WriteLine($"{x.Key} {x.Value.C} {x.Value.Area} {x.Value.Perimeter} => {x.Value.Area*x.Value.Perimeter}");
}
Console.WriteLine(totalPrice);

for(int i=0;i<data.Length;i++)
{
    for(int j=0;j<data[i].Length;j++)
    {
        var c = getData(i, j);
        var region = dataByRegion[c.Region.Value];
        var c1 = getData(i, j + 1);
        var c2 = getData(i + 1, j);
        var c1m = getData(i, j - 1);
        var c2m = getData(i - 1, j);

        TestSide(c, region, c1m, Sides.Top);
        TestSide(c, region, c1m, Sides.Bottom);
        TestSide(c, region, c2m, Sides.Left);
        TestSide(c, region, c2m, Sides.Right);

        /*if (c.Region == 3) {
            Console.WriteLine($"{i},{j - 1}:{c1m} {region.Sides}");
            Console.WriteLine($"{i},{j}:{c} {region.Sides}");
            data.Dump(x => x.AsString(x => $"{x.C} {Convert.ToString((int)x.FilteredSides, 2).PadLeft(4, '0')} "));
            Console.ReadKey();
        }*/
    }
}

foreach(var x in dataByRegion)
{
    Console.WriteLine($"{x.Key} {x.Value.C} {x.Value.Area} {x.Value.Sides} => {x.Value.Area*x.Value.Sides}");
}
data.Dump(x=>x.AsString(x=>$"{x.C} {Convert.ToString((int)x.Sides,2).PadLeft(4,'0')} "));
Console.WriteLine();
data.Dump(x=>x.AsString(x=>$"{x.C} {Convert.ToString((int)x.FilteredSides,2).PadLeft(4,'0')} "));

var result2=dataByRegion.Sum(x=>x.Value.Area*x.Value.Sides);
Console.WriteLine(result2);




void FillRegion(int i, int j, int region, char c)
{
    var cell=getData(i,j);
    if (cell.Region!=null || cell.C!=c) return;
    cell.Region=region;
    FillRegion(i-1,j,region,c);
    FillRegion(i+1,j,region,c);
    FillRegion(i,j-1,region,c);
    FillRegion(i,j+1,region,c);
}

static void TestSide(Cell c, Data region, Cell c1m, Sides side)
{
    if (
        (c1m.FilteredSides.HasFlag(side) && c1m.Region == c.Region, c1m.Sides.HasFlag(side) && c1m.Region == c.Region, c.Sides.HasFlag(side))
        switch
        {
            (true, _, _) => false,
            (false, true, true) => false,
            (false, false, true) => true,
            (false, _, false) => false,
        }
    )
    {
        c.FilteredSides |= side;
        region.Sides++;
    }
}

class Data {
    public char C {get;set;}
    public int Area {get;set;}
    public int Perimeter {get;set;}
    public int Sides {get;set;}
}

[Flags]
enum Sides {None=0,Top=1,Right=2,Bottom=4,Left=8}

record Cell(char C) { 
    public int? Region {get;set;} 

    public Sides Sides {get;set;} 
    public Sides FilteredSides {get;set;}
    public static Cell Empty=new Cell(' ');    
}

