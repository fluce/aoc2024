var s=await AOC.GetData();

var test1="""
Button A: X+94, Y+34
Button B: X+22, Y+67
Prize: X=8400, Y=5400

Button A: X+26, Y+66
Button B: X+67, Y+21
Prize: X=12748, Y=12176

Button A: X+17, Y+86
Button B: X+84, Y+37
Prize: X=7870, Y=6450

Button A: X+69, Y+23
Button B: X+27, Y+71
Prize: X=18641, Y=10279
""";
//s=test1.Replace("\r\n","\n");
// parse data extract A,B,T
var data = s.Trim().Split("\n\n")
    .Select(x => x.Split('\n')
    .Select(y => y.Trim())
    .ToArray())
    .Select(arr => (
        A: ParseCoordinates(arr[0]),
        B: ParseCoordinates(arr[1]),
        T: ParseCoordinates(arr[2])
    ))
    .ToArray();

var costA=3;
var costB=1;

var totalCost=0L;
foreach (var (A, B, T) in data)
{
    var cost=FindMinimalCostV2(A,B,T);
    if (cost==null) Console.WriteLine("No solution");
    else 
    {
        totalCost+=cost.Value.cost;
        Console.WriteLine(cost);
    }
}
Console.WriteLine(totalCost);

var totalCost2=0L;
foreach (var (A, B, T) in data)
{
    var cost=FindMinimalCostV2(A,B,(T.X+10000000000000L,T.Y+10000000000000L));
    if (cost==null) Console.WriteLine("No solution");
    else 
    {
        totalCost2+=cost.Value.cost;
        Console.WriteLine(cost);
    }
}
Console.WriteLine(totalCost2);

static (long X, long Y) ParseCoordinates(string input)
{
    var parts = input.Split(['X', 'Y', '=', '+', ','], StringSplitOptions.RemoveEmptyEntries);
    return (long.Parse(parts[1]), long.Parse(parts[3]));
}

(long nA,long nB,long cost)? FindMinimalCostV2((long dX,long dY) A, (long dX,long dY) B, (long X,long Y) T)
{
    // T.X = nA * A.dX + nB * B.dX
    // T.Y = nA * A.dY + nB * B.dY
    
    // calculate nA and nB
    // nA = (T.X - nB * B.dX) / A.dX
    // nB = (T.Y - nA * A.dY) / B.dY
    // nA = (T.X - (T.Y - nA * A.dY) / B.dY * B.dX) / A.dX
    // nA = (T.X - T.Y / B.dY * B.dX + nA * A.dY / B.dY * B.dX) / A.dX
    // nA = (T.X - T.Y / B.dY * B.dX) / A.dX + nA * A.dY / B.dY * B.dX / A.dX
    // nA * (1 - A.dY / B.dY * B.dX / A.dX) = (T.X - T.Y / B.dY * B.dX) / A.dX
    (double X, double Y) Tf = (T.X, T.Y);
    (double dX, double dY) Af = (A.dX, A.dY);
    (double dX, double dY) Bf = (B.dX, B.dY);
    var nA = (long)Math.Round((Tf.X - Tf.Y / Bf.dY * Bf.dX) / Af.dX / (1 - Af.dY / Bf.dY * Bf.dX / Af.dX));
    var nB = (long)Math.Round((Tf.Y - nA * Af.dY) / Bf.dY);    
    var cost = CalcCost(A, B, T, nA, nB);
    Console.WriteLine($"{A} {B} {T} => {nA} {nB} => {cost}");
    if (cost == null) return null;
    return ((long)nA, (long)nB, cost.Value);
}

long? CalcCost((long dX,long dY) A, (long dX,long dY) B, (long X,long Y) T, long nA, long nB)
{
    if (nA*A.dX+nB*B.dX!=T.X || nA*A.dY+nB*B.dY!=T.Y) return null;
    return nA * costA + nB * costB;
}

(long nA,long nB,long cost)? FindMinimalCost((long dX,long dY) A, (long dX,long dY) B, (long X,long Y) T)
{
    long nA=0;
    long nB=0;
    long cost=long.MaxValue;
    long n=0;
    for(long i=0;i<=100;i++) {
        var j=(T.X-i*A.dX)/B.dX;
        var c=CalcCost(A,B,T,i,j);
        if (c!=null){
            n++;
            //Console.WriteLine($"{i} {j} {c}");
        }
        if (c!=null && c<cost)
        {
            nA=i;
            nB=j;
            cost=c.Value;
        }
    }
    if (n>1) {
        Console.WriteLine($"{A} {B} {T} {nA} {nB} {cost} {n}");
    }
    if (cost==long.MaxValue) return null;
    return (nA,nB,cost);
}



