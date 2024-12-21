using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Unicode;

var s=await AOC.GetData().AsLines();
var size=(sX:101,sY:103);

var test="""
p=0,4 v=3,-3
p=6,3 v=-1,-3
p=10,3 v=-1,2
p=2,0 v=2,-1
p=0,0 v=1,3
p=3,0 v=-2,-2
p=7,6 v=-1,-3
p=3,0 v=-1,-2
p=9,3 v=2,3
p=7,3 v=-1,2
p=2,4 v=2,-3
p=9,5 v=-3,-3
""";

//s=test.Split(['\r','\n'],StringSplitOptions.RemoveEmptyEntries).ToArray();
//size=(sX:11,sY:7);
var regex=new Regex(@"p=(?<px>\d+),(?<py>\d+) v=(?<vx>-?\d+),(?<vy>-?\d+)");

var data=s.Select(x=>x.Trim()).Where(x=>x.Length>0).Select(x=>regex.Match(x)).Select(x=>(
    pX:int.Parse(x.Groups["px"].Value),
    pY:int.Parse(x.Groups["py"].Value),
    vX:int.Parse(x.Groups["vx"].Value),
    vY:int.Parse(x.Groups["vy"].Value)
)).ToArray();

(int pX,int pY) GetPosAfterTime((int pX,int pY) p, (int vX,int vY) v, double t, (int sX,int sY) size)
{
    return ((size.sX+(p.pX+(int)(v.vX*t))%size.sX)%size.sX,(size.sY+(p.pY+(int)(v.vY*t)+size.sY)%size.sY)%size.sY);
}

int GetQuadrant((int pX,int pY) p, (int sX,int sY) size)
{
    var m=(mX:(size.sX-1)/2,mY:(size.sY-1)/2);
    if (p.pX<m.mX && p.pY<m.mY) return 1;
    if (p.pX>m.mX && p.pY<m.mY) return 2;
    if (p.pX>m.mX && p.pY>m.mY) return 3;
    if (p.pX<m.mX && p.pY>m.mY) return 4;
    return 0;
}

double currentIter=100;//84+101*25.5f;//+101*103*25.5f;

Dictionary<byte[],int> hashes=new();

while (true) {

var allPos=data.Select(x=>GetPosAfterTime((x.pX,x.pY),(x.vX,x.vY),currentIter,size)).ToArray();

//allPos.Dump(x=>$"{x} {GetQuadrant(x,size)}");

var quadrants=allPos.Select(x=>GetQuadrant(x,size)).ToArray();
var safetyFactor=quadrants.GroupBy(x=>x).Where(x=>x.Key!=0).Aggregate(1,(acc,x)=>acc*x.Count());

if (currentIter==100)
    Console.WriteLine($"Iter: {currentIter} Safety factor: {safetyFactor}");

var table=Enumerable.Range(0,(size.sY+1)/2).Select(y=>Enumerable.Range(0,size.sX).Select(x=>' ').ToArray()).ToArray();
foreach (var (pX,pY) in allPos)
{
    var c=table[pY/2][pX];
    var half=pY%2;
    table[pY/2][pX]=(c,half) switch
    {
        (' ',0)=>'▀',
        (' ',1)=>'▄',
        ('▀',0)=>'█',
        ('▄',1)=>'█',
        ('▄',0)=>'▄',
        ('▀',1)=>'▀',
        ('█',0)=>'█',
        ('█',1)=>'█',
        _=>throw new Exception()
    };
}

var all=string.Join("\n",table.Select(x=>new string(x)));
if (all.Contains("▀▀▀▀▀▀▀▀▀▀") || all.Contains("▄▄▄▄▄▄▄▄▄▄") || all.Contains("██████████")) {
    Console.WriteLine(all);
    Console.WriteLine($"Iter: {currentIter} ");
    break;
}
if (((int)currentIter)%1000==0) Console.WriteLine($"Iter: {currentIter}");

currentIter+=1f;
/*Console.Clear();
Console.WriteLine($"Iter: {currentIter}");
table.Dump(x=>x.AsString());*/
/*var key=Console.ReadKey();
switch(key.Key)
{
    case ConsoleKey.Escape:
        return;
    case ConsoleKey.LeftArrow:
        Console.WriteLine("Decrease iter");
        currentIter-=0.01f;
        break;
    case ConsoleKey.RightArrow:
        Console.WriteLine("Increase iter");
        currentIter+=0.01f;
        break;
    case ConsoleKey.UpArrow:
        currentIter+=101*103*25.5f;
        break;
    case ConsoleKey.DownArrow:
        currentIter-=101*103*25.5f;
        break;
}
*/
}
