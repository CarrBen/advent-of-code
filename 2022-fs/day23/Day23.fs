module day23

open System.Diagnostics

type Elf = int * int
type Proposal = int * int * int
type Direction = North | South | West | East
type Elves = Elf list
type Proposals = Proposal list

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
        Seq.toList
    elves
    
let rotatedDirections(count: int): Direction list =
    let modCount = count % 4
    List.append (Directions |> List.skip modCount) (Directions |> List.take (modCount))

let posIsEmptyDxDy(elf: Elf, elves: Elves, dx: int, dy: int): bool =
    (elves |> List.tryFind (fun otherElf -> otherElf = ((fst elf) + dx, (snd elf) + dy))).IsNone

let dirIsEmpty(elf: Elf, elves: Elves, dir: Direction): bool =
    match dir with
        | North -> posIsEmptyDxDy(elf, elves, -1, -1) && posIsEmptyDxDy(elf, elves, 0, -1) && posIsEmptyDxDy(elf, elves, 1, -1)
        | South -> posIsEmptyDxDy(elf, elves, -1, 1) && posIsEmptyDxDy(elf, elves, 0, 1) && posIsEmptyDxDy(elf, elves, 1, 1)
        | West -> posIsEmptyDxDy(elf, elves, -1, -1) && posIsEmptyDxDy(elf, elves, -1, 0) && posIsEmptyDxDy(elf, elves, -1, 1)
        | East -> posIsEmptyDxDy(elf, elves, 1, -1) && posIsEmptyDxDy(elf, elves, 1, 0) && posIsEmptyDxDy(elf, elves, 1, 1)
    
let elfHasNeighbours(elf: Elf, elves: Elves): bool =
    let populatedDirs = Directions |> Seq.map (fun dir -> dirIsEmpty(elf, elves, dir)) |> Seq.filter (fun d -> not(d)) |> Seq.toArray
    populatedDirs.Length > 0
    
let generateProposal(elf: Elf, elves: Elves, dir: Direction): Proposal option =
    let elfIndex = elves |> List.findIndex (fun t -> t = elf)
    if not(elfHasNeighbours(elf, elves)) then None else
    if not(dirIsEmpty(elf, elves, dir)) then None else
    match dir with
        | North -> Some(Proposal(fst elf, (snd elf) - 1, elfIndex))
        | South -> Some(Proposal(fst elf, (snd elf) + 1, elfIndex))
        | West -> Some(Proposal((fst elf) - 1, snd elf, elfIndex))
        | East -> Some(Proposal((fst elf) + 1, snd elf, elfIndex))
    
let generateProposals(elves: Elves, current: int): Proposals =
    elves |>
        Seq.map (fun elf -> rotatedDirections(current) |>
            Seq.map (fun dir -> generateProposal(elf, elves, dir)) |>
            Seq.tryFind (fun p -> p.IsSome)) |>
        Seq.filter (fun p -> p.IsSome) |>
        Seq.map (fun p -> p.Value.Value) |>
        Seq.toList

let filterProposals(proposals: Proposals): Proposals =
    let invalidProposals = proposals |>
        List.filter (fun (cx, cy, cElf) -> (proposals |>
            List.filter (fun (ox, oy, oElf) -> (cx, cy) = (ox, oy))).Length > 1)
    proposals |> List.except invalidProposals
    
let moveElf(currentElf: Elf, moves: Proposals, index: int): Elf =
    let move = moves |> List.tryFind (fun (_, _, i) -> i = index)
    if move.IsNone then currentElf else
    let nx, ny, _ = move.Value
    Elf(nx, ny)
    
let moveElves(elves: Elves, moves: Proposals): Elves =
    let indexedElves = elves |> List.indexed
    indexedElves |> List.map (fun (i, elf) -> moveElf(elf, moves, i))
    
let scoreElves(elves: Elves): int =
    let maxX = elves |> List.map (fun (x, y) -> x) |> List.max
    let minX = elves |> List.map (fun (x, y) -> x) |> List.min
    let maxY = elves |> List.map (fun (x, y) -> y) |> List.max
    let minY = elves |> List.map (fun (x, y) -> y) |> List.min
    let area = (maxX - minX + 1) * (maxY - minY + 1)
    printfn "%A %A %A %A -> %A x %A - %A" maxX minX maxY minY (maxX - minX + 1) (maxY - minY + 1) elves.Length
    area - elves.Length

let rec step(limit: int, current: int, elves: Elves): Elves =
    if current >= limit then elves else
    
    let timer = new Stopwatch()
    timer.Start()
    
    let proposals = generateProposals(elves, current)
    printfn "Proposals %A" proposals
    printfn "Generate Proposals %Ams" timer.ElapsedMilliseconds
    if proposals.Length = 0 then
        timer.Stop()
        printfn "Aborting at %A" current
        elves
    else
    let moves = filterProposals(proposals)
    printfn "Valid Proposals %A" moves
    printfn "Validate Proposals %Ams" timer.ElapsedMilliseconds
    let newElves = moveElves(elves, moves)
    printfn "New Elves %A" newElves
    printfn "Move Elves %Ams" timer.ElapsedMilliseconds
    
    timer.Stop()
    
    step(limit, current + 1, newElves)    

let part1 (input : string) =
    let elves = parseLines(input)
    printfn "Loaded %A Elves" elves.Length
    let result = step(10, 0, elves)
    let score = scoreElves(result)
    printfn "Score %A" score
    score
    
let part2 (input : string) =
    0