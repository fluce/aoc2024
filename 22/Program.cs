var s=await AOC.GetData().AsLines();

int[] test=[1,2,3,2024];

var data=s.Where(x=>x.Length>0).Select(x=>int.Parse(x)).ToArray();
Console.WriteLine($"length: {data.Length}");
//data=test;
/*foreach (var item in test)
{
    Console.WriteLine($"{item}: {TransformLoop(item,2000)}");
}
*/
var sum=data.Sum(x=>(long)TransformLoop(x,2000));
Console.WriteLine(sum);

Console.WriteLine(TransformLoopPart2(123,10,[-1,-1,0,2]));

sbyte[] targetDeltas=new sbyte[4];
sbyte[] maxDeltas=new sbyte[4];
int maxBananas=0;
var maxIter=19L*19*19*19;
var iter=0L;
var skippedPrefixes=new HashSet<sbyte[]>();
for (sbyte i0=-9;i0<10;i0++)
{
    targetDeltas[0]=i0;
    for (sbyte i1=-9;i1<10;i1++)
    {
        sbyte[] prefix=[i0,i1];
        if (skippedPrefixes.Contains(prefix)) continue;
        var filteredData=data.Where(x=>HasPrefix2(x,2000,prefix)).ToArray();
        if (filteredData.Length==0) {
            skippedPrefixes.Add(prefix);
            continue;
        }
        targetDeltas[1]=i1;
        for (sbyte i2=-9;i2<10;i2++)
        {
            sbyte[] prefix3=[i0,i1,i2];
            sbyte[] prefix32=[i1,i2];
            if (skippedPrefixes.Contains(prefix32)) continue;
            var filteredData2=filteredData.Where(x=>HasPrefix3(x,2000,prefix3)).ToArray();
            if (filteredData2.Length==0) continue;
            targetDeltas[2]=i2;
            for (sbyte i3=-9;i3<10;i3++)
            {
                sbyte[] prefix42=[i2,i3];
                if (skippedPrefixes.Contains(prefix42)) continue;
                targetDeltas[3]=i3;
                iter++;
                var bananas=filteredData2.Sum(x=>TransformLoopPart2(x,2000,targetDeltas));
                //Console.WriteLine($"{100L*iter/maxIter} {filteredData2.Length} {bananas} ({maxBananas}): {targetDeltas.AsString()}");
                if (bananas>maxBananas)
                {
                    maxBananas=bananas;
                    maxDeltas=targetDeltas.ToArray();
                    Console.WriteLine($"{100L*iter/maxIter} {maxBananas}: {targetDeltas.AsString()}");
                }
            }
        }
    }
}
Console.WriteLine($"{maxBananas}: {maxDeltas.AsString()}");

bool HasPrefix2(int input, int count, sbyte[] targetDeltas)
{
    sbyte[] deltas=new sbyte[2];
    for (int i = 0; i < count; i++)
    {
        var next=Transform(input);
        int price=next%10;
        sbyte delta=(sbyte)(price-input%10);
        //Console.WriteLine($"{i,4}: {input} -> {next} : {price} : {delta}");
        deltas[0]=deltas[1];
        deltas[1]=delta;
        if (deltas[0]==targetDeltas[0] && deltas[1]==targetDeltas[1])
        {
            //Console.WriteLine($"Found at {i}");
            return true;
        }
        input=next;
    }
    return false;
}

bool HasPrefix3(int input, int count, sbyte[] targetDeltas)
{
    sbyte[] deltas=new sbyte[3];
    for (int i = 0; i < count; i++)
    {
        var next=Transform(input);
        int price=next%10;
        sbyte delta=(sbyte)(price-input%10);
        //Console.WriteLine($"{i,4}: {input} -> {next} : {price} : {delta}");
        deltas[0]=deltas[1];
        deltas[1]=deltas[2];
        deltas[2]=delta;
        if (deltas[0]==targetDeltas[0] && deltas[1]==targetDeltas[1] && deltas[2]==targetDeltas[2])
        {
            //Console.WriteLine($"Found at {i}");
            return true;
        }
        input=next;
    }
    return false;
}

int TransformLoopPart2(int input, int count, sbyte[] targetDeltas)
{
    sbyte[] deltas=new sbyte[4];
    for (int i = 0; i < count; i++)
    {
        var next=Transform(input);
        int price=next%10;
        sbyte delta=(sbyte)(price-input%10);
        //Console.WriteLine($"{i,4}: {input} -> {next} : {price} : {delta}");
        deltas[0]=deltas[1];
        deltas[1]=deltas[2];
        deltas[2]=deltas[3];
        deltas[3]=delta;
        if (deltas.SequenceEqual(targetDeltas))
        {
            //Console.WriteLine($"Found at {i}");
            return price;
        }
        input=next;
    }
    return 0;
}


int TransformLoop(int input, int count)
{
    for (int i = 0; i < count; i++)
    {
        input=Transform(input);
    }
    return input;
}


int Transform(int input)
{
    int work=input<<6;
    work^=input;
    work=input=work&0x00FFFFFF;
    work>>=5;
    work^=input;
    work=input=work&0x00FFFFFF;
    work<<=11;
    work^=input;
    work=input=work&0x00FFFFFF;
    return work;
}

