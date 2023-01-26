module day22

type Coord = int * int
type Tile = Empty | Wall | Redirect of (Coord option) * (Coord option)
type Grid = Tile[][]
type Direction = Up | Down | Left | Right
type Turn = Left | Right
type Move = Turn of Turn | Distance of int

let parseEmpty(map: string[], (x: int, y: int)): Tile =
    let rightX = (x + 1) % map[0].Length
    let leftX = if x > 0 then x - 1 else map[0].Length - 1
    let horizontalWall = if map[y][rightX] <> '.' && map[y][leftX] <> '.' then None else if map[y][rightX] = '.' then map[y] |> Seq.tryFindIndexBack (fun c -> c = '#') else map[y] |> Seq.tryFindIndex (fun c -> c = '#')
    let horizontalEmpty = if map[y][rightX] <> '.' && map[y][leftX] <> '.' then None else if map[y][rightX] = '.' then map[y] |> Seq.tryFindIndexBack (fun c -> c = '.') else map[y] |> Seq.tryFindIndex (fun c -> c = '.')
    let horizontal = if map[y][rightX] <> '.' && map[y][leftX] <> '.' then None else if map[y][rightX] = '.' then (if horizontalWall.IsNone || horizontalWall.Value < horizontalEmpty.Value then Some(Coord (horizontalEmpty.Value, y)) else None ) else (if horizontalWall.IsNone || horizontalWall.Value > horizontalEmpty.Value then Some(Coord (horizontalEmpty.Value, y)) else None )
    
    let upY = if y > 0 then y - 1 else map.Length - 1
    let downY = (y + 1) % map.Length
    //let vertical = if map[upY][x] <> '.' && map[downY][x] <> '.' then None else if map[downY][x] = '.' then Some(Coord (x, (map |> Seq.findIndexBack (fun line -> line[x] = '.')))) else Some(Coord (x, (map |> Seq.findIndex (fun line -> line[x] = '.'))))
    let verticalWall = if map[upY][x] <> '.' && map[downY][x] <> '.' then None else if map[downY][x] = '.' then map |> Seq.tryFindIndexBack (fun line -> line[x] = '#') else map |> Seq.tryFindIndex (fun line -> line[x] = '#')
    let verticalEmpty = if map[upY][x] <> '.' && map[downY][x] <> '.' then None else if map[downY][x] = '.' then map |> Seq.tryFindIndexBack (fun line -> line[x] = '.') else map |> Seq.tryFindIndex (fun line -> line[x] = '.')
    let vertical = if map[upY][x] <> '.' && map[downY][x] <> '.' then None else if map[downY][x] = '.' then (if verticalWall.IsNone || verticalWall.Value < verticalEmpty.Value then Some(Coord (x, verticalEmpty.Value)) else None ) else (if verticalWall.IsNone || verticalWall.Value > verticalEmpty.Value then Some(Coord (x, verticalEmpty.Value)) else None )
    
    if horizontal.IsNone && vertical.IsNone then Tile.Redirect (None, None) else
    Tile.Redirect (horizontal, vertical)

let parseLines(input: string): string[] * string =
    let lines = input.Split('\n') |> Array.filter (fun line -> line.Length > 0)
    let rawMap = lines[.. lines.Length - 2]
    let mapWidest = rawMap |> Seq.map (fun line -> line.Length) |> Seq.max
    let map = rawMap |> Array.map (fun line -> String.concat "" [line; (String.replicate (mapWidest - line.Length) " ")]) |> Array.ofSeq
    (map, lines |> Array.last)

let parse(input: string): Grid * Move list =
    let map, instructions = parseLines(input)
    
    let distances = instructions.Split([|'R'; 'L'|]) |> Array.map int |> Array.map Move.Distance
    let turns = instructions |> Seq.filter (fun c -> c = 'L' || c = 'R') |> Seq.map (fun c -> Move.Turn (if c = 'L' then Turn.Left else Turn.Right)) |> Array.ofSeq
    let lastDistance = distances |> Array.last
    let moves = List.append (Seq.fold2 (fun state a b -> List.append state [a; b]) [] distances turns) [lastDistance]
    
    let grid = map |> Array.indexed |> Array.map (fun (y, line) -> line |> Seq.indexed |> Seq.map (fun (x, c) -> match c with | '.' -> Tile.Empty | '#' -> Tile.Wall | ' ' -> parseEmpty(map, (x, y))) |> Array.ofSeq)
    
    (grid, moves)
    
let convertDir(input: Direction): int =
    match input with
        | Direction.Right -> 0
        | Direction.Down -> 1
        | Direction.Left -> 2
        | Direction.Up -> 3
        
