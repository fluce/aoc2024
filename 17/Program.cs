var s=await AOC.GetData().AsLines();

var test1="""
Register A: 729
Register B: 0
Register C: 0

Program: 0,1,5,4,3,0
""";

var test2="""
Register A: 2024
Register B: 0
Register C: 0

Program: 0,1,5,4,3,0
""";

var test3="""
Register A: 10
Register B: 0
Register C: 0

Program: 5,0,5,1,5,4
""";

var test4="""
Register A: 0
Register B: 29
Register C: 0

Program: 1,7
""";

var test5="""
Register A: 0
Register B: 2024
Register C: 43690

Program: 4,0
""";

var test6="""
Register A: 0
Register B: 0
Register C: 9

Program: 2,6
""";

var test7="""
Register A: 2024
Register B: 0
Register C: 9

Program: 0,3,5,4,3,0
""";

//s=test7.Split('\n').Select(x=>x.Trim()).ToArray();

var registers=new Dictionary<char,long>();
registers['A']=long.Parse(s[0].Split(':')[1].Trim());
registers['B']=long.Parse(s[1].Split(':')[1].Trim());
registers['C']=long.Parse(s[2].Split(':')[1].Trim());
var programAsString=s[4].Split(": ")[1];
var program=programAsString.Split(',').Select(byte.Parse).ToArray();

var result1=RunProgram(registers,programAsString);
Console.WriteLine($"Result1: {result1}");


void iter(ref long A, ref long B, ref long C, ref long accumulator) 
{
    byte D = (byte)(A & 7);
    C = A >> (D ^ 7);
    B = D ^ C;
    A = A >> 3;
    var _out=(A & 7) ^ (A >> (byte)((A & 7) ^ 7));
    accumulator = (accumulator << 3) | (B & 7);
}

long iter2(long A) => (A & 7) ^ (A >> (byte)((A & 7) ^ 7));

long? runPart2(long A, byte[] program, int depth=0)
{
//    Console.WriteLine($"Trying A: {A} Depth: {depth}");
//    Console.ReadKey();
    for (long i=0;i<8;i++)
    {
        var tentativeA=(A<<3) | i;
        if (tentativeA==A) continue;
        var res=RunProgram3(tentativeA,program);
//        Console.WriteLine($"Tentative A: {tentativeA}");
//        Console.WriteLine($"Result: {res.AsString(separator:",")}");
//        Console.WriteLine($"Program: {program.AsString(separator:",")}");

        if (res.SequenceEqual(program.TakeLast(res.Length)))
        {
            if (res.Length==program.Length)
                return tentativeA;
//            Console.WriteLine($"digit {i}");
            var iterRun=runPart2(tentativeA,program, depth+1);
            if (iterRun.HasValue) {
                Console.WriteLine($"Found digit {iterRun.Value}");
                return iterRun;
            }
        }
    }
    return null;
}

var result2=runPart2(0,program);
var res=RunProgram3(result2.Value,program);
Console.WriteLine($"Program: {program.AsString(separator:",")}");
Console.WriteLine($"Result: {res.AsString(separator:",")}");
Console.WriteLine($"Result2: {result2}");


/*

long _A=long.Parse(s[0].Split(':')[1].Trim());
long _B=0;
long _C=0;
long _accumulator=0;
do
{
    Console.WriteLine($"A: {Convert.ToString(_A,8)} accumulator: {Convert.ToString(_accumulator,8)}");
    iter(ref _A, ref _B, ref _C, ref _accumulator);                    
}
while (_A!=0);
Console.WriteLine($"accumulator: {Convert.ToString(_accumulator,8)}");


var list = program.Split(",").Select(x=>byte.Parse(x)).ToArray();
long goodA=0;
var l=list.Length;
for (int i=(int)(l-1);i>=0;i--) {
    Console.WriteLine($"Looking for digit {list.AsString(separator:",")} {i}:{list[i]}");
    Console.ReadKey();
    bool found=false;
    long goodDigit=8;
    goodA<<=3;
    for(long digit=7;digit>=0;digit--)
    {
        long tentativeA=goodA | digit;
        Console.WriteLine($"""
                           {i} {digit} 
                           A : {Convert.ToString(goodA,8).PadLeft(15,'0')} 
                           AS: {Convert.ToString(goodA<<3,8).PadLeft(15,'0')} 
                           D : {Convert.ToString(digit,8).PadLeft(15,'0')}
                           TA: {Convert.ToString(tentativeA,8).PadLeft(15,'0')}
                           """);
        var _out=iter2(tentativeA);
        Console.WriteLine($"A: {Convert.ToString(tentativeA,8)} out: {Convert.ToString(_out,8)} list[{i}]: {list[i]}");
        if (_out==list[i])
        {
            Console.WriteLine($"GoodA = {Convert.ToString(goodA,8)} ({goodA}) => GoodDigit={digit}");
            Console.ReadKey();
            found=true;
            goodDigit=digit;
            break;
        }
        
    }
    if (found)
        goodA|=goodDigit;
}

registers['A']=goodA;
registers['B']=0;
registers['C']=0;
var result2=RunProgram(registers,program);
Console.WriteLine($"Result2: {result2}");
*/
string RunProgram(Dictionary<char,long> registers, string programAsString)
{
    var program=programAsString.Split(',').Select(byte.Parse).ToArray();
    return RunProgram2(registers,program).AsString(separator:",");
}

byte[] RunProgram3(long A, byte[] program)
{
    var registers=new Dictionary<char,long>();
    registers['A']=A;
    registers['B']=0;
    registers['C']=0;
    return RunProgram2(registers,program);
}


byte[] RunProgram2(Dictionary<char,long> registers, byte[] program)
{
    var ip=0L;

    List<byte> output=new();

    while(ip<program.Length-1)
    {
        var instr=(OpCode)program[ip];
        var operand=program[ip+1];
        var combo=(long operand,bool doNotThrow=false)=>operand switch {
            4=>registers['A'],
            5=>registers['B'],
            6=>registers['C'],
            7=>doNotThrow?7:throw new Exception("Invalid operand"),
            _=>operand
        };

        //Console.WriteLine($"IP: {ip} Instr: {instr}({(int)instr}) Operand: {Convert.ToString(operand,8)} Combo: {Convert.ToString(combo(operand,true),8)} A: {Convert.ToString(registers['A'],8)} B: {Convert.ToString(registers['B'],8)} C: {Convert.ToString(registers['C'],8)}");
        switch(instr)
        {
            case OpCode.adv:
                registers['A']=(long)(registers['A']/Math.Pow(2,combo(operand)));
                ip+=2;
                break;
            case OpCode.bdv:
                registers['B']=(long)(registers['A']/Math.Pow(2,combo(operand)));
                ip+=2;
                break;
            case OpCode.cdv:
                registers['C']=(long)(registers['A']/Math.Pow(2,combo(operand)));
                ip+=2;
                break;
            case OpCode.bxl:
                registers['B']=registers['B'] ^ operand;
                ip+=2;
                break;
            case OpCode.bst:
                registers['B']=combo(operand)%8;
                ip+=2;
                break;
            case OpCode.jnz:
                if (registers['A']!=0) 
                    ip=operand;
                else 
                    ip+=2;
                break;
            case OpCode.bxc:
                registers['B']=registers['B'] ^ registers['C'];
                ip+=2;
                break;
            case OpCode.@out:
                output.Add((byte)(combo(operand)%8));
                ip+=2;
                break;
        }

    }
    return output.ToArray();
}

enum OpCode { adv=0, bxl=1, bst=2, jnz=3, bxc=4, @out=5, bdv=6, cdv=7 }

