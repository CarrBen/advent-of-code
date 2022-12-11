module day4

let parseSections (range: string): Set<int> =
    let start = range.Split('-')[0] |> int
    let finish = range.Split('-')[1] |> int
    
    Set({ start .. finish })

let checkLinePart1 (line: string): int =
    if line.Length < 1 then 0
    else
    
    let left = line.Split(',')[0]
    let right = line.Split(',')[1]
    
    let a = parseSections left
    let b = parseSections right
    
    if Set.isSubset a b || Set.isSubset b a then 1
    else 0
    
let checkLinePart2 (line: string): int =
    if line.Length < 1 then 0
    else
    
    let left = line.Split(',')[0]
    let right = line.Split(',')[1]
    
    let a = parseSections left
    let b = parseSections right
    
    if Set.intersect a b |> Set.count > 0 then 1
    else 0

let part1 (input : string) =
    let lines = input.Split('\n')
    lines |> Seq.map checkLinePart1 |> Seq.sum
    
    
let part2 (input : string) =
    let lines = input.Split('\n')
    lines |> Seq.map checkLinePart2 |> Seq.sum
