using System.Text.RegularExpressions;

var s=await AOC.GetData();

//s="xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))";
//s="xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))";
Regex regex=new Regex(@"(mul\((?<a>\d+),(?<b>\d+)\)|do\(\)|don't\(\))",RegexOptions.Compiled);

var matches=regex.Matches(s);

var total=0L;
var total2=0L;
var enabled=true;
foreach(Match match in matches) {
    if (match.Value.StartsWith("mul")) {
        var v=long.Parse(match.Groups["a"].Value)*long.Parse(match.Groups["b"].Value);
        total+=v;
        if (enabled) {
            total2+=v;
        }
    }
    else {
        if (match.Value.StartsWith("don't")) {
            enabled=false;
        }
        else if (match.Value.StartsWith("do")) {
            enabled=true;
        }
    }
}
    
Console.WriteLine(total);
Console.WriteLine(total2);