var s=await AOC.GetData().AsLines();

var test="""
###############
#...#...#.....#
#.#.#.#.#.###.#
#S#...#.#.#...#
#######.#.#.###
#######.#.#...#
#######.#.###.#
###..E#...#...#
###.#######.###
#...###...#...#
#.#####.#.###.#
#.#...#.#.#...#
#.#.#.#.#.#.###
#...#...#...###
###############
""";

//s=test.Split(['\n','\r'],StringSplitOptions.RemoveEmptyEntries).ToArray();

var map=s.Select(x=>x.ToArray()).Where(x=>x.Length>0).ToArray();

(int X,int Y) start=map.Find(x=>x=='S');
(int X,int Y) end=map.Find(x=>x=='E');

Console.WriteLine($"Start: {start} End: {end}");

var visitedList=new List<Node>();

bool IsWall(Node node) => map[node.Y][node.X]=='#';

IEnumerable<Node> GetNeighbours(Node node, Func<Node,bool> isWall)
{
    visitedList.Add(node);
    if (node.X>0 && !isWall(new Node(node.X-1,node.Y))) yield return new Node(node.X-1,node.Y);
    if (node.Y>0 && !isWall(new Node(node.X,node.Y-1))) yield return new Node(node.X,node.Y-1);
    if (node.X<map[node.Y].Length-1 && !isWall(new Node(node.X+1,node.Y))) yield return new Node(node.X+1,node.Y);
    if (node.Y<map.Length-1 && !isWall(new Node(node.X,node.Y+1))) yield return new Node(node.X,node.Y+1);    
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
    if (current==goal) {
        return true;
    };
    return false;
}

Func<Node,bool> IsNotPassable(Node passableWall) => (Node node) => IsWall(node) && node!=passableWall;


var list=AOC.ApplyAStar(new Node(start.X,start.Y), new Node(end.X,end.Y), (x)=>GetNeighbours(x,IsWall), GetCost, GetHeuristic, IsGoal);
var baseTime=list.Count-1;
Console.WriteLine(baseTime);
/*foreach(var p in visitedList) map[p.Y][p.X]='V';
foreach(var p in list) map[p.Node.Y][p.Node.X]='O';
map.ColoredDump(x=>x switch { '#'=>("#",ConsoleColor.Cyan),'O'=>("O",ConsoleColor.Yellow),'.'=>(".",ConsoleColor.White),(var z)=>(z.ToString(),ConsoleColor.Red) });
*/
//return;
var min=list.Count-1;
Node minWall=new Node(0,0);
var cheatCount=0;
var wallToCheck=AOC.FindAll(map,(x,pos)=>pos.X>0 && pos.Y>0 && pos.X<map[pos.Y].Length-1 && pos.Y<map.Length-1 && x=='#').ToArray();
Console.WriteLine(wallToCheck.Length);
foreach(var (i,w) in wallToCheck.Index())
{
    if (i%100==0) Console.WriteLine(i);
    var passableWall=new Node(w.X,w.Y);
    visitedList.Clear();
    var passableWallCheck=IsNotPassable(passableWall);
    list=AOC.ApplyAStar(new Node(start.X,start.Y), new Node(end.X,end.Y), (x)=>GetNeighbours(x,passableWallCheck), GetCost, GetHeuristic, IsGoal);
    if (baseTime-(list.Count-1) >= 100) cheatCount++;
    if (list.Count-1<min) {
        min=list.Count-1;
        minWall=passableWall;
    }
}
Console.WriteLine($"{minWall} => {min} => {baseTime-min}");
Console.WriteLine(cheatCount);

visitedList.Clear();
var passableWallCheck2=IsNotPassable(minWall);
list=AOC.ApplyAStar(new Node(start.X,start.Y), new Node(end.X,end.Y), (x)=>GetNeighbours(x,passableWallCheck2), GetCost, GetHeuristic, IsGoal);

foreach(var p in visitedList) map[p.Y][p.X]='V';
foreach(var p in list) map[p.Node.Y][p.Node.X]='O';
map.ColoredDump(x=>x switch { '#'=>("#",ConsoleColor.Cyan),'O'=>("O",ConsoleColor.Yellow),'.'=>(".",ConsoleColor.White),(var z)=>(z.ToString(),ConsoleColor.Red) });


record Node(int X,int Y);