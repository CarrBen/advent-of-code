module day15

// http://www.fssnip.net/29/title/Regular-expression-active-pattern
open System.Text.RegularExpressions

let (|Regex|_|) pattern input =
    let m = Regex.Match(input, pattern)
    if m.Success then Some(List.tail [ for g in m.Groups -> g.Value ])
    else None


type Point = { x: int; y: int }
type Sensor = { loc: Point; beacon: Point }
type Region = { left: int; right: int }

let parseSensor (input: string): Sensor =
    match input with
        | Regex @"Sensor at x=(\-?\d+), y=(\-?\d+): closest beacon is at x=(\-?\d+), y=(\-?\d+)" [sx; sy; bx; by] -> { loc={ x=int sx; y=int sy }; beacon={ x=int bx; y=int by }}
        | _ -> { Sensor.loc={x=0; y=0}; beacon={x=0; y=0}}

let dist (a: Point, b: Point): int =
    (abs (a.x - b.x)) + (abs (a.y - b.y))
    
let rowRegion (row: int, s: Sensor): Region option =
    let d = dist(s.loc, s.beacon)
    if abs (row - s.loc.y) > d then None else
    Some({ left=(s.loc.x + (abs (row-s.loc.y))) - d; right=(s.loc.x - (abs (row-s.loc.y))) + d })
    
let foldRegionUpTo (limit: int) (count: int, last: int) region =
        let r = min region.right limit
        if region.left <= last && r > last then (count + (r - last), r)
        else if r > last then (count + (r - region.left + 1), r)
        else (count, last)
        
let foldRegionUpToGap (limit: int) (count: int, last: int) region =
        let r = min region.right limit
        if region.left > last then
            printfn "Last %A Next %A" last region.left
        if region.left <= last && r > last then (count + (r - last), r)
        else if r > last then (count + (r - region.left + 1), r)
        else (count, last)

let part1 (input : string, row: int) =
    let sensors = input.Split('\n') |> Seq.filter (fun line -> line.Length > 0) |> Seq.map parseSensor
    let regions = sensors |> Seq.map (fun s -> rowRegion(row, s)) |> Seq.filter (fun r -> r.IsSome) |> Seq.map (fun r -> r.Value) |> Seq.sortBy (fun r -> r.left)
    let leftmost = regions |> Seq.map (fun r -> r.left) |> Seq.min
    regions |> Seq.map (printfn "%A") |> List.ofSeq
    ((0, leftmost), regions) ||> Seq.scan (foldRegionUpTo System.Int32.MaxValue) |> Seq.map (printfn "%A") |> List.ofSeq
    let count, _ = ((0, leftmost), regions) ||> Seq.fold (foldRegionUpTo System.Int32.MaxValue)
    count
    
let part2 (input : string, limit: int) =
    let sensors = input.Split('\n') |> Seq.filter (fun line -> line.Length > 0) |> Seq.map parseSensor
    let rows = { 0 .. limit } |> Seq.map (fun row -> sensors |> Seq.map (fun s -> rowRegion(row, s)) |> Seq.filter (fun r -> r.IsSome) |> Seq.map (fun r -> r.Value) |> Seq.sortBy (fun r -> r.left))
    let counts =  rows |> Seq.map (fun row -> ((0, 0), row) ||> Seq.fold (foldRegionUpTo limit)) |> Seq.indexed |> List.ofSeq
    let y, _ = counts |> Seq.find (fun (i, (count, l)) -> count < l)
    printfn "Row %A" y
    let t = ((0, 0), (rows |> Array.ofSeq)[y]) ||> Seq.fold (foldRegionUpToGap limit)
    (int64(2829680) * int64(4000000)) + int64(y)