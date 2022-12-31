module day16

type Node = { name: string; connections: (string * int) list; flow: int }
type State = { flowing: string list; relief: int; remaining: int }

let parseNode (input: string): Node =
    let parts = input.Split(' ')
    let name = parts[1];
    let flow = int(parts[4].Substring(5).Trim(';'))
    let tunnels = parts[9..] |> Seq.map (fun t -> t.Trim(','), 1) |> List.ofSeq
    { Node.name = name; flow = flow; connections = tunnels }
    
let parseNodes (input: string): Map<string,Node> =
    input.Split('\n') |> Seq.filter (fun line -> line.Length > 0) |> Seq.map parseNode |> Seq.map (fun n -> n.name, n) |> Map
    
let newConnection(zeroNodes: Node seq, node: Node) (n: string): (string * int) seq =
    let _, oldConnectionDist = node.connections |> Seq.find (fun (name, d) -> name = n)
    let zeroNode = zeroNodes |> Seq.find (fun zn -> zn.name = n)
    zeroNode.connections |> Seq.filter (fun (zn, zd) -> not(zn = node.name)) |> Seq.map (fun (znn, znd) -> (znn, oldConnectionDist+znd))
    
let rec pruneNode (zeroNodes: Node seq, alreadyRemoved: string seq) (node: Node): Node =
    let zeroNames = zeroNodes |> Seq.map (fun n -> n.name)
    let removedNames = node.connections |> Seq.map fst |> Seq.filter (fun n -> zeroNames |> Seq.contains n)
    if removedNames |> Seq.length = 0 then node else
    let remainingNodes = node.connections |> Seq.filter (fun (name, dist) -> not(removedNames |> Seq.contains name))
    let newNodes = removedNames |> Seq.map (newConnection(zeroNodes, node)) |> Seq.concat |> Seq.filter (fun (n, d) -> not(alreadyRemoved |> Seq.contains n))
    let newConnections = (Seq.append remainingNodes newNodes |> List.ofSeq)
    let newNode = { node with connections=newConnections }
    if (newConnections |> Seq.filter (fun (name, dist) -> zeroNames |> Seq.contains name) |> Seq.length) > 0 then
    pruneNode(zeroNodes, Seq.append alreadyRemoved removedNames)(newNode)
    else
    newNode
    
let pruneNodes (input: Map<string,Node>): Map<string,Node> =
    let zeroNodes = input.Values |> Seq.filter (fun n -> n.flow = 0) |> Seq.filter (fun n -> not(n.name = "AA"))
    input.Values |> Seq.except zeroNodes |> Seq.map (pruneNode (zeroNodes, Seq.empty)) |> Seq.map (fun n -> (n.name, n)) |> Map

let findNode: string -> Node -> bool =
    fun name node ->
        node.name = name
        
