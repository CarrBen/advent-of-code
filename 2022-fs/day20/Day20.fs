module day20

let newIndex (input: (int * int64) list, (originalIndex: int, value: int64)): int =
    let index = input |> List.findIndex (fun (index, v) -> originalIndex = index && value = v)
    let newIndex = int64(index) + value
    // printfn "%A at %A Wraps %A x %A" value originalIndex wraps input.Length
    if newIndex < 0L then int(newIndex % (int64(input.Length) - 1L)) + input.Length - 1 else
    int(newIndex % (int64(input.Length) - 1L)) 

let mix (input: (int * int64) list, (originalIndex: int, value: int64)): (int * int64) list =
    let index = input |> List.findIndex (fun (index, v) -> originalIndex = index && value = v)
    let removed = input |> List.removeAt index
    let ni = newIndex(input, (originalIndex, value))
    if ni < 0 || newIndex(input, (originalIndex, value)) >= input.Length then printfn "Bad index %A mixing %A" ni value
    removed |> List.insertAt ni (originalIndex, value)
    
let get (input: (int * int64) list) (index: int): int64 =
    printfn "Getting %A/%A" index input.Length
    let start = input |> List.findIndex (fun (i, v) -> v = 0)
    printfn "Start is %A" start
    let targetIndex = (start + index) % input.Length
    printfn "Target is %A" targetIndex
    let _, v = input[targetIndex]
    v
    
let part1 (input : string) =
    let lines = input.Split('\n') |> Seq.filter (fun line -> line.Length > 0)
    let start = lines |> Seq.map int64 |> Seq.indexed |> List.ofSeq
    
    let mutable current = start
    for index, item in start do
        current <- mix(current, (index, item))
    seq { 1000; 2000; 3000 } |> Seq.map (fun i -> get current i) |> Seq.sum
    
let DECRYPTION_KEY = 811589153L
    
let part2 (input : string) =
    let lines = input.Split('\n') |> Seq.filter (fun line -> line.Length > 0)
    let start = lines |> Seq.map int64 |> Seq.map ((*) DECRYPTION_KEY) |> Seq.indexed |> List.ofSeq
    
    let mutable current = start
    for i in 1 .. 10 do
        printfn "Part 2 mix %A" i
        for index, item in start do
            current <- mix(current, (index, item))
    seq { 1000; 2000; 3000 } |> Seq.map (fun i -> get current i) |> List.ofSeq |> List.sum