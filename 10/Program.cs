using System.Drawing;

var s=await AOC.GetData().AsLines();

var test="""
89010123
78121874
87430965
96549874
45678903
32019012
01329801
10456732
""";

//s=test.Split("\r\n");

var data=s.Where(x=>x.Length>0).Select(x=>x.Select(y=>y-'0').ToArray()).ToArray();

var starts=data.FindAll(x=>x==0).ToArray();
starts.Dump();

var countStep1=starts.Select(x=>FindTrailTailsCount(data,x,false)).Sum();
Console.WriteLine(countStep1);

var countStep2=starts.Select(x=>FindTrailTailsCount(data,x,true)).Sum();
Console.WriteLine(countStep2);


int FindTrailTailsCount(int[][] data, (int X, int Y) start, bool step2)
{
    var visited=new HashSet<(int X, int Y)>();
    var queue=new Queue<((int X, int Y) Position, int Steps)>();
    queue.Enqueue((start,0));
    var tailList=new List<((int X, int Y) Position, int Step)>();
    while (queue.Count>0)
    {
        var (position,steps)=queue.Dequeue();
        if (!step2) {
            if (visited.Contains(position))
                continue;
            visited.Add(position);
        }
        var (x,y)=position;
        var currentAltitude=data[y][x];
        if (currentAltitude==9) 
        {
            tailList.Add((position,steps));
            continue;
        }
        if (x>0 && data[y][x-1]==currentAltitude+1)
            queue.Enqueue(((x-1,y),steps+1));
        if (x<data[y].Length-1 && data[y][x+1]==currentAltitude+1)
            queue.Enqueue(((x+1,y),steps+1));
        if (y>0 && data[y-1][x]==currentAltitude+1)
            queue.Enqueue(((x,y-1),steps+1));
        if (y<data.Length-1 && data[y+1][x]==currentAltitude+1)
            queue.Enqueue(((x,y+1),steps+1));
    }
    return tailList.Count;
}
