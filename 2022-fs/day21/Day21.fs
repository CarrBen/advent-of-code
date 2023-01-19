module day21

type Op = Plus = '+' | Minus = '-' | Mult = '*' | Div = '/'
type Monkey = Constant of int64 | Operation of Op * string * string

let extractOperator(monk: string): Op =
    let parts = monk.Split(' ')
    match char(parts[1]) with
        | '/' -> Op.Div
        | '*' -> Op.Mult
        | '+' -> Op.Plus
        | '-' -> Op.Minus
        | _ -> raise (System.InvalidOperationException())
        
let getOp(op: Op): (int64 -> int64 -> int64) =
    match op with
        | Op.Div -> (/)
        | Op.Mult -> (*)
        | Op.Plus -> (+)
        | Op.Minus -> (-)
        | _ -> raise (System.InvalidOperationException())
        
let extractNames(monk: string): string * string =
    let parts = monk.Split(' ')
    (parts[0], parts[2])

let parseMonkeyPart1(line: string): string * Monkey =
    let parts = line.Split(": ")
    let name = parts[0]
    let monk = parts[1]
    
    try
        let constant = monk |> int64
        (name, Constant constant)
    with :? System.FormatException ->
        let operator = extractOperator(monk)
        let left, right = extractNames(monk)
        (name, Operation (operator, left, right))

let parseMonkeyPart2(line: string): string * Monkey =
    let parts = line.Split(": ")
    let name = parts[0]
    
    //if name = "humn" then (name, Target) else
    
    parseMonkeyPart1(line)

let parseMonkeys(input: string, parser: string -> (string * Monkey)): Map<string, Monkey> =
    let lines = input.Split('\n')  |> Seq.filter (fun line -> line.Length > 0)
    lines |> Seq.map parser |> Map

let rec getValue(monkeys: Map<string, Monkey>, name: string): int64 =
    let monk = monkeys[name]
    match monk with
        | Constant m -> m
        | Operation (op, l, r) -> getOp(op) (getValue(monkeys, l)) (getValue(monkeys, r))

let rec findMonkey(monkeys: Map<string, Monkey>, target: string, current: string): bool =
    let monkey = monkeys[current]
    match monkey with
        | Constant _ -> false
        | Operation (op, l, r) -> if l = target || r = target then true else findMonkey(monkeys, target, l) || findMonkey(monkeys, target, r)

let rec findPath(monkeys: Map<string, Monkey>, target: string, current: string): (Monkey * string) list option =
    if current = target then Some([]) else
    let m = monkeys[current]
    match m with
        | Constant _ -> None
        | Operation (op, l, r) ->
            let left = findPath(monkeys, target, l)
            if left.IsSome then Some(List.append [(m, l)] left.Value) else
            let right = findPath(monkeys, target, r)
            if right.IsSome then Some(List.append [(m, r)] right.Value) else
            None
            
let rec reverseOp(monkeys: Map<string, Monkey>, Operation (op, l, r): Monkey, subject: string, value: int64): int64 =
    if subject = l then
        match op with
            | Op.Plus-> value - getValue(monkeys, r)
            | Op.Minus -> value + getValue(monkeys, r)
            | Op.Mult -> value / getValue(monkeys, r)
            | Op.Div -> value * getValue(monkeys, r)
    else
        match op with
            | Op.Plus-> value - getValue(monkeys, l)
            | Op.Minus -> (value - getValue(monkeys, l)) * -1L
            | Op.Mult -> value / getValue(monkeys, l)
            | Op.Div -> getValue(monkeys, l) / value
            
let rec applyOp(monkeys: Map<string, Monkey>, ops: (Monkey * string) list, start: int64): int64 =
    let (op, subject)::nextOps = ops
    
    let nextValue = reverseOp(monkeys, op, subject, start)
    
    if nextOps.Length > 0 then applyOp(monkeys, nextOps, nextValue)
    else nextValue

let part1 (input : string) =
    let m = parseMonkeys(input, parseMonkeyPart1)
    getValue(m, "root")
    
let part2 (input : string) =
    let m = parseMonkeys(input, parseMonkeyPart2)
    let (Operation (_, l, r)) = m["root"]
    let target = if findMonkey(m, "humn", l) then r else l
    let targetValue = getValue(m, target)
    let humn = if findMonkey(m, "humn", l) then l else r
    printfn "Target is value of %A (%A) humn is a descendant of %A" target targetValue humn
    let path = findPath(m, "humn", humn)
    printfn "Path is %A" path.Value
    applyOp(m, path.Value, targetValue)