let applyTurn(dir: Direction, turn: Turn): Direction =
    match dir, turn with
        | Direction.Right, Turn.Right -> Direction.Down
        | Direction.Down, Turn.Right -> Direction.Left
        | Direction.Left, Turn.Right -> Direction.Up
        | Direction.Up, Turn.Right -> Direction.Right
        | Direction.Right, Turn.Left -> Direction.Up
        | Direction.Down, Turn.Left -> Direction.Right
        | Direction.Left, Turn.Left -> Direction.Down
        | Direction.Up, Turn.Left -> Direction.Left
        
let applyMove(map: Grid, d: int, (x: int, y: int), dir: Direction): (int * int) * Direction =
    let dx, dy = match dir with
                                | Direction.Left -> (-1, 0)
                                | Direction.Right -> (1, 0)
                                | Direction.Up -> (0, -1)
                                | Direction.Down -> (0, 1)
        
    let mutable cx, cy = x, y
    let mutable nx, ny = x, y
    for i in { 1 .. d } do
        nx <- cx + dx
        nx <- if nx < 0 then map[0].Length - 1 else nx
        nx <- if nx >= map[0].Length then 0 else nx
        
        ny <- cy + dy
        ny <- if ny < 0 then map.Length - 1 else ny
        ny <- if ny >= map.Length then 0 else ny
                
        // let nnx_neg = if ny >= 0 && ny < map.Length then map[ny] |> Seq.tryFindIndexBack (fun t -> match t with | Tile.Redirect (None, None) -> false | Tile.Redirect (_, _) -> true | _ -> false) else None
        // let nnx_pos = if ny >= 0 && ny < map.Length then map[ny] |> Seq.tryFindIndex (fun t -> match t with | Tile.Redirect (None, None) -> false | Tile.Redirect (_, _) -> true | _ -> false) else None
        // let nny_neg = if nx >= 0 && nx < map[0].Length then map |> Seq.tryFindIndexBack (fun row -> match row[nx] with | Tile.Redirect (None, None) -> false | Tile.Redirect (_, _) -> true | _ -> false) else None
        // if nny_neg.IsSome then printfn "nny_neg found %A %A" nny_neg.Value (map[nny_neg.Value][nx]) else ()
        // let nny_pos = if nx >= 0 && nx < map[0].Length then map |> Seq.tryFindIndex (fun row -> match row[nx] with | Tile.Redirect (None, None) -> false | Tile.Redirect (_, _) -> true | _ -> false) else None
        // let curX, curY = if nx < 0 then (if nnx_neg.IsSome then (nnx_neg.Value, ny) else (cx, ny)) else
        //                             if nx >= map[cy].Length then (if nnx_pos.IsSome then (nnx_pos.Value, ny) else (cx, ny)) else
        //                             if ny < 0 then (if nny_neg.IsSome then (nx, nny_neg.Value) else (nx, cy)) else
        //                             if ny >= map.Length then (if nny_pos.IsSome then (nx, nny_pos.Value) else (nx, cy)) else
        //                             match map[ny][nx] with
        //                             | Tile.Empty -> (nx, ny)
        //                             | Tile.Wall -> (cx, cy)
        //                             | Tile.Redirect (h, v) -> if v.IsNone && h.IsNone then (cx, cy) else if dx = 0 && v.IsSome then v.Value else if dy = 0 && h.IsSome then h.Value else (cx, cy)
        
        let curX, curY = match map[ny][nx] with
                                    | Tile.Empty -> (nx, ny)
                                    | Tile.Wall -> (cx, cy)
                                    | Tile.Redirect (h, v) -> if v.IsNone && h.IsNone then (cx, cy) else if dx = 0 && v.IsSome then v.Value else if dy = 0 && h.IsSome then h.Value else (cx, cy)
        cx <- curX
        cy <- curY
        //printfn "Moved %A to %A" dir (cx, cy)
    done
        
    //printfn "Started %A did %A %A ended %A" (x, y) d dir (cx, cy)
    (cx, cy), dir
        
let rec applyMoves(map: Grid, moves: Move list, ((x: int, y: int), dir: Direction)): (int * int) * Direction =
    let move::rest = moves
    
    let (newX, newY), newDir = match move with
                                                        | Distance d -> applyMove(map, d, (x, y), dir)
                                                        | Turn t -> (x, y), applyTurn(dir, t)
    
    if rest.Length > 0 then
        applyMoves(map, rest, ((newX, newY), newDir))
    else
        ((newX, newY), newDir)

let part1 (input : string) =
    let map, moves = parse(input)
    printfn "Moves %A" moves
    let grid, _ = parseLines(input)
    let start = (((grid[0] |> Seq.findIndex (fun c -> c = '.')), 0), Direction.Right)
    let (endX, endY), endDir = applyMoves(map, moves, start)
    (1000 * (endY + 1)) + (4 * (endX + 1)) + convertDir(endDir)
    
let part2 (input : string) =
    0