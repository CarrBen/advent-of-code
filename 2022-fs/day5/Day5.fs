module day5

type Move = { Count: int; From: int; To: int; }

type Crate = char
type Crates = List<List<Crate>>

let parseCrates (input: string): Crates =
    let lines = input.Split('\n') |> Seq.filter (fun l -> Seq.length l > 0)
    let rows = lines |> Seq.filter (fun l -> l |> Seq.contains '[')
    let widest = rows |> Seq.map Seq.length |> Seq.max
    let stacks = [0..widest-1] |> Seq.map (fun i -> rows |> Seq.map (fun r -> r |> Seq.tryItem i |> Option.defaultValue ' '))
    let columns = lines |> Seq.last |> Seq.indexed |> Seq.filter (fun (i, c) -> c <> ' ') |> Seq.map (fun (i, c) -> i)
    let output = columns |> Seq.map (fun i -> stacks |> Seq.item i |> Seq.filter (fun c -> c <> ' ') |> List.ofSeq) |> List.ofSeq
    output
        
let parseMoves (input: string) = 
    let lines = input.Split('\n') |> Seq.filter (fun l -> Seq.length l > 0)
    lines |> Seq.map (fun l -> l.Split(' ')) |> Seq.map (fun p -> { Move.Count = int p[1]; From = (int p[3]) - 1; To = (int p[5]) - 1 }) |> List.ofSeq

let splitInput (input: string) = 
    let sep = input.IndexOf("\n\n")
    let crates = input.[..sep]
    let moves = input.[sep+2..]
    (crates, moves)
    
let expandMoves moves =
    moves |> Seq.map (Seq.unfold (fun m -> if m.Count < 1 then None else Some ({ m with Count = 1}, {m with Count = m.Count - 1}))) |> Seq.concat |> List.ofSeq
    
let runMove (crates: Crates, move: Move): Crates = 
    if move.From = move.To then crates
    else
        let values = crates.Item(move.From) |> Seq.take(move.Count)
        crates |> Seq.indexed 
            |> Seq.map (fun (i, l) -> if (i = move.From) then (i, l |> List.skip(move.Count)) else (i, l))
            |> Seq.map (fun (i, l) -> if (i = move.To) then (i, (l |> Seq.append (seq values) |> List.ofSeq)) else (i, l))
            |> Seq.map (fun (_, l) -> l) |> List.ofSeq
    
let runMoves (crates: Crates, moves: List<Move>): Crates =
    let mutable state = crates
    for move in moves do
        state <- runMove (state, move)

    state

let part1 (input : string) =
    let cratesText, movesText = splitInput input
    let crates = parseCrates cratesText
    let moves = parseMoves movesText |> expandMoves
    runMoves (crates, moves) |> Seq.map List.head |> Array.ofSeq |> System.String
    
let part2 (input : string) =
    let cratesText, movesText = splitInput input
    let crates = parseCrates cratesText
    let moves = parseMoves movesText
    runMoves (crates, moves) |> Seq.map List.head |> Array.ofSeq |> System.String
