using System.Numerics;

var s=await AOC.GetData().AsLines();

var test1="""
###############
#.......#....E#
#.#.###.#.###.#
#.....#.#...#.#
#.###.#####.#.#
#.#.#.......#.#
#.#.#####.###.#
#...........#.#
###.#.#####.#.#
#...#.....#.#.#
#.#.#.###.#.#.#
#.....#...#.#.#
#.###.#.#.#.#.#
#S..#.....#...#
###############
""";

var test2="""
#################
#...#...#...#..E#
#.#.#.#.#.#.#.#.#
#.#.#.#...#...#.#
#.#.#.#.###.#.#.#
#...#.#.#.....#.#
#.#.#.#.#.#####.#
#.#...#.#.#.....#
#.#.#####.#.###.#
#.#.#.......#...#
#.#.###.#####.###
#.#.#...#.....#.#
#.#.#.#####.###.#
#.#.#.........#.#
#.#.#.#########.#
#S#.............#
#################
""";

s=test1.Split('\n').Select(x=>x.Trim()).ToArray();

var data=s.Select(x=>x.ToArray()).ToArray();

var currentPos=data.Find(x=>x=='S');
var currentDirection=Direction.East;

(int dX, int dY)[] directionsMap=[(1,0),(0,1),(-1,0),(0,-1)];

var goalPos=data.Find(x=>x=='E');

var path=ApplyAStar(new Node(currentPos,currentDirection), new Node(goalPos,Direction.North), GetNeighbours, GetCost, GetHeuristic, IsGoal);

Console.WriteLine(path.Count);
path.Dump();

var path2=ApplyBreadthFirst(new Node(currentPos,currentDirection), new Node(goalPos,Direction.North), GetNeighbours, GetCost, IsGoal);
foreach(var p in path2)
{
    Console.WriteLine(p.Last());
}

IEnumerable<Node> GetNeighbours(Node node)
{
    foreach(var (i,direction) in directionsMap.Index())
    {
        var x=node.Pos.X+direction.dX;
        var y=node.Pos.Y+direction.dY;
        if (x<0 || y<0 || x>=data[0].Length || y>=data.Length) continue;
        if (data[y][x]=='#') continue;
        yield return new Node((x,y),(Direction)i);
    }
}

long GetCost(Node from, Node to)
{
    var distCost=Math.Abs(from.Pos.X-to.Pos.X)+Math.Abs(from.Pos.Y-to.Pos.Y);
    if (distCost>1) return long.MaxValue;
    var directionCost=Math.Abs((int)from.Direction-(int)to.Direction)%2*1000;
    return distCost+directionCost;
}

long GetHeuristic(Node from, Node to)
{
    return Math.Abs(from.Pos.X-to.Pos.X)+Math.Abs(from.Pos.Y-to.Pos.Y)+(from.Pos.X!=to.Pos.X ^ from.Pos.Y!=to.Pos.Y?1000:0);
}

bool IsGoal(Node current, Node goal)
{
    return current.Pos==goal.Pos;
}

List<(N,C)> ApplyAStar<N,C>(N start, N goal, Func<N,IEnumerable<N>> getNeighbours, Func<N,N,C> getCost, Func<N,N,C> getHeuristic, Func<N,N,bool> isGoal) where N: notnull where C: struct,IComparable<C>,ISignedNumber<C>, IEquatable<C>
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

// Return all paths from start to goal
List<List<(N,C)>> ApplyBreadthFirst<N,C>(N start, N goal, Func<N,IEnumerable<N>> getNeighbours, Func<N,N,C> getCost, Func<N,N,bool> isGoal) where N: notnull where C: struct,IComparable<C>,ISignedNumber<C>, IEquatable<C>
{
    var result=new List<List<(N,C)>>();
    var queue=new Queue<List<(N,C)>>();
    queue.Enqueue(new List<(N,C)>{(start,default(C))});
    while(queue.Count>0)
    {
        var currentPath=queue.Dequeue();
        var current=currentPath.Last().Item1;
        if (isGoal(current,goal))
        {
            result.Add(currentPath);
            continue;
        }
        foreach(var neighbour in getNeighbours(current))
        {
            if (currentPath.Any(x=>x.Item1.Equals(neighbour))) continue;
            var newPath=new List<(N,C)>(currentPath);
            newPath.Add((neighbour,getCost(current,neighbour)));
            queue.Enqueue(newPath);
        }
    }
    return result;  
}


enum Direction { East, South, West, North };

record Node((int X,int Y) Pos, Direction Direction);

