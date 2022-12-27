module day13

type Outcome = Right | Wrong | Continue
type Element = Const of int | List of Element list
type Pair = { left: Element; right: Element }

let rec parseElement (input: char list, buffer: char list, stack: Element list list): Element =
    //printfn "%A %A %A" input buffer stack
    let c::rest = input
    if c = '[' then
        parseElement(rest, [], List.append [[]] stack)
    else if c = ']' then
        let current::others = stack
        let newCurrent = if buffer.Length = 0 then current else List.append current [Const (buffer |> Array.ofList |> System.String |> int)]
        if stack.Length = 1 then List newCurrent else
        let parent::more = others
        //printfn "On] %A %A %A" newCurrent parent more
        parseElement(rest, [], List.append [(List.append parent [List newCurrent])] more)
    else if c = ',' then
        if buffer.Length = 0 then parseElement(rest, [], stack) else
        let current::others = stack
        parseElement(rest, [], List.append [(List.append current [Const (buffer |> Array.ofList |> System.String |> int)])] others)
    else parseElement(rest, List.append buffer [c], stack)

let parsePair (input: string): Pair =
    let lines = input.Split('\n')
    { Pair.left = (parseElement(lines[0] |> List.ofSeq, [], [])); right = (parseElement(lines[1] |> List.ofSeq, [], [])) }

let parsePairs (input: string): Pair list =
    input.Split("\n\n") |> Seq.filter (fun line -> line.Length > 0) |> Seq.map parsePair |> List.ofSeq
    
let parseElements (input: string): Element list =
    input.Split("\n") |> Seq.filter (fun line -> line.Length > 0) |> Seq.map (fun line -> parseElement(line |> List.ofSeq, [], [])) |> List.ofSeq

let rec pickValid (l: Element option, r: Element option): Outcome option =
    if l.IsNone && r.IsSome then Some(Right) else
    if l.IsSome && r.IsNone then Some(Wrong) else
    let result = elementsValid(l.Value, r.Value)
    if result = Continue then 
    None else 
    Some(result)

and elementsValid(left: Element, right: Element): Outcome =
    match left, right with
        | Const l, Const r -> if l < r then Right else if l > r then Wrong else Continue 
        | List l, List r -> 
            let lList = Seq.append (l |> Seq.map Some) (None |> Seq.replicate ([0; r.Length - l.Length] |> Seq.max))
            let rList = Seq.append (r |> Seq.map Some) (None |> Seq.replicate ([0; l.Length - r.Length] |> Seq.max))
            let res = Seq.zip lList rList |> Seq.tryPick pickValid
            if res.IsSome then res.Value else Continue
        | Const l, List r -> elementsValid(List [Const l], List r)
        | List l, Const r -> elementsValid(List l, List [Const r])

let pairValid (pair: Pair): bool =
    match elementsValid(pair.left, pair.right) with
        | Right -> true
        | Wrong -> false
        | Continue -> printfn "Got Contiue for pair %A" pair; false
        
let compareElements: Element -> Element -> int =
    fun a b ->
        let result = elementsValid(a,b)
        match result with
            | Right -> -1
            | Wrong ->  1
            | Continue -> 0

let part1 (input : string) =
    let pairs = parsePairs(input)
    pairs |> Seq.map (fun p -> pairValid p) |> Seq.indexed |> Seq.filter (fun (i, r) -> r) |> Seq.map (fun (i, r) -> i + 1) |> Seq.sum
    
let part2 (input : string) =
    let elements = (List.append (parseElements input) [List ([List [Const 2]]); List ([List [Const 6]])])
    let sorted = elements |> List.sortWith compareElements
    let a = sorted |> Seq.findIndex (fun item -> item = List [List [Const 2]])
    let b = sorted |> Seq.findIndex (fun item -> item = List [List [Const 6]])
    (a+1)*(b+1)