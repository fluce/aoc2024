var s=await AOC.GetData().AsLines();

var test="""
029A
980A
179A
456A
379A
""".Trim().Split(['\n','\r'],StringSplitOptions.RemoveEmptyEntries);

s=test;

var data=s.Where(x=>x.Length>0).ToArray();

var keyboard1=new char[,]{
    {'7','8','9'},
    {'4','5','6'},
    {'1','2','3'},
    {' ','0','A'},
};

var allKeys1=keyboard1.Enum().Where(x=>x.Item!=' ').ToDictionary(x=>x.Item,x=>x.C);
var keyboard1Distances=new Dictionary<(char,char),int>();
foreach(var a in allKeys1){
    foreach(var b in allKeys1){
        if (a.Key==b.Key) continue;
        keyboard1Distances[(a.Key,b.Key)]=Math.Abs(a.Value.X-b.Value.X)+Math.Abs(a.Value.Y-b.Value.Y);
    }
}
var neighbors1=new Dictionary<char,List<(char,char)>>()
{
    { '7', [('8','>'),('4','v')] },
    { '8', [('9','>'),('5','v'),('7','<')] },
    { '9', [('6','v'),('8','<')] },
    { '4', [('7','^'),('5','>'),('1','v')] },
    { '5', [('8','^'),('6','>'),('2','v'),('4','<')] },
    { '6', [('9','^'),('3','v'),('5','<')] },
    { '1', [('4','^'),('2','>')] },
    { '2', [('5','^'),('3','>'),('0','v'),('1','<')] },
    { '3', [('6','^'),('A','v'),('2','<')] },
    { '0', [('2','^'),('A','>')] },
    { 'A', [('3','^'),('0','<')] },
};

var keyboard2=new char[,] {
    {' ','^','A'},
    {'<','v','>'},
};
//<v A < AA >>^ A v AA <^ A > A <v< A >>^ A v A ^ A <v A >^ A <v< A >^ A > AA v A ^ A <v< A > A >^ AAA v A <^ A > A

//<vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>A
//<vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>A
var neighbors2=new Dictionary<char,List<(char,char)>>()
{
    { '^', [('A','>'),('v','v')] },
    { 'A', [('^','<'),('>','v')] },
    { '<', [('v','>')] },
    { 'v', [('>','>'),('^','^'),('<','<')] },
    { '>', [('v','<'),('A','^')] },
};

var allKeys2=keyboard2.Enum().Where(x=>x.Item!=' ').ToDictionary(x=>x.Item,x=>x.C);
var keyboard2Distances=new Dictionary<(char,char),int>();
foreach(var a in allKeys2){
    foreach(var b in allKeys2){
        if (a.Key==b.Key) continue;
        keyboard2Distances[(a.Key,b.Key)]=Math.Abs(a.Value.X-b.Value.X)+Math.Abs(a.Value.Y-b.Value.Y);
    }
}

var pathMap=allKeys1.Join(allKeys1, x=>true, x=>true, (a,b)=>(First:a,Second:b))
    .Where(x=>x.First.Key!=x.Second.Key)
    .ToDictionary(
        x=>(x.First.Key,x.Second.Key),
        x=>GetShortestPath(x.First.Key,x.Second.Key,neighbors1)
    );
pathMap.Dump();

var s4=GetMoves("029A", allKeys1, neighbors1);
Console.WriteLine(s4);
data.Dump();
var sum=data.Sum(x=>CalcOne(x));
Console.WriteLine(sum);

long CalcOne(string input)
{
    var s1=GetMoves(input, allKeys1, neighbors1);
    var s2=GetMoves(s1, allKeys2, neighbors2);
    var s3=GetMoves(s2, allKeys2, neighbors2);
    Console.WriteLine($"{input}\n{s1}\n{s2}\n{s3}");
    Console.WriteLine($"Complexity: {s3.Length} * {int.Parse(input.TrimEnd('A'))}");
    return s3.Length*int.Parse(input.TrimEnd('A'));
}
//<v<A>A<A>>^AvAA<^A>A<v<A>>^AvA^A<v<A>>^AAvA<A>^A<A>A<v<A>A>^AAAvA<^A>A
string GetShortestPath(char from, char to, Dictionary<char,List<(char,char)>> neighbors)
{
    var list=AOC.ApplyAStar(from, to, (c)=>{
        var ret=neighbors[c].Select(x=>x.Item1);
        return ret;
    }, (a,b)=>1, (a,b)=>0, (a,b)=>a==b);
    var ret=list.Aggregate(
                    (a:"",p:' '),
                    (acc,x)=>(
                        acc.a+(acc.p==' '?"":neighbors[acc.p].First(y=>y.Item1==x.Node).Item2),
                        x.Node)
    );
    return ret.a;
}

string GetMoves(string input, Dictionary<char,(int X,int Y)> keys, Dictionary<char,List<(char,char)>> neighbors){
    var pathMap=keys.Join(keys, x=>true, x=>true, (a,b)=>(First:a,Second:b))
        .Where(x=>x.First.Key!=x.Second.Key)
        .ToDictionary(
            x=>(x.First.Key,x.Second.Key),
            x=>GetShortestPath(x.First.Key,x.Second.Key,neighbors)
        );
    pathMap.Dump();
    Console.WriteLine($"Input: {input}");
    var moves="";
    var current='A';
    int i=0;
    foreach(var c in input) {
        var move="";
        //Console.WriteLine($"{current} -> {c}");
        if (current!=c)
            move=pathMap[(current,c)];
        Console.WriteLine($"{++i,2} {current} -> {c} : {move}");
        moves+=move;
        moves+="A";
        current=c;
    }
    return moves;
}

