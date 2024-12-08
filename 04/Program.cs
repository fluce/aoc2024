var data=(await AOC.GetData().AsLines()).Select(x=>x.ToArray()).Where(x=>x.Any()).ToArray();

var input="""
MMMSXXMASM
MSAMXMSMSA
AMXSXMAAMM
MSAMASMSMX
XMASAMXAMM
XXAMMXXAMA
SMSMSASXSS
SAXAMASAAA
MAMMMXMMMM
MXMXAXMASX
""";

//data=input.Split("\n").Select(x=>x.Trim().ToArray()).Where(x=>x.Any()).ToArray();

(int dX, int dY)[] dirs = [
    (1,0),
    (0,1),
    (-1,0),
    (0,-1),
    (1,1),
    (-1,1),
    (-1,-1),
    (1,-1)
];

var size=(sX:data[0].Length,sY:data.Length);

bool IsInBounds((int x,int y) pos) => pos.x>=0 && pos.x<size.sX && pos.y>=0 && pos.y<size.sY;

var stringToMatch="XMAS";

int count=0;

var data2=data.Select(x=>new string('.',data[0].Length).ToArray()).ToArray();

foreach(var ix in Enumerable.Range(0,size.sX))
    foreach(var iy in Enumerable.Range(0,size.sY))
        foreach(var dir in dirs)
        {            
            var pos=(x:ix,y:iy);
            bool match=true;
            for(var i=0;i<stringToMatch.Length;i++)
            {
                var c=stringToMatch[i];
                if (!IsInBounds(pos))
                {
                    match=false;
                    break;
                }
                if (data[pos.y][pos.x]!=c)
                {
                    match=false;
                    break;
                }
                pos=(pos.x+dir.dX,pos.y+dir.dY);
            }
            if (match)
            {
                pos=(x:ix,y:iy);
                for(var i=0;i<stringToMatch.Length;i++)
                {
                    data2[pos.y][pos.x]=stringToMatch[i];
                    pos=(pos.x+dir.dX,pos.y+dir.dY);
                }
                count++;
            }
        }

data2.Dump(x=>x.AsString());

Console.WriteLine(count);

var count2=0;
data2=data.Select(x=>new string('.',data[0].Length).ToArray()).ToArray();

foreach(var ix in Enumerable.Range(1,size.sX-2))
    foreach(var iy in Enumerable.Range(1,size.sY-2))
    {
        if (data[iy][ix]=='A') {
            var diag1_A=data[iy-1][ix-1]=='M' && data[iy+1][ix+1]=='S';
            var diag2_A=data[iy-1][ix+1]=='M' && data[iy+1][ix-1]=='S';
            var diag1_B=data[iy-1][ix-1]=='S' && data[iy+1][ix+1]=='M';
            var diag2_B=data[iy-1][ix+1]=='S' && data[iy+1][ix-1]=='M';
            if ((diag1_A || diag1_B) && (diag2_A || diag2_B))
            {
                count2++;
                data2[iy][ix]='A';
                data2[iy-1][ix-1]=data[iy-1][ix-1];
                data2[iy+1][ix+1]=data[iy+1][ix+1];
                data2[iy-1][ix+1]=data[iy-1][ix+1];
                data2[iy+1][ix-1]=data[iy+1][ix-1];
            }
        }
    }

data2.Dump(x=>x.AsString());

Console.WriteLine(count2);
