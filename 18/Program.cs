var s=await AOC.GetData().AsLines();

var size=(sX:71,sY:71); int n=1024;

var test="""
5,4
4,2
4,5
3,0
2,1
6,3
2,4
1,5
0,6
3,3
2,6
5,1
1,2
5,5
2,5
6,5
1,4
0,4
6,4
1,1
6,1
1,0
0,5
1,6
2,0
""";

//s=test.Split('\n').Select(x=>x.Trim()).ToArray(); size=(sX:7,sY:7); n=12;

var data=s.Select(x=>x.Trim()).Where(x=>x.Length>0).Select(x=>x.Trim().Split(',').Select(int.Parse).ToArray()).Select(x=>(X:x[0],Y:x[1])).ToArray();

var map=new int[size.sX,size.sY];

foreach(var d in data.Take(n))
{
    map[d.X,d.Y]=1;
}

map.ColoredDump(x=>x switch { 1=>("#",ConsoleColor.Cyan),2=>("O",ConsoleColor.Yellow),_=>(".",ConsoleColor.White) });

IEnumerable<Node> GetNeighbours(Node node)
{
    if (node.X>0 && map[node.X-1,node.Y]==0) yield return new Node(node.X-1,node.Y);
    if (node.Y>0 && map[node.X,node.Y-1]==0) yield return new Node(node.X,node.Y-1);
    if (node.X<size.sX-1 && map[node.X+1,node.Y]==0) yield return new Node(node.X+1,node.Y);
    if (node.Y<size.sY-1 && map[node.X,node.Y+1]==0) yield return new Node(node.X,node.Y+1);
}

long GetCost(Node from, Node to)
{
    var distCost=Math.Abs(from.X-to.X)+Math.Abs(from.Y-to.Y);
    if (distCost>1) return long.MaxValue;
    return distCost;
}

long GetHeuristic(Node from, Node to)
{
    return Math.Abs(from.X-to.X)+Math.Abs(from.Y-to.Y);
}

bool IsGoal(Node current, Node goal)
{
    return current==goal;
}

var list=AOC.ApplyAStar(new Node(0,0), new Node(size.sX-1,size.sY-1), GetNeighbours, GetCost, GetHeuristic, IsGoal);
Console.WriteLine(list.Count-1);
var i=n;
while(list.Count>0 && i<data.Length)
{
    i++;
    var d=data[i];
    map[d.X,d.Y]=1;
    list=AOC.ApplyAStar(new Node(0,0), new Node(size.sX-1,size.sY-1), GetNeighbours, GetCost, GetHeuristic, IsGoal);
}
Console.WriteLine($"{i} : {data[i]}");

//foreach(var p in list) map[p.Node.X,p.Node.Y]=2;
//map.ColoredDump(x=>x switch { 1=>("#",ConsoleColor.Cyan),2=>("O",ConsoleColor.Yellow),_=>(".",ConsoleColor.White) });





record Node(int X,int Y);