let rec permutations (input: 'a seq): 'a list seq =
    if input |> Seq.length = 1 then seq { input |> List.ofSeq } else
    input |> Seq.indexed |> Seq.collect (fun (index, item) -> input |> Seq.removeAt index |> permutations |> Seq.map (fun list -> List.append [item] list))
    
let rec nodeDist (nodes: Map<string,Node>, a: Node, b: Node, visited: string list): int =
    if a.name = b.name then 0 else
    if a.connections |> Seq.map (fun (c, d) -> c) |> Seq.except visited |> Seq.length = 0 then System.Int32.MaxValue/2 else
    let directConnections = a.connections |> Seq.filter (fun (c, d) -> c = b.name)
    let indirectConnections = a.connections |> Seq.filter (fun (c, d) -> not(visited |> Seq.contains c)) |> Seq.map (fun (c, d) -> (c, nodeDist(nodes, nodes[c], b, List.append visited [a.name]) + d))
    //printfn "%A -> %A\n Connections %A\n" a.name b.name ((Seq.append directConnections indirectConnections) |> List.ofSeq)
    let bestC, bestD = (Seq.append directConnections indirectConnections) |> Seq.minBy (fun (c, d) -> d)
    bestD
        
let mutable permuteBFSCount = 0
let mutable permuteDFSCount = 0

let openValve (map: Map<string,Node>, start: Node, dest: Node, oldState: State): State =
    let dist = nodeDist(map, start, dest, [])
    { flowing=(List.append oldState.flowing [dest.name]); remaining=(oldState.remaining - dist - 1); relief=oldState.relief + ((oldState.remaining - dist - 1) * dest.flow) }
        
let rec permuteBFS (map: Map<string,Node>, input: Node seq, current: string, state: State): State =
    permuteBFSCount <- permuteBFSCount + 1
    if input |> Seq.length = 0 then state else
    let futureInputs = input |> Seq.indexed |> Seq.map (fun (index, value) -> (value, input |> Seq.removeAt index))
    futureInputs |> Seq.map (fun (newCurrent, newInput) -> permuteBFS(map, newInput, newCurrent.name, openValve(map, map[current], newCurrent, state))) |> Seq.maxBy (fun s-> s.relief)

let rec permuteDFS (map: Map<string,Node>, input: Node seq, dfs: int list, current: string, state: State, target: int): State =
    permuteDFSCount <- permuteDFSCount + 1
    //printfn "%A %A %A\n" permuteDFSCount dfs state
    if state.remaining <= 0 then state else
    if dfs.Length = 0 then state else
    let newCurrentIndex::newDfs = dfs
    let newCurrent = input |> Seq.item newCurrentIndex
    let newInput = input |> Seq.removeAt newCurrentIndex
    let dist = nodeDist(map, map[current], newCurrent, [])
    let newRemaining = (state.remaining - dist - 1);
    if newRemaining < 0 then state else
    let newState = { State.remaining=newRemaining; flowing=(List.append state.flowing [newCurrent.name]); relief=state.relief + (newRemaining * newCurrent.flow) }
    let _, projection, _ = ((newState.remaining, newState.relief, newCurrent), newInput |> Seq.sortByDescending (fun n -> n.flow)) ||> Seq.fold (fun (rem, rel, nod) (input: Node) -> (rem-2, rel + (rem-2)*input.flow, input))
    if projection < target then state else
    permuteDFS(map, newInput, newDfs, newCurrent.name, newState, target)
    
let incrementDFS (dfs: int list, limit: int): int list =       
    (true, (dfs |> List.indexed |> Seq.rev))
    ||> Seq.mapFold (fun carry (index, value) -> ((index, if carry then ((value + 1) % (limit - index)) else value)), (if carry then ((value + 1) >= (limit - index)) else false)) 
    |> fst |> Seq.rev |> Seq.map snd |> List.ofSeq
    
let runDFS (map: Map<string,Node>, input: Node seq, current: string, state: State): State =
    let inputLen = (input |> Seq.length)
    let startDfs = 0 |> List.replicate inputLen
    
    let mutable bestState = permuteDFS(map, input, startDfs, current, state, 0)
    let mutable currentDfs = incrementDFS(startDfs, inputLen)
    while currentDfs |> Seq.sum > 0 do
        let newState = permuteDFS(map, input, currentDfs, current, state, bestState.relief)
        bestState <- if newState.relief > bestState.relief then newState else bestState
        currentDfs <- incrementDFS(currentDfs, inputLen)
    bestState

let part1_slow (input : string) =
    let nodes = parseNodes(input)
    let prunedNodes = pruneNodes(nodes)
    let nonStartNodes = nodes |> Map.filter (fun k v -> not(k = "AA"))
    let nonStartPrunedNodes = prunedNodes |> Map.filter (fun k v -> not(k = "AA"))
    
    let theNodes = prunedNodes
    let theNonStartNodes = nonStartPrunedNodes
    
    printfn "Node Counts %A/%A/%A" (nodes |> Seq.length) (prunedNodes |> Seq.length) (nonStartPrunedNodes |> Seq.length)
    
    let outputDFS = runDFS(theNodes, theNonStartNodes.Values, "AA", { State.flowing = []; relief=0; remaining=30 })
    printfn "Permute DFS Call Count %A" permuteDFSCount
    printfn "DFS Output %A" outputDFS.relief
    outputDFS.relief - 1
    
    // let outputBFS = permuteBFS(theNodes, theNonStartNodes.Values, "AA", { State.flowing = []; relief=0; remaining=30 })
    // printfn "Permute BFS Call Count %A" permuteBFSCount
    // outputBFS.relief
    
// let rec dfs (map: Map<string,Node>, current: Node, visited: string list, state: State): State =
//     let candidates = current.connections |> List.filter (fun (conn, dist) -> not(visited |> Seq.contains conn))
//     if (candidates |> Seq.length = 0) then printfn "No Candidates. Returning."; state else
    
//     let projections = candidates |>
//         List.map (fun (conn, dist) -> (map[conn], { State.flowing=List.append state.flowing [conn]; relief=state.relief + (state.remaining - nodeDist(map, current, map[conn], []) - 1 ) * map[conn].flow; remaining=state.remaining - nodeDist(map, current, map[conn], []) - 1 }))
//         |> List.filter (fun (n, s) -> s.remaining >= 0)
    
//     if (projections |> Seq.length = 0) then printfn "No Projections. Returning."; state else
    
//     let bestNode, bestProjection = projections |> Seq.maxBy (fun (n, s) -> s.relief)
//     let newVisited = List.append visited [bestNode.name]
    
//     printfn "Best %A" bestNode.name
    
//     dfs(map, bestNode, newVisited, bestProjection)

let foldRelief (map: Map<string, Node>) (distMap: Map<string, Map<string, int>>): int * int * string -> string -> int * int * string =
    fun (relief, remaining, current) candidate ->
        let dist = distMap[current][candidate]
        if remaining - dist <= 2 then
            (relief, 0, current)
        else
        
        let newRemaining = remaining - dist - 1
        let newRelief = relief + (newRemaining * map[candidate].flow)
        (newRelief, newRemaining, candidate)

let computeRelief (map: Map<string, Node>)  (distMap: Map<string, Map<string, int>>) (current: string) (candidates: string list) (remaining: int): int =
    let relief, _, _ = ((0, remaining, current), candidates) ||> Seq.fold (foldRelief map distMap)
    relief

let mutable DFSCount = 0
let mutable cacheAttempt = 0
let mutable cacheHit = 0
let mutable cacheMiss = 0
    
let rec dfs (map: Map<string,Node>, distMap: Map<string,Map<string,int>>, current: Node, state: State, parentBestState: State, cache: Map<string, string list>): State * Map<string, string list> =
    DFSCount <- DFSCount + 1
    if DFSCount % 100000 = 0 then printfn "DFS Count %A" DFSCount else ()
    
    if state.relief < parentBestState.relief && state.remaining < parentBestState.remaining then (state, cache) else
    let candidates = map.Values |> Seq.filter (fun node -> not(state.flowing |> Seq.contains node.name)) |> Array.ofSeq
    if (candidates |> Array.isEmpty) then (state, cache) else
    
    let mutable bestState = parentBestState
    let mutable updatedCached = cache
    for node in candidates do
        //let newRemaining = state.remaining - nodeDist(map, current, node, []) - 1
        let newRemaining = state.remaining - distMap[current.name][node.name] - 1
        let newState = { State.flowing=List.append state.flowing [node.name]; relief=state.relief + (newRemaining * node.flow); remaining=newRemaining }
        
        let cacheKey = (Seq.append [node.name] (candidates |> Seq.except [node] |> Seq.map (fun n -> n.name) |> Seq.sort)) |> String.concat ""
        let cacheResult = updatedCached.TryFind cacheKey
        cacheAttempt <- cacheAttempt + 1
        
        if cacheResult.IsNone then
            cacheMiss <- cacheMiss + 1
            let output, newCache = if newRemaining >= 0 then dfs(map, distMap, node, newState, bestState, updatedCached) else (state, updatedCached)
            let candidateOrder = output.flowing |> Seq.except newState.flowing |> List.ofSeq
            
            updatedCached <- newCache            
            updatedCached <- if output.relief > bestState.relief then Map.add cacheKey candidateOrder updatedCached else updatedCached
            bestState <- if output.relief > bestState.relief then output else bestState
        else
            cacheHit <- cacheHit + 1
            let cachedOrder = cacheResult.Value
            let computedRelief = computeRelief map distMap node.name cachedOrder newState.remaining
            let newCacheState = { flowing=(List.append newState.flowing (candidates |> Seq.except [node] |> Seq.map (fun n -> n.name) |> List.ofSeq)); relief=newState.relief + computedRelief; remaining=0 }
            bestState <- if newCacheState.relief > bestState.relief then newCacheState else bestState
        
    (bestState, updatedCached)
    
let precomputeDistances (map: Map<string,Node>): Map<string,Map<string,int>> =
    map.Values |> Seq.map (fun nodeA -> (nodeA.name, (map.Values |> Seq.map (fun nodeB -> (nodeB.name, nodeDist(map, nodeA, nodeB, []))) |> Map ))) |> Map
    
let part1 (input : string) =
    let nodes = parseNodes(input)
    let prunedNodes = pruneNodes(nodes)
    let nonStartNodes = nodes |> Map.filter (fun k v -> not(k = "AA"))
    let nonStartPrunedNodes = prunedNodes |> Map.filter (fun k v -> not(k = "AA"))
    
    let usePruned = true
    let theNodes = if usePruned then prunedNodes else nodes
    let theNonStartNodes = if usePruned then nonStartPrunedNodes else nonStartNodes
    
    let distMap = precomputeDistances(theNodes)
    
    let t = System.Diagnostics.Stopwatch.StartNew()
    let output, _ = dfs(theNodes, distMap, theNodes["AA"], { State.flowing = ["AA"]; relief=0; remaining=30 }, { State.flowing = []; relief=0; remaining=0 }, new Map<string, string list>([]))
    printfn "DFS Time %A" t.Elapsed.TotalMilliseconds
    printfn "DFS Count %A" DFSCount
    printfn "DFS Cache Hit %A/%A Miss %A/%A" cacheHit cacheAttempt cacheMiss cacheAttempt
    output.relief
    
let part2 (input : string) =
    0