module day6

let packetCheckFrom (input: string, index: int): int * bool =
    index + 1, input.Substring(index-3, 4) |> Seq.distinct |> Seq.length = 4
    
let messageCheckFrom (input: string, index: int): int * bool =
    index + 1, input.Substring(index-13, 14) |> Seq.distinct |> Seq.length = 14

let part1 (input : string) =
    { 4 .. input.Length } |> Seq.map (fun i -> packetCheckFrom(input, i)) |> Seq.find (fun (i, r) -> r) |> (fun (i, r) -> i)
    
let part2 (input : string) =
    { 14 .. input.Length } |> Seq.map (fun i -> messageCheckFrom(input, i)) |> Seq.find (fun (i, r) -> r) |> (fun (i, r) -> i)