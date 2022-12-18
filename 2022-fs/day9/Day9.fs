module day9

type Direction = Left | Up | Right | Down

let parseDir (input: char): Direction =
    match input with
        | 'R' -> Right
        | 'U' -> Up
        | 'L' -> Left
        | 'D' -> Down
        
let expandMove (dir: Direction, count: int): (Direction * (Direction * int)) option =
    if count <= 0 then None else
    Some(dir, (dir, count - 1))

let parseMoves (input: string): List<Direction> =
    input.Split('\n') 
    |> Seq.filter (fun line -> line.Length > 0) 
    |> Seq.map (fun line -> line[0], line.Split(' ')[1]) 
    |> Seq.map (fun (d, c) -> parseDir(d), int c) 
    |> Seq.map (fun (d, c) -> (d, c) |> Seq.unfold expandMove)
    |> Seq.concat
    |> List.ofSeq
    
let applyMove ((x: int, y: int), dir: Direction): int * int =
    match dir with
        | Up -> (x, y+1)
        | Down -> (x, y-1)
        | Left -> (x-1, y)
        | Right -> (x+1, y)
    
let updateTail ((hx: int, hy: int), (tx: int, ty: int)): int * int =
    if sqrt(float(hx-tx)**2 + float(hy-ty)**2) < 1.5 then (tx, ty) else
    if hx = tx then (tx, ty + sign(hy-ty)) else
    if hy = ty then (tx + sign(hx-tx), ty) else
    (tx + sign(hx-tx), ty + sign(hy-ty))
    
let doMove : ((int * int) * (int * int)) -> Direction -> ((int * int) * (int * int)) =
    fun ((hx, hy), (tx, ty)) dir ->
        let hPos = applyMove((hx, hy), dir)
        let tPos = updateTail(hPos, (tx, ty))
        (hPos, tPos)
        
let updateChain : (int * int) -> (int * int) -> (int * int) =
    fun (hx, hy) (tx: int, ty: int) ->
        updateTail((hx, hy), (tx, ty))
    
let doChain : (int * int) list -> Direction -> (int * int) list =
    fun points dir ->
        (applyMove(points.Head, dir), points.Tail) 
        ||> Seq.scan updateChain
        |> List.ofSeq

let part1 (input : string) =
    let moves = parseMoves(input)
    (((0, 0), (0, 0)), moves) ||> Seq.scan doMove
    |> Seq.map (fun (hPos, tPos) -> tPos)
    |> Seq.distinct
    |> Seq.length
    
let part2 (input : string) =
    let moves = parseMoves(input)
    ((0,0) |> Seq.replicate 10 |> List.ofSeq, moves) ||> Seq.scan doChain
    |> Seq.map (fun (points) -> points |> Seq.last)
    |> Seq.distinct
    |> Seq.length