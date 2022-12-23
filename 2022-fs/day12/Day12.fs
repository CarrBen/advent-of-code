module day12

type Grid = int list list
type Coord = int * int
type Map = { grid: Grid; low: Coord; high: Coord }

let parse (input: string): Map =
    let lines = input.Split('\n') |> Seq.filter (fun line -> line.Length > 0)
    let width = input.IndexOf('\n')
    let startIndex = lines |> Seq.concat |> Seq.findIndex (fun c-> c= 'S')
    let endIndex = lines |> Seq.concat |> Seq.findIndex (fun c-> c= 'E')
    
    let grid = lines |> Seq.map (fun line -> line |> Seq.map (fun c -> if c = 'S' then int('a') else if c = 'E' then int('z') else int(c)) |> Seq.map (fun c-> c - 97) |> List.ofSeq) |> List.ofSeq

    { 
        Map.grid = grid
        low = (startIndex % width, startIndex / width);
        high = (endIndex % width, endIndex / width);
    }
    
let validMoveBack (map: Map, (sx: int, sy: int), (ex: int, ey: int)): bool =
    ex >= 0 && ex < map.grid[0].Length && ey >= 0 && ey < map.grid.Length &&
    map.grid[ey][ex] >= map.grid[sy][sx]-1    
    
let validMoveForward (map: Map, (sx: int, sy: int), (ex: int, ey: int)): bool =
    ex >= 0 && ex < map.grid[0].Length && ey >= 0 && ey < map.grid.Length &&
    map.grid[ey][ex] <= map.grid[sy][sx]+1

let update (scores: Grid, coord: Coord, value: int): Grid =
    let cx, cy = coord
    scores 
    |> Seq.indexed 
    |> Seq.map (fun (y, line) -> line |> Seq.indexed |> Seq.map (fun (x, item) -> if y = cy && x = cx then value else item) |> List.ofSeq)
    |> List.ofSeq
    
let rec score (map: Map, scores: Grid, queue: Coord list): Grid =
    if queue.IsEmpty then scores else
    let (cx, cy)::rest = queue
    let directions = [(1,0); (-1,0); (0,1); (0,-1)]
    if (cx, cy) = map.low then score(map, scores, directions |> Seq.map (fun (dx, dy) -> (cx + dx, cy + dy)) |> Seq.filter (fun (x, y) -> validMoveForward(map, (cx, cy), (x, y))) |> List.ofSeq) else
    let lowest = directions |> Seq.map (fun (dx, dy) -> if validMoveBack(map, (cx, cy), (cx + dx, cy + dy)) then scores[cy + dy][cx + dx] else System.Int32.MaxValue) |> Seq.min
    //printfn "<<< %A %A" (cx, cy) lowest
    if lowest <> System.Int32.MaxValue then
        let newScores = if lowest + 1 < scores[cy][cx] then update(scores, (cx, cy), lowest + 1) else scores
        let newQueue = Seq.append rest (directions |> Seq.map (fun (dx, dy) -> (cx + dx, cy + dy)) |> Seq.filter (fun (x, y) -> (validMoveForward(map, (cx, cy), (x, y)) && (rest |> Seq.tryFind (fun (rx, ry) -> rx = x && ry = y)).IsNone && newScores[y][x] = System.Int32.MaxValue) || (validMoveBack(map, (cx, cy), (x, y)) && (rest |> Seq.tryFind (fun (rx, ry) -> rx = x && ry = y)).IsNone && newScores[y][x] > newScores[cy][cx]+1))) |> List.ofSeq
        //newScores |> Seq.map (fun line -> printfn "%A" line) |> List.ofSeq
        //printfn "--- %A %A" (cx, cy) newQueue
        score(map, newScores, newQueue)
    else
    if rest.IsEmpty then scores else score(map, scores, rest)

let part1 (input : string) =
    let map = parse(input)
    let empty_scores = System.Int32.MaxValue |> Seq.replicate map.grid[0].Length |> List.ofSeq |> Seq.replicate map.grid.Length |> List.ofSeq
    let start_scores = update(empty_scores, map.low, 0)
    let scores = score(map, start_scores, [map.low])
    let ex, ey = map.high
    //scores |> Seq.map (fun line -> printfn "%A" line) |> List.ofSeq
    scores[ey][ex]
    
let part2 (input : string) =
    let map = parse(input)
    let empty_scores = System.Int32.MaxValue |> Seq.replicate map.grid[0].Length |> List.ofSeq |> Seq.replicate map.grid.Length |> List.ofSeq
    let starting_points = map.grid |> Seq.indexed |> Seq.map (fun (y, line) -> line |> Seq.indexed |> Seq.filter (fun (x, v) -> v = 0) |> Seq.map (fun (x, v) -> (x, y))) |> Seq.concat
    let start_scores = (empty_scores, starting_points) ||> Seq.fold (fun s point -> update(s, point, 0))
    let scores = score(map, start_scores, [map.low])
    let ex, ey = map.high
    //scores |> Seq.map (fun line -> printfn "%A" line) |> List.ofSeq
    scores[ey][ex]