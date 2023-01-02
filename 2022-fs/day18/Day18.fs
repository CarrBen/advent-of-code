module day18

type Axis = int16
type Grid = Map<Axis, Map<Axis, Map<Axis, bool>>>
type Point = Axis * Axis * Axis
type World = { grid: Grid; points: Point list }

let parseLine (line: string): Point =
    let parts = line.Split(',')
    (int16 parts[0], int16 parts[1], int16 parts[2])

let buildGrid (points: Point list): Grid =
    let p = points |> 
            Seq.groupBy (fun (a,_,_) -> a) |> 
            Seq.map (fun (x, xGroup) -> (x, xGroup |> 
                Seq.groupBy (fun (_,a,_) -> a) |>
                Seq.map (fun (y, yGroup) -> (y, yGroup |>
                    Seq.map (fun (a,b,c) -> (c, true)) |>
                    Map
                )) |> Map
            ))
    new Grid(p)

let parseWorld (input: string): World =
    let points = input.Split('\n') |> Seq.filter (fun line -> line.Length > 0) |> Seq.map parseLine |> List.ofSeq
    { World.points=points; grid=buildGrid(points) }

let gridGet (grid: Grid, (px, py, pz): Point): bool =
    let x = grid.TryFind(px)
    if x.IsNone then false else
    let y = x.Value.TryFind(py)
    if y.IsNone then false else
    y.Value.TryFind(pz).IsSome

let rec fill (grid: Grid, volume: Point list, queue: Point list, visited: Point list): Point seq =
    if queue.Length = 0 then visited else
    let current::rest = queue
    let cx, cy, cz = current
    let newVisited = List.append visited [current]
    let dirs = seq { (1s,0s,0s); (-1s,0s,0s); (0s,1s,0s); (0s,-1s,0s); (0s,0s,1s); (0s,0s,-1s) }
    let candidates = dirs |> Seq.map (fun (dx, dy, dz) -> (cx+dx,cy+dy,cz+dz))
    let newQueue = candidates |> Seq.filter (fun p -> (volume |> Seq.contains p) && not(queue |> Seq.contains p) && not(visited |> Seq.contains p) && not(gridGet(grid, p))) |> List.ofSeq
    fill(grid, volume, List.append rest  newQueue, newVisited)

let fillEnclosed (world: World): World =
    let xValues = world.points |> Seq.map (fun (x,y,z) -> x)
    let yValues = world.points |> Seq.map (fun (x,y,z) -> y)
    let zValues = world.points |> Seq.map (fun (x,y,z) -> z)
    
    let xMin = xValues |> Seq.min
    let xMax = xValues |> Seq.max
    let yMin = yValues |> Seq.min
    let yMax = yValues |> Seq.max
    let zMin = zValues |> Seq.min
    let zMax = zValues |> Seq.max
    
    let volume = seq { (xMin - 1s) .. (xMax + 1s)} |> 
                 Seq.map (fun x -> seq { (yMin - 1s) .. (yMax + 1s)} |>
                    Seq.map (fun y -> seq { (zMin - 1s) .. (zMax + 1s)} |>
                        Seq.map (fun z -> (x,y,z))
                    ) |> Seq.concat
                 ) |> Seq.concat |> List.ofSeq
    printfn "Volume %A" (volume |> Seq.length)
    let outside = fill(world.grid, volume, [(xMin-1s,yMin-1s,zMin-1s)], [])
    printfn "Outside %A" (outside |> Seq.length)
    let enclosed = volume |> Seq.except outside |> Seq.except world.points
    printfn "Enclosed %A" (enclosed |> Seq.length)
    
    printfn "World Points %A" (world.points |> Seq.length)
    printfn "World Enclosed Points %A" (Seq.concat [enclosed; world.points] |> Seq.length)
    { world with grid=buildGrid(Seq.concat [enclosed; world.points] |> List.ofSeq) }

let scorePoint (grid: Grid) ((px, py, pz): Point): int =
    let dirs = seq { (1s,0s,0s); (-1s,0s,0s); (0s,1s,0s); (0s,-1s,0s); (0s,0s,1s); (0s,0s,-1s) }
    6 - (dirs |> Seq.map (fun (dx, dy, dz) -> if gridGet(grid, (px+dx, py+dy, pz+dz)) then 1 else 0) |> Seq.sum)

let part1 (input : string) =
    let world = parseWorld(input)
    world.points |> Seq.map (scorePoint world.grid) |> Seq.sum
    
let part2 (input : string) =
    let world = parseWorld(input)
    let enclosedWorld = fillEnclosed(world)
    world.points |> Seq.map (scorePoint enclosedWorld.grid) |> Seq.sum