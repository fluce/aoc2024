var s=await AOC.GetData().AsLines();

var pairs=s.Select(x=>x.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray()).Where(x=>x.Length==2).ToArray();

var list1=pairs.Select(x=>x[0]).Order();
var list2=pairs.Select(x=>x[1]).Order();

var sumOfDifferences=list1.Zip(list2).Select(x=>Math.Abs(x.First-x.Second)).Sum();

Console.WriteLine(sumOfDifferences);

var similarity=list1.Sum(x=>x*list2.TakeWhile(y=>y<=x).Count(y=>y==x));

Console.WriteLine(similarity);