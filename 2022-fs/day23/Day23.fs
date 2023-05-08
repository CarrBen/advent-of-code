module day23

open System.Diagnostics

type Elf = int * int
type Proposal = int * int * int
type Direction = North | South | West | East
type Elves = Elf[]
type Proposals = Proposal[]

let Directions = [Direction.North; Direction.South; Direction.West; Direction.East]

let parseLines(input: string): Elves =
    let lines = input.Split('\n') |> Array.filter (fun line -> line.Length > 0)
    let elves = lines |>
        Seq.indexed |>
        Seq.map (fun (y, line) -> line |>
            Seq.indexed |>
            Seq.filter (fun (x, char) -> char = '#') |>
            Seq.map (fun(x, _) -> (Elf(x, y)))) |>
        Seq.concat |>
        Seq.toArray
    elves
    
let rotatedDirections(count: int): Direction list =
    let modCount = count % 4
    List.append (Directions |> List.skip modCount) (Directions |> List.take (modCount))

let posIsEmptyDxDy(elf: Elf, elves: Elves, dx: int, dy: int): bool =
    (elves |> Array.tryFind (fun otherElf -> otherElf = ((fst elf) + dx, (snd elf) + dy))).IsNone

let dirIsEmpty(elf: Elf, elves: Elves, dir: Direction): bool =
    match dir with
        | North -> posIsEmptyDxDy(elf, elves, -1, -1) && posIsEmptyDxDy(elf, elves, 0, -1) && posIsEmptyDxDy(elf, elves, 1, -1)
        | South -> posIsEmptyDxDy(elf, elves, -1, 1) && posIsEmptyDxDy(elf, elves, 0, 1) && posIsEmptyDxDy(elf, elves, 1, 1)
        | West -> posIsEmptyDxDy(elf, elves, -1, -1) && posIsEmptyDxDy(elf, elves, -1, 0) && posIsEmptyDxDy(elf, elves, -1, 1)
        | East -> posIsEmptyDxDy(elf, elves, 1, -1) && posIsEmptyDxDy(elf, elves, 1, 0) && posIsEmptyDxDy(elf, elves, 1, 1)
    
let elfHasNeighbours(elf: Elf, elves: Elves): bool =
    let populatedDirs = Directions |> Seq.map (fun dir -> dirIsEmpty(elf, elves, dir)) |> Seq.filter (fun d -> not(d)) |> Seq.toArray
    populatedDirs.Length > 0
    
let generateProposal(elf: Elf, elfIndex: int, elves: Elves, dir: Direction): Proposal option =
    let ex, ey = elf
    let elvesCache = elves
    if not(dirIsEmpty(elf, elvesCache, dir)) then None else
    match dir with
        | North -> Some(Proposal(ex, ey - 1, elfIndex))
        | South -> Some(Proposal(ex, ey + 1, elfIndex))
        | West -> Some(Proposal(ex - 1, ey, elfIndex))
        | East -> Some(Proposal(ex + 1, ey, elfIndex))
        
let generateElfProposals(elf: Elf, elves: Elves, dirs: Direction list): Proposal option list =
    let ex, ey = elf
    let elvesCache = elves |> Array.filter (fun (ax, ay) -> ax >= (ex - 1) && ax <= (ex + 1) && ay >= (ey - 1) && ay <= (ey + 1))
    let elfIndex = elves |> Array.findIndex (fun t -> t = elf)
    if elvesCache.Length = 1 then [] else
    dirs |> List.map (fun dir ->  generateProposal(elf, elfIndex, elvesCache, dir))
    
let generateProposals(elves: Elves, current: int): Proposals =
    let roundDirections = rotatedDirections(current)
    elves |>
        Seq.map (fun elf -> generateElfProposals(elf, elves, roundDirections) |>
            Seq.tryFind (fun p -> p.IsSome)) |>
        Seq.filter (fun p -> p.IsSome) |>
        Seq.map (fun p -> p.Value.Value) |>
        Seq.toArray

let filterProposals(proposals: Proposals): Proposals =
    let invalidProposals = proposals |>
        Array.filter (fun (cx, cy, cElf) -> (proposals |>
            Array.filter (fun (ox, oy, oElf) -> (cx, cy) = (ox, oy))).Length > 1)
    proposals |> Array.except invalidProposals
    
let moveElf(currentElf: Elf, moves: Proposals, index: int): Elf =
    let move = moves |> Array.tryFind (fun (_, _, i) -> i = index)
    if move.IsNone then currentElf else
    let nx, ny, _ = move.Value
    Elf(nx, ny)
    
let moveElves(elves: Elves, moves: Proposals): Elves =
    let indexedElves = elves |> Array.indexed
    indexedElves |> Array.map (fun (i, elf) -> moveElf(elf, moves, i))
    
let scoreElves(elves: Elves): int =
    let maxX = elves |> Array.map (fun (x, y) -> x) |> Array.max
    let minX = elves |> Array.map (fun (x, y) -> x) |> Array.min
    let maxY = elves |> Array.map (fun (x, y) -> y) |> Array.max
    let minY = elves |> Array.map (fun (x, y) -> y) |> Array.min
    let area = (maxX - minX + 1) * (maxY - minY + 1)
    printfn "%A %A %A %A -> %A x %A - %A" maxX minX maxY minY (maxX - minX + 1) (maxY - minY + 1) elves.Length
    area - elves.Length

let rec step(limit: int, current: int, elves: Elves): Elves * int =
    if current >= limit then elves, (current + 1) else
    
    let timer = new Stopwatch()
    timer.Start()
    
    let proposals = generateProposals(elves, current)
    // printfn "Proposals %A" proposals
    printfn "Generate Proposals %Ams" timer.ElapsedMilliseconds
    if proposals.Length = 0 then
        timer.Stop()
        printfn "Aborting at %A" (current + 1)
        elves, (current + 1)
    else
    let moves = filterProposals(proposals)
    // printfn "Valid Proposals %A" moves
    printfn "Validate Proposals %Ams" timer.ElapsedMilliseconds
    let newElves = moveElves(elves, moves)
    // printfn "New Elves %A" newElves
    printfn "Move Elves %Ams" timer.ElapsedMilliseconds
    
    timer.Stop()
    printfn "Step %A took %Ams" (current + 1) timer.ElapsedMilliseconds
    
    step(limit, current + 1, newElves)    

let part1 (input : string) =
    let elves = parseLines(input)
    printfn "Loaded %A Elves" elves.Length
    let result, _ = step(10, 0, elves)
    let score = scoreElves(result)
    printfn "Score %A" score
    score
    
let part2 (input : string) =
    let elves = parseLines(input)
    printfn "Loaded %A Elves" elves.Length
    let result, steps = step(99999, 0, elves)
    printfn "Steps %A" steps
    steps