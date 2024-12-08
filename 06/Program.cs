var s=await AOC.GetData().AsLines();

var test="""
....#.....
.........#
..........
..#.......
.......#..
..........
.#..^.....
........#.
#.........
......#...
""";
//s=test.Split("\r\n");

var data=s.Select(x=>x.ToArray()).Where(x=>x.Any()).ToArray();
var size=(sX:data[0].Length,sY:data.Length);
var startPosition=data.Find(x=>x=='^');

var (count,loop,pathGrid,visited)=CalcGuardPath(startPosition,data);

pathGrid.Dump(x=>x.AsString(separator:""));
Console.WriteLine(count);

var loopCount=0;
foreach(var pos in visited.Keys.Select(x=>x.Position).Distinct().Where(x=>x!=startPosition))
{
    var testData=data.Select(x=>x.ToArray()).ToArray();
    testData[pos.Y][pos.X]='#';
    var result=CalcGuardPath(startPosition,testData);
    if (result.IsLoop)
    {
        //result.PathGrid.Dump(x=>x.AsString(separator:""));
        //Console.WriteLine(pos);
        loopCount++;
    }
}
Console.WriteLine(loopCount);

(int Count, bool IsLoop, char[][] PathGrid, Dictionary<((int X,int Y) Position,Direction Direction),bool> Visited) CalcGuardPath((int X, int Y) startPosition, char[][] data)
{
    data=data.Select(x=>x.ToArray()).ToArray();
    var currentPosition=startPosition;
    data[currentPosition.Y][currentPosition.X]='X';
    var currentDirection=Direction.Up;
    var visited=new Dictionary<((int X, int Y),Direction Direction),bool>();
    visited[(currentPosition,currentDirection)]=true;
    var steps=0;
    var count=1;
    var loop=false;
    while (true)
    {
        var tentativeNextPositionDelta=currentDirection switch
        {
            Direction.Up=>(X:0,Y:-1),
            Direction.Right=>(X:1,Y:0),
            Direction.Down=>(X:0,Y:1),
            Direction.Left=>(X:-1,Y:0)
        };

        var tentativeNextPosition = (X:currentPosition.X+tentativeNextPositionDelta.X,Y:currentPosition.Y+tentativeNextPositionDelta.Y);

        if (tentativeNextPosition.X<0 || tentativeNextPosition.X>=size.sX || tentativeNextPosition.Y<0 || tentativeNextPosition.Y>=size.sY)
        {
            break;
        }

        if (data[tentativeNextPosition.Y][tentativeNextPosition.X]=='#')
        {
            currentDirection=currentDirection switch
            {
                Direction.Up=>Direction.Right,
                Direction.Right=>Direction.Down,
                Direction.Down=>Direction.Left,
                Direction.Left=>Direction.Up
            };
        } else {
            currentPosition=tentativeNextPosition;
            steps++;
            if (data[currentPosition.Y][currentPosition.X]!='X')
            {
                count++;
                data[currentPosition.Y][currentPosition.X]='X';
            }
        }
        if (visited.ContainsKey((currentPosition,currentDirection)))
        {
            loop=true;
            break;
        }
        visited[(currentPosition,currentDirection)]=true;
    }
    return (count,loop, data, visited);
}


enum Direction
{
    Up,
    Right,
    Down,
    Left
}