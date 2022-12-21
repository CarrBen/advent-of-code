module day11

type Argument = Const of int64 | Old of int64
type Op = { op: int64 -> int64 -> int64; left: Argument; right: Argument }
type Test = { modulo: int }
type Monkey = { id: int; levels: int64 list; ifTrue: int; ifFalse: int; test: Test; op: Op; inspectionCount: int }

let splitMonkeys : string list list -> string -> string list list =
    fun state line ->
        if line.StartsWith("Monkey") then Seq.append state [[line]] |> List.ofSeq else
        let revState = state |> Seq.rev
        Seq.append [(Seq.append (revState |> Seq.head) [line]) |> List.ofSeq] (revState |> Seq.tail |> List.ofSeq) |> Seq.rev |> List.ofSeq

let parseArg (input: string): Argument =
    match input with
        | "old" -> Old(0)
        | _ -> Const(int64 input)
    
let parseOperator (input: string): int64 -> int64 -> int64 =
    match input with
        | "+" -> (+)
        | "-" -> (-)
        | "*" -> (*)
        | "/" -> (/)

let parseOp (input: string[]): Op =
    {
        op = parseOperator(input.[1]);
        left = parseArg(input.[0]);
        right = parseArg(input.[2]);
    }

let parseMonkey (input: string list): Monkey =
    {
        Monkey.id = int (input[0].Split(' ').[1].Split(':').[0]);
        levels = input[1].Split("items:").[1].Split(',') |> Seq.map int64 |> List.ofSeq;
        op = input[2].Split("new =").[1].Trim(' ').Split(' ') |> parseOp;
        test = { Test.modulo = int (input[3].Split("divisible by")[1]) };
        ifTrue = int (input[4].Split("monkey")[1]);
        ifFalse = int (input[5].Split("monkey")[1]);
        inspectionCount = 0;
    }

let parseMonkeys (input: string): Monkey list =
    let lines = input.Split('\n') |> Seq.filter (fun line -> line.Length > 0)
    ([], lines) ||> Seq.fold splitMonkeys |> Seq.map parseMonkey |> List.ofSeq
    
let updateLevelPart1 : Op * int64 -> int64 -> int64 =
    fun (operation, modulo) old ->
        (/) (operation.op (
            match operation.left with
                | Const c -> c
                | Old _ -> old
            ) (
            match operation.right with
                | Const c -> c
                | Old _ -> old
            )
        ) 3L % modulo
        
let updateLevelPart2 : Op * int64 -> int64 -> int64 =
    fun (operation, modulo) old ->
        (/) (operation.op (
            match operation.left with
                | Const c -> c
                | Old _ -> old
            ) (
            match operation.right with
                | Const c -> c
                | Old _ -> old
            )
        ) 1L % modulo
    
let whenTrue : Test -> int64 -> bool =
    fun test value ->
        (value % int64(test.modulo)) = 0L
        
let whenFalse : Test -> int64 -> bool =
    fun test value ->
        (value % int64(test.modulo)) <> 0L
    
let runMonkeyLevelsPart1 : Monkey * int64 -> int -> int64 seq =
    fun (target, modulo) index ->
        if target.ifTrue = index then target.levels |> Seq.map (updateLevelPart1 (target.op, modulo)) |> Seq.filter (whenTrue target.test) else
        if target.ifFalse = index then target.levels |> Seq.map (updateLevelPart1 (target.op, modulo)) |> Seq.filter (whenFalse target.test) else
        []
        
let runMonkeyLevelsPart2 : Monkey * int64 -> int -> int64 seq =
    fun (target, modulo) index ->
        if target.ifTrue = index then target.levels |> Seq.map (updateLevelPart2 (target.op, modulo)) |> Seq.filter (whenTrue target.test) else
        if target.ifFalse = index then target.levels |> Seq.map (updateLevelPart2 (target.op, modulo)) |> Seq.filter (whenFalse target.test) else
        []
    
let runMonkeyPart1 : Monkey * int64 -> int * Monkey -> Monkey =
    fun (target, modulo) (currentIndex, current) ->
        { 
            current with 
            levels = Seq.append (if target.id = currentIndex then [] else current.levels) (runMonkeyLevelsPart1 (target, modulo) currentIndex) |> List.ofSeq
            inspectionCount = if current.id = target.id then current.inspectionCount + current.levels.Length else current.inspectionCount
        }
        
let runMonkeyPart2 : Monkey * int64 -> int * Monkey -> Monkey =
    fun (target, modulo) (currentIndex, current) ->
        { 
            current with 
            levels = Seq.append (if target.id = currentIndex then [] else current.levels) (runMonkeyLevelsPart2 (target, modulo) currentIndex) |> List.ofSeq
            inspectionCount = if current.id = target.id then current.inspectionCount + current.levels.Length else current.inspectionCount
        }
    
let runMonkeysPart1 : Monkey list -> int -> Monkey list =
    fun start index ->
        start |> Seq.indexed |> Seq.map (runMonkeyPart1 (start.[index], (start |> Seq.map (fun m -> int64(m.test.modulo)) |> Seq.reduce (*)))) |> List.ofSeq
        
let runMonkeysPart2 : Monkey list -> int -> Monkey list =
    fun start index ->
        start |> Seq.indexed |> Seq.map (runMonkeyPart2 (start.[index], (start |> Seq.map (fun m -> int64(m.test.modulo)) |> Seq.reduce (*)))) |> List.ofSeq
    
let runRoundPart1 (start: Monkey list): Monkey list =
    (start, { 0 .. (start.Length - 1) }) ||> Seq.fold runMonkeysPart1 |> List.ofSeq
    
let runRoundPart2 (start: Monkey list): Monkey list =
    (start, { 0 .. (start.Length - 1) }) ||> Seq.fold runMonkeysPart2 |> List.ofSeq

let part1 (input : string) =
    let monkeys = parseMonkeys(input);
    (monkeys, { 0 .. 19 } ) ||> Seq.fold (fun m i -> runRoundPart1(m))
    |> Seq.sortByDescending (fun item -> item.inspectionCount) 
    |> Seq.take 2 
    |> Seq.map (fun m -> m.inspectionCount) 
    |> List.ofSeq 
    |> List.reduce (*)
    
let part2 (input : string) =
    let monkeys = parseMonkeys(input);
    (monkeys, { 0 .. 9999 } ) ||> Seq.fold (fun m i -> runRoundPart2(m))
    |> Seq.sortByDescending (fun item -> item.inspectionCount) 
    |> Seq.take 2 
    |> Seq.map (fun m -> int64(m.inspectionCount))
    |> List.ofSeq 
    |> List.reduce (*)