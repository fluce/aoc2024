using System.Runtime.CompilerServices;

var s=await AOC.GetData().AsLines();

var test="""
##########
#..O..O.O#
#......O.#
#.OO..O.O#
#..O@..O.#
#O#..O...#
#O..O..O.#
#.OO.O.OO#
#....O...#
##########

<vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^
vvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v
><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<
<<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^
^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><
^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^
>^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^
<><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>
^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>
v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^
""";

var test2="""
#######
#...#.#
#.....#
#..OO@#
#..O..#
#.....#
#######

<vv<<^^<<^^
""";

//s=test.Split('\n').Select(x=>x.Trim()).ToArray();

var map=s.TakeWhile(x=>x.Length>0).Select(x=>x.ToArray()).ToArray();
var moves=s.SkipWhile(x=>x.Length>0).Skip(1).SelectMany(x=>x).Select<char,(int dX, int dY)>(x=>x switch {
    '^'=>(0,-1),
    'v'=>(0,1),
    '<'=>(-1,0),
    '>'=>(1,0),
    _=>throw new Exception()
}).ToArray();

map.Dump(x=>x.AsString());
//moves.Dump();

var pos=map.Find(x=>x=='@');
//Console.WriteLine(pos);
foreach(var move in moves)
{   
    //Console.Clear();
    //Console.WriteLine($"Pos={pos} Move={move}");
    var nextpos=(X:pos.X+move.dX,Y:pos.Y+move.dY);
    var next=map[nextpos.Y][nextpos.X];
    switch (next)
    {
        case '#':
            break;
        case '.':
            map[nextpos.Y][nextpos.X]='@';
            map[pos.Y][pos.X]='.';
            pos=nextpos;
            break;
        case 'O':
            int i=1;
            while (true)
            {
                var nextnextpos=(X:nextpos.X+move.dX*i,Y:nextpos.Y+move.dY*i);
                var nextnext=map[nextnextpos.Y][nextnextpos.X];
                if (nextnext=='#') break;
                if (nextnext=='.')
                {
                    map[nextnextpos.Y][nextnextpos.X]='O';
                    map[nextpos.Y][nextpos.X]='@';
                    map[pos.Y][pos.X]='.';
                    pos=nextpos;
                    break;
                }
                i++;
            }
            break;
        default:
            throw new Exception($"Invalid character {next} at {nextpos}");
    }
    //map.Dump(x=>x.AsString());
    //await Task.Delay(1);
}

var sum=map.Enum().Where(x=>x.Item=='O').Sum(x=>x.C.X+x.C.Y*100);
Console.WriteLine(sum);

var map2=s.TakeWhile(x=>x.Length>0).Select(x=>x.SelectMany<char,char>(x=>x switch {
    '#'=>['#','#'],
    '.'=>['.','.'],
    'O'=>['[',']'],
    '@'=>['@','.'],
    _=>throw new Exception()
}).ToArray()).ToArray();

map2.Dump(x=>x.AsString(separator:""));

pos=map2.Find(x=>x=='@');
//Console.WriteLine(pos);
foreach(var move in moves)
{   
    //Console.WriteLine($"Pos: {pos} Move: {move}");
    //Console.Clear();
    //Console.WriteLine($"Pos={pos} Move={move}");
    var nextpos=(X:pos.X+move.dX,Y:pos.Y+move.dY);
    var next=map2[nextpos.Y][nextpos.X];
    switch ((next,move.dX))
    {
        case ('#',_):
            break;
        case ('.',_):
            map2[nextpos.Y][nextpos.X]='@';
            map2[pos.Y][pos.X]='.';
            pos=nextpos;
            break;
        case ('[' or ']',not 0): {
            int i=1;
            while (true)
            {
                var nextnextpos=(X:nextpos.X+move.dX*i,Y:nextpos.Y);
                var nextnext=map2[nextnextpos.Y][nextnextpos.X];
                if (nextnext=='#') break;
                if (nextnext=='.')
                {
                    while (i>0)
                    {
                        map2[nextnextpos.Y][nextnextpos.X]=map2[nextnextpos.Y][nextnextpos.X-move.dX];
                        i--;
                        nextnextpos=(X:nextnextpos.X-move.dX,Y:nextnextpos.Y);
                    }
                    map2[nextpos.Y][nextpos.X]='@';
                    map2[pos.Y][pos.X]='.';
                    pos=nextpos;
                    break;
                }
                i++;
            }
            break;
        }
        case ('[' or ']',0): {
            List<(int X, int Y)> rowOfMovingTiles=[nextpos, (X:nextpos.X+next switch {'['=>1,']'=>-1},Y:nextpos.Y)];
            List<List<(int X, int Y)>> rowsOfMovingTiles=[rowOfMovingTiles];
            bool stop=false;
            while (true)
            {
                var nextRowOfMovingTiles=new List<(int X, int Y)>();
                foreach(var tile in rowOfMovingTiles)
                {
                    var nextTile=(X:tile.X,Y:tile.Y+move.dY);
                    var nextTileChar=map2[nextTile.Y][nextTile.X];
                    switch (nextTileChar)
                    {
                        case '[' or ']':
                            nextRowOfMovingTiles.Add(nextTile);
                            nextRowOfMovingTiles.Add((X:nextTile.X+nextTileChar switch {'['=>1,']'=>-1},Y:nextTile.Y));
                            break;
                        case '#':
                            stop=true;
                            break;
                        case '.':
                            break;
                        default:
                            throw new Exception($"Invalid character {nextTileChar} at {nextTile}");
                    }
                }
                if (stop) break;
                nextRowOfMovingTiles=nextRowOfMovingTiles.Distinct().ToList();
                if (nextRowOfMovingTiles.Count==0) 
                    break;
                rowsOfMovingTiles.Add(nextRowOfMovingTiles);
                rowOfMovingTiles=nextRowOfMovingTiles;
            }
            if (stop) break;

            foreach(var row in rowsOfMovingTiles.Reverse<List<(int X, int Y)>>())
            {
                foreach(var tile in row)
                {
                    map2[tile.Y+move.dY][tile.X]=map2[tile.Y][tile.X];
                    map2[tile.Y][tile.X]='.';
                }
            }
            map2[nextpos.Y][nextpos.X]='@';
            map2[pos.Y][pos.X]='.';
            pos=nextpos;
            break;
        }
        default:
            throw new Exception($"Invalid character {next} at {nextpos}");
    }
//
//    Console.ReadKey();
//    await Task.Delay(1);
}
map2.Dump(x=>x.AsString(separator:""));
var sum2=map2.Enum().Where(x=>x.Item=='[').Sum(x=>x.C.X+x.C.Y*100);
Console.WriteLine(sum2);
