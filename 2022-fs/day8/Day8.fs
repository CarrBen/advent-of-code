module day8

let testVisibility (grid: int[][], (x: int, y: int)): bool =
    if x = 0 || y = 0 || x = (grid[0].Length - 1) || y = (grid.Length - 1) then true else
    let treeHeight = grid[y][x]
    let left = { 0 .. x-1 } |> Seq.map (fun tx -> grid[y][tx] < treeHeight) |> Seq.forall (fun item -> item)
    let right = { x+1 .. grid[0].Length - 1 } |> Seq.map (fun tx -> grid[y][tx] < treeHeight) |> Seq.forall (fun item -> item)
    let up = { 0 .. y-1 } |> Seq.map (fun ty -> grid[ty][x] < treeHeight) |> Seq.forall (fun item -> item)
    let down = { y+1 .. grid.Length - 1 } |> Seq.map (fun ty -> grid[ty][x] < treeHeight) |> Seq.forall (fun item -> item)
    [left; right; up; down] |> Seq.exists (fun item -> item)

let countView (input: seq<bool>): int =
    let filtered = input |> Seq.takeWhile (fun item -> item)
    if (filtered |> Seq.length) = (input |> Seq.length) then input |> Seq.length
    else (filtered |> Seq.length) + 1

let score (grid: int[][], (x: int, y: int)): int =
    if x = 0 || y = 0 || x = (grid[0].Length - 1) || y = (grid.Length - 1) then 0 else // Trees on edge have 1 viewing distance zero, which will make the overall score zero
    let treeHeight = grid[y][x]
    let left = { x-1 .. -1 .. 0 } |> Seq.map (fun tx -> grid[y][tx] < treeHeight) |> countView
    let right = { x+1 .. grid[0].Length - 1 } |> Seq.map (fun tx -> grid[y][tx] < treeHeight) |> countView
    let up = { y-1 .. -1 .. 0 } |> Seq.map (fun ty -> grid[ty][x] < treeHeight) |> countView
    let down = { y+1 .. grid.Length - 1 } |> Seq.map (fun ty -> grid[ty][x] < treeHeight) |> countView
    [left; right; up; down] |> Seq.fold (fun acc vis -> vis * acc) 1
    
let parseGrid (input: string): int[][] =
    input.Split('\n') |> Seq.filter (fun line -> line.Length > 0) |> Seq.map (fun line -> line |> Seq.map int |> Array.ofSeq) |> Array.ofSeq

let part1 (input : string) =
    let grid = parseGrid input
    { 0 .. grid.Length - 1 } |> Seq.map (fun (y: int) -> { 0 .. grid[0].Length - 1 } |> Seq.map (fun x -> testVisibility(grid, (x, y))) |> Seq.sumBy (fun item -> if item then 1 else 0)) |> Seq.sum
    
let part2 (input : string) =
    let grid = parseGrid input
    { 0 .. grid.Length - 1 } |> Seq.map (fun (y: int) -> { 0 .. grid[0].Length - 1 } |> Seq.map (fun x -> score(grid, (x, y))) |> Seq.max) |> Seq.max