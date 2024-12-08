using System.Collections;

var s=await AOC.GetData().AsLines();

var test="""
190: 10 19
3267: 81 40 27
83: 17 5
156: 15 6
7290: 6 8 6 15
161011: 16 10 13
192: 17 8 14
21037: 9 7 18 13
292: 11 6 16 20
""".Split("\r\n");

//s=test;

var data=s.Select(x=>x.Split(':', 2))
          .Where(x=>x.Length>0 && x[0].Length>0)
          .Select(x=>
             (
                Result:long.Parse(x[0]),
                Numbers:x[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray()
            )
          )
          .ToArray();

var testPossiblyOk=data.Select(x=>(x,IsPossiblyOk(x.Result,x.Numbers)));

var onlyPossiblyOk1=testPossiblyOk.Where(x=>x.Item2).Sum(x=>x.Item1.Result);

Console.WriteLine(onlyPossiblyOk1);

var remaining = testPossiblyOk.Where(x=>!x.Item2).ToArray();
var testPossiblyOk2=remaining.Select(x=>(x.x,IsPossiblyOk2(x.x.Result,x.x.Numbers)));
var onlyPossiblyOk2=testPossiblyOk2.Where(x=>x.Item2).Sum(x=>x.Item1.Result);
Console.WriteLine(onlyPossiblyOk2+onlyPossiblyOk1);



bool IsPossiblyOk(long result, long[] numbers)
{
    var operatorsMapSize=numbers.Length-1;
    var operatorsMapMax=1 << operatorsMapSize;
    for(var i=0;i<operatorsMapMax;i++)
    {
        var currentResult=numbers[0];
        for(int bit=0;bit<operatorsMapSize;bit++)
        {
            var op=(i & (1 << bit))!=0;
            var num=numbers[bit+1];
            if (op)
                currentResult+=num;
            else
                currentResult*=num;
            if (currentResult>result)
                break;
        }
        if (currentResult==result)
            return true;
    }
    return false;
}

List<byte[]> GetOperatorsMap(int size)
{
    if (size==1) return [[0],[1],[2]];
    var smaller=GetOperatorsMap(size-1);
    var result=new List<byte[]>();
    foreach(var s in smaller)
    {
        result.Add(s.Concat(new byte[]{0}).ToArray());
        result.Add(s.Concat(new byte[]{1}).ToArray());
        result.Add(s.Concat(new byte[]{2}).ToArray());
    }
    return result;
}

bool IsPossiblyOk2(long result, long[] numbers)
{
//    Console.WriteLine($"{result}: {numbers.AsString()} ?");
    var operatorMap=GetOperatorsMap(numbers.Length-1);
    foreach(var op in operatorMap)
    {
/*        Console.Write(numbers[0]);
        for(int i=0;i<op.Length;i++)
        {
            Console.Write(op[i]==0?"+":op[i]==1?"*":"|");
            Console.Write(numbers[i+1]);
        }
        Console.Write(" ");*/

        var currentResult=numbers[0];
        for(int i=0;i<op.Length;i++)
        {
            var num=numbers[i+1];
            switch(op[i])
            {
                case 0: currentResult+=num; break;
                case 1: currentResult*=num; break;
                case 2: currentResult=long.Parse(currentResult.ToString()+num.ToString()); break;
            }
            if (currentResult>result)
                break;
        }
        if (currentResult==result) 
        {
            //Console.WriteLine("OK");
            return true;        
        }
        //Console.WriteLine();
    }
    return false;
}