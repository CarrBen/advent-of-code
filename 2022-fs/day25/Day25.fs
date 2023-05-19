module day25

let charToInt(input: char): int64 =
    match input with
        | '=' -> -2
        | '-' -> -1
        | '0' -> 0
        | '1' -> 1
        | '2' -> 2

let intToChar(input: int64): char =
    match input with
        | -2L -> '='
        | -1L -> '-'
        | 0L -> '0'
        | 1L -> '1'
        | 2L -> '2'

let snafuToDec(input: string): int64 =
    input |> Seq.rev |> Seq.indexed |> Seq.map (fun (i, c) -> if i = 0 then charToInt(c) else charToInt(c) * int64(5.0 ** float(i))) |> Seq.sum
    
let rec decToSnafu(input: int64, index: int): string =
    let value = int64(5.0 ** float(index))
    if (input * 5L) < value then "" else
    let offset = input + (2L * value)
    let modulo = offset % (value * 5L)
    let result = (modulo / value) - 2L
    let output = string(intToChar(result))
    let newInput = input - (result * value)
    if result = 0 && newInput < value then output else
    [| decToSnafu(newInput, index + 1); output |] |> String.concat ""
    

let part1 (input : string) =
    let lines = input.Split('\n') |> Array.filter (fun line -> line.Length > 0)
    let sum = lines |> Seq.map snafuToDec |> Seq.sum
    decToSnafu(sum, 0)
    
let part2 (input : string) =
    0