module day14

let t = System.Diagnostics.Stopwatch.StartNew()

type Point = { x: int; y: int }
type Wall(points: Point list) =
    static let wallMatcher p (start, finish) =
        if start.x = finish.x && p.x = start.x then
            p.y >= min start.y finish.y && p.y <= max start.y finish.y
        else if start.y = finish.y && p.y = start.y then
            p.x >= min start.x finish.x && p.x <= max start.x finish.x
        else false

    static let parsePoint (p: string) =
        let x::y = p.Trim(' ').Split(",") |> List.ofArray
        { Point.x = int(x); y = int(y |> Seq.exactlyOne) }
        
    member this.Contains(p: Point) =
        points |> Seq.pairwise |> Seq.exists (wallMatcher p)
        
    member this.MaxY() =
        points |> Seq.map (fun p -> p.y) |> Seq.max
        
    static member parse(input: string) =
        let p = input.Trim(' ').Split("->") |> Seq.map parsePoint |> List.ofSeq
        Wall(p)
        
let containsSandV1 (point: Point, sand: Point[]): bool =
    sand |> Array.contains point
    
let containsWallV1 (point: Point, walls: Wall[]): bool =
    walls |> Array.exists (fun w-> w.Contains(point))
        
let rec dropSand (start: Point, walls: Wall[], sand: Point[]): Point[] =
    if start.y > (walls |> Seq.map (fun w -> w.MaxY()) |> Seq.max) + 3 then sand else  // Exit when falling below the last wall
    let dropDown = { start with y = start.y + 1 }
    if not(containsWallV1(dropDown, walls)) && not(containsSandV1(dropDown, sand)) then dropSand(dropDown, walls, sand) else
    let dropLeft = { dropDown with x = start.x - 1 }
    if not(containsWallV1(dropLeft, walls)) && not(containsSandV1(dropLeft,sand )) then dropSand(dropLeft, walls, sand) else
    let dropRight = { dropDown with x = start.x + 1 }
    if not(containsWallV1(dropRight, walls)) && not(containsSandV1(dropRight, sand)) then dropSand(dropRight, walls, sand) else
    if start = { x=500; y=0 } then Array.append sand [|start|] else  // Exit when full of sand up to the source
    printfn "%A %A" walls.Length sand.Length
    printfn "%A" t.ElapsedMilliseconds
    dropSand({ x=500; y=0 }, walls, Array.append sand [|start|])

let part1 (input : string) =
    let walls = input.Split('\n') |> Seq.filter (fun line -> line.Length > 0) |> Seq.map Wall.parse |> Array.ofSeq
    printfn "%A" t.ElapsedMilliseconds
    let sand = dropSand({ x=500; y=0 }, walls, [||])
    sand |> Seq.length
    
let part2 (input : string) =
    let walls = input.Split('\n') |> Seq.filter (fun line -> line.Length > 0) |> Seq.map Wall.parse |> Array.ofSeq
    let maxY = (walls |> Seq.map (fun w -> w.MaxY()) |> Seq.max) + 2
    printfn "%A" t.ElapsedMilliseconds
    let sand = dropSand({ x=500; y=0 }, Array.append walls [|Wall([{ x=System.Int32.MinValue; y=maxY }; { x=System.Int32.MaxValue; y=maxY }])|], [||])
    sand |> Seq.length