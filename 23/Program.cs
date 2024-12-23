var s=await AOC.GetData().AsLines();

var test="""
kh-tc
qp-kh
de-cg
ka-co
yn-aq
qp-ub
cg-tb
vc-aq
tb-ka
wh-tc
yn-cg
kh-ub
ta-co
de-co
tc-td
tb-wq
wh-td
ta-ka
td-qp
aq-cg
wq-ub
ub-vc
de-ta
wq-aq
wq-vc
wh-yn
ka-de
kh-ta
co-tc
wh-qp
tb-vc
td-yn
""".Trim().Split(['\n','\r'],StringSplitOptions.RemoveEmptyEntries);

//s=test;

var data=s.Where(x=>x.Length>0).Select(x=>x.Split('-')).Select(x=>(a:x[0],b:x[1])).ToArray();

var allNodes=data.SelectMany(x=>new[]{x.a,x.b}).Distinct().ToArray();

var neighbours=new Dictionary<string,HashSet<string>>();

foreach (var item in data)
{
    if (!neighbours.ContainsKey(item.a)) neighbours[item.a]=new();
    if (!neighbours.ContainsKey(item.b)) neighbours[item.b]=new();
    neighbours[item.a].Add(item.b);
    neighbours[item.b].Add(item.a);
}

Console.WriteLine(data.Count());
var chiefNodes=allNodes.Where(x=>x.StartsWith('t'));
HashSet<(string,string,string)> networks=new();
foreach(var chiefNode in chiefNodes)
{
    var adj=neighbours[chiefNode];
    foreach(var adjNode1 in adj)
        foreach(var adjNode2 in adj)
            if (adjNode1!=adjNode2)
            {
                if (neighbours[adjNode1].Contains(adjNode2)) {
                    var network=new[]{chiefNode,adjNode1,adjNode2}.OrderBy(x=>x).ToArray();
                    networks.Add((network[0],network[1],network[2]));
                }                
            }
}
networks.OrderBy(x=>x.Item1).ThenBy(x=>x.Item2).ThenBy(x=>x.Item3).Dump();
Console.WriteLine(networks.Count);




var nodeToNetworks=new Dictionary<string,int>();
var networksToNodes=new Dictionary<int,List<string>>();
var nextNetwork=0;
foreach(var n in data) {
    if (nodeToNetworks.TryGetValue(n.a,out var na)) {
        networksToNodes[na].Add(n.b);
        nodeToNetworks[n.b]=na;
    } else {
        nodeToNetworks[n.a]=nextNetwork;
        networksToNodes[nextNetwork]=new List<string>{n.a,n.b};
        nextNetwork++;
    }
    if (nodeToNetworks.TryGetValue(n.b,out var nb)) {
        networksToNodes[nb].Add(n.a);
        nodeToNetworks[n.a]=nb;
    } else {
        nodeToNetworks[n.b]=nextNetwork;
        networksToNodes[nextNetwork]=new List<string>{n.a,n.b};
        nextNetwork++;
    }
}
var networkCounts=0;
foreach(var n in networksToNodes.Keys) {
    networksToNodes[n]=networksToNodes[n].Distinct().ToList();
}
networksToNodes.Dump(x=>x.Key+" "+x.Value.AsString());
Console.WriteLine(networksToNodes.Where(x=>x.Value.Any(x=>x.StartsWith('t'))).Count());
Console.WriteLine(networksToNodes.Where(x=>x.Value.Any(x=>x.StartsWith('t'))).Max(x=>x.Value.Count));
var largestNetwork=Array.Empty<string>();
var cache=new Dictionary<string,string[]>();

foreach(var network in 
            networksToNodes.Where(x=>x.Value.Any(x=>x.StartsWith('t'))&&x.Value.Count>=3)
                           .OrderByDescending(x=>x.Value.Count))
{
    Console.WriteLine($"{network.Key} {network.Value.Count}");
    var nodes=network.Value.ToArray();
    foreach(var node in nodes) {
        Console.WriteLine($"{node} => {neighbours[node].Count} {neighbours[node].Order().AsString()}");
        var largestfullyConnectedSet=FindLargestConnectedSet(neighbours[node].Append(node).Order().ToArray(),neighbours);
        if (largestfullyConnectedSet.Length>largestNetwork.Length) {
            largestNetwork=largestfullyConnectedSet;
        }
    }
}

Console.WriteLine($"Largest network : {largestNetwork.Length} {largestNetwork.Order().AsString(separator:",")}");

string[] FindLargestConnectedSet(string[] nodes, Dictionary<string,HashSet<string>> neighbours)
{
    var ret=Array.Empty<string>();
    if (nodes.Length==2) {
        if (neighbours[nodes[0]].Contains(nodes[1])) {
            ret=[nodes[0],nodes[1]];
        } 
    } else {
        var key=string.Join(',',nodes);
        if (cache.ContainsKey(key)) return cache[key];
        foreach(var node in nodes) {
            var connectedSet=FindLargestConnectedSet(nodes.Where(x=>x!=node).ToArray(),neighbours);
            if (connectedSet!=null && connectedSet.Length>ret.Length+1) {
                if (connectedSet.All(x=>neighbours[node].Contains(x))) {
                    ret=connectedSet.Append(node).Order().ToArray();
                } else {
                    ret=connectedSet;
                }
            }
        }
        cache[key]=ret;
    }
    //if (ret.Length>12) Console.WriteLine($"> {ret.Length} {ret.Order().AsString()}");
    return ret;
}