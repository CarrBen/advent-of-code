module day3

let scoreItem item =
    //let numItem = item as int
    
    if item >= 'A' && item <= 'Z' then int item - int '@' + 26
    else int item - int '`'


let scoreGroup (lines: string[]): int =
    lines |> Seq.map Set |> Seq.reduce (fun acc line -> Set.intersect acc line) |> Seq.map scoreItem |> Seq.sum


let checkLine (line: string): int =
    let first = Set(line[0 .. (String.length line)/2 - 1])
    let second = Set(line[(String.length line)/2 .. (String.length line)-1])
    Set.intersect first second |> Seq.map scoreItem |> Seq.sum    


let part1 (input : string) =
    let lines = input.Split("\n")
    lines |> Seq.map checkLine |> Seq.sum
    
    
let part2 (input : string) =
    let lines = input.Split("\n")
    Seq.chunkBySize 3 lines |> Seq.map scoreGroup |> Seq.sum
