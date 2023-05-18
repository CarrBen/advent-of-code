module day24

type Valley = {width:int; height:int}
type Position = {x:int; y:int}
type Positions = Position array
type Direction = Up | Right | Down | Left
type Blizzard = {x:int; y:int; dir:Direction}
type Blizzards = Blizzard array

let charToDir(c: char): Direction =
    match c with
        | '>' -> Right
        | 'v' -> Down
        | '<' -> Left
        | '^' -> Up
        
let dirToDelta(d: Direction) =
    match d with
        | Up -> (0, -1)
        | Left -> (-1, 0)
        | Right -> (1, 0)
        | Down -> (0, 1)

let parse(input: string): Valley * Blizzards =
    let lines = input.Split('\n') |> Array.filter (fun line -> line.Length > 0)
    let width = lines[0].Length - 2
    let height = lines.Length - 2
    
    let blizzards = lines |> 
        Seq.indexed |> 
        Seq.map (fun (y, line) -> line |> 
            Seq.indexed |>
            Seq.map (fun (x, c) -> if c = '#' || c = '.' then None else Some({x=x; y=y; dir=charToDir(c)})) |>
            Seq.filter (fun i -> i.IsSome) |>
            Seq.map (fun i -> i.Value)) |>
        Seq.concat |>
        Seq.toArray
    
    ({width=width; height=height}, blizzards)
    
let validBlizzard(valley: Valley, blizzard: Blizzard): bool =
    blizzard.x >= 1 && blizzard.y >= 1&& blizzard.x <= valley.width && blizzard.y <= valley.height
    
let wrapBlizzard(valley: Valley, blizzard: Blizzard): Blizzard =
    if blizzard.x < 1 then {blizzard with x=blizzard.x + valley.width} else
    if blizzard.y < 1 then {blizzard with y=blizzard.y + valley.height} else
    if blizzard.x > valley.width then {blizzard with x=blizzard.x - valley.width} else
    if blizzard.y > valley.height then {blizzard with y=blizzard.y - valley.height} else
    blizzard
    
let stepBlizzard(valley: Valley) (blizzard: Blizzard): Blizzard =
    let dx, dy = dirToDelta(blizzard.dir)
    let newBlizzard = {x=blizzard.x + dx; y=blizzard.y + dy; dir=blizzard.dir}
    if validBlizzard(valley, newBlizzard) then newBlizzard else
    wrapBlizzard(valley, newBlizzard)
    
let stepBlizzards(valley: Valley, blizzards: Blizzards): Blizzards =
    blizzards |> Array.map (stepBlizzard valley)
    
let stepParty(valley: Valley, blizzards: Blizzards) (pos: Position): Positions =
    let localBlizzards = blizzards |> Array.filter (fun blizz -> blizz.x >= (pos.x-1) && blizz.x <= (pos.x+1) && blizz.y <= (pos.y+1) && blizz.y >= (pos.y-1))
    let deltas = [| (0,0); (-1,0); (1,0); (0,-1); (0,1) |]
    deltas |>
        Array.map (fun (dx, dy) -> (pos.x+dx, pos.y+dy)) |>
        Array.filter (fun (x, y) -> ((x=1 && y=0) || (x = valley.width && y = (valley.height + 1)) || x > 0 && y > 0 && x < (valley.width+1) && y < (valley.height+1))) |>
        Array.filter (fun (x, y) -> (localBlizzards |> Array.tryFind (fun blizz -> blizz.x = x && blizz.y = y)).IsNone) |>
        Array.map (fun (x, y) -> {x=x; y=y})   

let isFinished(valley: Valley) (pos: Position): bool =
    pos.y = (valley.height + 1) && pos.x = valley.width
    
let rec step(valley: Valley, blizzards: Blizzards, positions: Positions, s: int): int =
    let nextBlizzards = stepBlizzards(valley, blizzards)
    let nextPositions = positions |> Array.map (stepParty (valley, nextBlizzards)) |> Array.concat |> Array.distinct
    let finishedPositions = nextPositions |> Array.filter (isFinished valley)
    if finishedPositions.Length > 0 then s else
    step(valley, nextBlizzards, nextPositions, s+1)

let part1 (input : string) =
    let valley, blizzards = parse(input)
    let x, y = 1, 0
    step(valley, blizzards, [| {x=x; y=y} |], 1)
    
let part2 (input : string) =
    0