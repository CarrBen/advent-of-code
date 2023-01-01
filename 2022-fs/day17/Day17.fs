module day17

type Rock = byte[]
type Grid = byte[]
type State = { y: int; rock: Rock; jetIndex: int; atRest: bool }


let rocks = [|
    [| 0uy; 0uy; 0uy; 0b00111100uy |];
    [| 0uy; 0b00010000uy; 0b00111000uy; 0b00010000uy |];
    [| 0uy; 0b00001000uy; 0b00001000uy; 0b00111000uy |];
    [| 0b00100000uy; 0b00100000uy; 0b00100000uy; 0b00100000uy |];
    [| 0uy; 0uy; 0b00110000uy; 0b00110000uy |];
|]

let printGrid (grid: byte[], lower: int, upper: int): unit =
    for y in { upper .. -1 .. lower } do
        printfn "%04d %08B" y grid[y]

let rockCollision (y: int, rock: byte[], grid: byte[]): bool =
    // The first rev is because rocks are defined the right way up, but the grid is upside down
    rock |> Array.rev |> Array.indexed |> Array.exists (fun (ry, row) -> not((row &&& grid[y + ry]) = 0uy))
    
let rockStart (grid: byte[], yHint: int): int =
    (seq { (max (yHint) 0) .. (grid.Length - 1) } |> Seq.filter (fun y -> grid[y] = 0uy) |> Seq.head) + 3
    
let updateGrid(grid: byte[], state: State): byte[] =
    let before, other = grid |> Array.splitAt state.y
    let target, after = other |> Array.splitAt 4
    let flippedRock = state.rock |> Array.rev  // Flip as rocks are defined the correct way up, but grid is upside down
    let newTarget = target |> Array.indexed |> Array.map (fun (index, row) -> (row ||| flippedRock[index]))
    seq { before; newTarget; after } |> Array.concat

let dropRock (rock: Rock, grid: Grid, jets: string, jetIndex: int, y: int): State =
    let jet = jets[int(jetIndex)]
    let x = match jet with
        | '>' -> 1
        | '<' -> -1
        | _ -> failwith "Unexpected jet"
    let candidateRock = rock |> Array.map (fun row -> if x = 1 then (row >>> 1) else (row <<< 1))
    
    let wallJettedRock = if ((rock |> Array.exists (fun row -> (row &&& 0b10000000uy) = 0b10000000uy)) && x= -1) || (candidateRock |> Array.exists (fun row -> (row &&& 0b00000001uy) = 0b00000001uy)) then rock else candidateRock
    let rockJettedRock = if rockCollision(y, wallJettedRock, grid) then rock else wallJettedRock
    let rockAtRest = if y=0 then true else rockCollision(y - 1, rockJettedRock, grid)
    let finalY = if rockAtRest then y else y - 1
    
    { State.y=finalY; rock=rockJettedRock; jetIndex=(jetIndex+1) % jets.Length; atRest=rockAtRest }

let part1 (input : string, rockLimit: int) =
    let jets = input
    let mutable grid = 0uy |> Array.replicate 4096
    let mutable jetIndex = 0
    let mutable rockCount = 0
    let mutable yHint = 0

    while rockCount < rockLimit do
        let rock = rocks[rockCount % rocks.Length]
        
        let mutable state = { State.y=rockStart(grid, yHint); jetIndex=jetIndex; rock=rock; atRest=false }
        while not(state.atRest) do
            state <- dropRock(state.rock, grid, jets, state.jetIndex, state.y)
            jetIndex <- state.jetIndex
        
        grid <- updateGrid(grid, state)

        rockCount <- rockCount + 1
        yHint <- state.y
    
    rockStart(grid, yHint) - 3
    
type PatternKey = { rockIndex: int; jetIndex: int; context1: int64; context2: int64 }
type PatternInfo = { rockCount: int64; y: int64 }
    
let part2 (input : string, rockLimit: int64) =
    let jets = input
    let mutable grid = 0uy |> Array.replicate (8192)
    let mutable jetIndex = 0
    let mutable rockCount = 0L
    let mutable yHint = 0L  // Last ending position, used as a hint to find the next start position quicker
    let mutable yOffset = 0L  // Subtract this from yHint to go from real-space to array space
    let mutable patternMap = Map<PatternKey, PatternInfo>([])

    while rockCount < rockLimit do
        let rock = rocks[int(rockCount % rocks.LongLength)]
        
        let patternKey = { PatternKey.rockIndex=int(rockCount % rocks.LongLength); jetIndex=jetIndex; context1=System.BitConverter.ToInt64(grid, (max (int(yHint - yOffset) - 4) 0)); context2=System.BitConverter.ToInt64(grid, (max (int(yHint - yOffset) - 12) 0)) }
        let patternHit = patternMap |> Map.containsKey patternKey
        
        if patternHit && yOffset = 0 then
            let firstInfo = patternMap[patternKey]
            let rockPeriod = rockCount - firstInfo.rockCount
            let yPeriod = yHint - firstInfo.y
            
            let rocksNeeded = rockLimit - rockCount
            let patternRepeats = rocksNeeded / rockPeriod
            yOffset <- patternRepeats * yPeriod
            yHint <- yHint + yOffset
            rockCount <- rockCount + (patternRepeats * rockPeriod)
            
            printfn "Pattern %A %A %A %A" patternKey rockPeriod yPeriod patternRepeats
        else ()

        //if patternHit then printfn "Pattern Hit %A %A" yHint patternKey; printGrid(grid, yHint - 20, yHint + 4) else ()
        patternMap <- Map.add patternKey { PatternInfo.rockCount=rockCount; y=yHint } patternMap
        
        let mutable state = { State.y=rockStart(grid, int(yHint - yOffset)); jetIndex=jetIndex; rock=rock; atRest=false }
        
        while not(state.atRest) do
            state <- dropRock(state.rock, grid, jets, state.jetIndex, state.y)
            jetIndex <- state.jetIndex
        
        grid <- updateGrid(grid, state)

        rockCount <- rockCount + 1L
        yHint <- int64(state.y) + yOffset
    
    int64(rockStart(grid, int(yHint - yOffset))) + yOffset - 3L