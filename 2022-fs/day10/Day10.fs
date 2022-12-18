module day10

open System

type Cmd = NOOP | ADDX
type State = { cycle: int; value: int }
type Instruction = { cmd: Cmd; arg: int }

let parseInstructions (input: string): Instruction list =
    input.Split('\n') 
    |> Seq.filter (fun line -> line.Length > 0)
    |> Seq.map (fun line -> match line.Split(' ') with
                            | [| _ |] -> { Instruction.cmd = NOOP; arg = 0 }
                            | [| _; v |] -> { Instruction.cmd = ADDX; arg = int v }
    ) |> List.ofSeq
    
let executeInstruction : State -> Instruction -> State =
    fun start inst ->
        match inst.cmd with
            | NOOP -> { start with cycle = start.cycle + 1}
            | ADDX -> { State.cycle = start.cycle + 2; value = start.value + inst.arg }

let registerAt (input: string, cycle: int): int =
    let instructions = parseInstructions(input)
    let states = ({ State.cycle = 1; value = 1 }, instructions) ||> Seq.scan executeInstruction
    let firstState = states |> Seq.tryFind (fun s -> s.cycle = cycle)
    if firstState.IsSome then firstState.Value.value else
    let secondState = states |> Seq.tryFind (fun s -> s.cycle = (cycle - 1))
    if secondState.IsSome then secondState.Value.value else
    let thirdState = states |> Seq.tryFind (fun s -> s.cycle = (cycle - 2))
    if thirdState.IsSome then thirdState.Value.value else
    0
    
let part1 (input : string) =
    [ 20; 60; 100; 140; 180; 220 ]
    |> Seq.map (fun c -> registerAt(input, c) * c)
    |> Seq.sum
    
let part2 (input : string) =
    let strings = [0 .. 5] |> Seq.map (fun row -> [1 .. 40] |> Seq.map (fun x -> if x - 2 <= registerAt(input, row * 40 + x) && registerAt(input, row * 40 + x) <= x + 0 then '#' else '.')) |> Seq.map String.Concat 
    strings |> Seq.map (fun line -> printfn "%A" line) |> List.ofSeq