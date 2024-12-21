using System.Runtime.CompilerServices;

var s=await AOC.GetData();

var test="2333133121414131402";

//s=test;

var data=s.ToCharArray().Select(x=>(int)(x-'0')).Where(x=>x>=0).ToArray();
var files=data.Index().Where(x=>x.Index%2==0).Select(x=>x.Item).ToArray();
var blanks=data.Index().Where(x=>x.Index%2==1).Select(x=>x.Item).ToArray();

Console.WriteLine($"File count: {files.Length}");
Console.WriteLine($"Files: {files.AsString()}");
Console.WriteLine($"Blanks: {blanks.AsString()}");

var checkSum1=CalcCheckSum1(files,blanks);
Console.WriteLine(checkSum1);
var checkSum2=CalcCheckSum2(data);
Console.WriteLine(checkSum2);

long CalcCheckSum1(int[] inputfiles, int[] inputblanks)
{
    var files=inputfiles.ToArray();
    var blanks=inputblanks.ToArray();
    var checkSum=0L;

    var index=0;
    var fileIndexLeft=0;
    var fileIndexRight=files.Length-1;
    List<int> data=new();
    while (true)
    {
        while (files[fileIndexLeft]>0) {
            data.Add(fileIndexLeft);
            checkSum+=(index++)*(fileIndexLeft);
            files[fileIndexLeft]--;
            //Console.WriteLine($"Left {fileIndexLeft} {files[fileIndexLeft]} - {data.AsString()}");
        }
        if (fileIndexRight<=fileIndexLeft) break;
        while (files[fileIndexRight]==0)
            fileIndexRight--;
        //Console.WriteLine($"Left done, blanks: {blanks[fileIndexLeft]} - right: {files[fileIndexRight]}");
        while (blanks[fileIndexLeft]>0 && files[fileIndexRight]>0)
        {
            data.Add(fileIndexRight);
            checkSum+=(index++)*(fileIndexRight);
            files[fileIndexRight]--;
            blanks[fileIndexLeft]--;
            if (blanks[fileIndexLeft]>0 && files[fileIndexRight]==0)
                fileIndexRight--;
            //Console.WriteLine($"Right {fileIndexRight} {blanks[fileIndexLeft]} {files[fileIndexRight]} - {data.AsString()}");
        }
        //Console.WriteLine(data.AsString());
        fileIndexLeft++;
    }

    return checkSum;
}

long CalcCheckSum2(int[] input)
{
    int id=0;
    var files=new LinkedList<AbstractFile>(
                input.Select<int,AbstractFile>( 
                    (x,i)=>(i%2) switch { 
                                0=>new File{ Id=id++, Length=x }, 
                                1=>new EmptyRoom { Length=x } 
                            }
                )
             );

    var fileToMove=files.Last;
    while(true)
    {
        while (fileToMove!=null && fileToMove.Value is not File)
            fileToMove=fileToMove.Previous;
        if (fileToMove==null) {
            break;
        }
        if (fileToMove.Value is File f)
            Console.WriteLine("FileToMove: "+f.Id);
        else 
            Console.WriteLine("FileToMove: Empty");

        var blankToFill=files.First;
        while (blankToFill!=fileToMove && !(blankToFill.Value is EmptyRoom && blankToFill.Value.Length>=fileToMove.Value.Length))
            blankToFill=blankToFill.Next;

        if (blankToFill==fileToMove || blankToFill==null || blankToFill.Value.Length<fileToMove.Value.Length || blankToFill.Value is not EmptyRoom) {
            fileToMove=fileToMove.Previous;
            continue;
        }

        var oldFileToMove=fileToMove;
        fileToMove=fileToMove.Previous;
        files.AddBefore(blankToFill,oldFileToMove.Value);        
        files.AddBefore(oldFileToMove,new EmptyRoom { Length=oldFileToMove.Value.Length });
        files.Remove(oldFileToMove);
        blankToFill.Value.Length-=oldFileToMove.Value.Length;

        //Console.WriteLine(
        //    string.Join("",files.SelectMany(x=>x switch { File f => Enumerable.Repeat(f.Id.ToString(),f.Length), EmptyRoom e => e.Length==0?[]:Enumerable.Repeat(".",e.Length) }))
        //);


    }

    var checkSum=files.SelectMany(x=>x switch { File f => Enumerable.Repeat(f.Id,f.Length), EmptyRoom e => Enumerable.Repeat(0,e.Length) })
         .Index()
         .Sum(x=>(long)(x.Index*x.Item));
    return checkSum;
}

class AbstractFile
{
    public int Length { get; set; }
}

class File : AbstractFile
{
    public int Id { get; set; }
}

class EmptyRoom : AbstractFile
{
}
