namespace day22.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open day22


[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.Part1Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part1 = day22.part1(input)
        Assert.AreEqual(6032, part1)

    [<TestMethod>]
    member this.Part1MoveLeftOffEdgeIntoWall () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let map, moves = day22.parse(input)
        let (x, y), d = applyMove(map, 2, (0, 4), Direction.Left)
        Assert.AreEqual(Direction.Left, d)
        Assert.AreEqual(0, x)
        Assert.AreEqual(4, y)
        
    [<TestMethod>]
    member this.Part1MoveRightOffEdgeIntoWall () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let map, moves = day22.parse(input)
        let (x, y), d = applyMove(map, 2, (11, 2), Direction.Right)
        Assert.AreEqual(Direction.Right, d)
        Assert.AreEqual(11, x)
        Assert.AreEqual(2, y)
        
    [<TestMethod>]
    member this.Part1MoveDownOffEdgeIntoWall () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let map, moves = day22.parse(input)
        let (x, y), d = applyMove(map, 2, (3, 7), Direction.Down)
        Assert.AreEqual(Direction.Down, d)
        Assert.AreEqual(3, x)
        Assert.AreEqual(7, y)
        
    [<TestMethod>]
    member this.Part1MoveUpOffEdgeIntoWall () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let map, moves = day22.parse(input)
        let (x, y), d = applyMove(map, 2, (14, 8), Direction.Up)
        Assert.AreEqual(Direction.Up, d)
        Assert.AreEqual(14, x)
        Assert.AreEqual(8, y)
            
    [<TestMethod>]
    member this.Part1MoveLeftOffEdge () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let map, moves = day22.parse(input)
        let (x, y), d = applyMove(map, 2, (8, 3), Direction.Left)
        Assert.AreEqual(Direction.Left, d)
        Assert.AreEqual(10, x)
        Assert.AreEqual(3, y)
        
    [<TestMethod>]
    member this.Part1MoveRightOffEdge () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let map, moves = day22.parse(input)
        let (x, y), d = applyMove(map, 2, (11, 5), Direction.Right)
        Assert.AreEqual(Direction.Right, d)
        Assert.AreEqual(1, x)
        Assert.AreEqual(5, y)
        
    [<TestMethod>]
    member this.Part1MoveDownOffEdge () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let map, moves = day22.parse(input)
        let (x, y), d = applyMove(map, 2, (1, 7), Direction.Down)
        Assert.AreEqual(Direction.Down, d)
        Assert.AreEqual(1, x)
        Assert.AreEqual(5, y)
        
    [<TestMethod>]
    member this.Part1MoveUpOffEdge () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let map, moves = day22.parse(input)
        let (x, y), d = applyMove(map, 2, (1, 4), Direction.Up)
        Assert.AreEqual(Direction.Up, d)
        Assert.AreEqual(1, x)
        Assert.AreEqual(6, y)
        
    [<TestMethod>]
    member this.Part1MoveRightOffEdgeAndToWall () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let map, moves = day22.parse(input)
        let (x, y), d = applyMove(map, 20, (11, 5), Direction.Right)
        Assert.AreEqual(Direction.Right, d)
        Assert.AreEqual(7, x)
        Assert.AreEqual(5, y)

    [<TestMethod>]
    member this.Part1ParseEmptyRedirectNone () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let map, _ = day22.parseLines(input)
        let tile = parseEmpty(map, (0, 0))
        Assert.AreEqual(Tile.Redirect (None, None), tile)
        
    [<TestMethod>]
    member this.Part1ParseEmptyLeftEdge () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let map, _ = day22.parseLines(input)
        let tile = parseEmpty(map, (7, 1))
        Assert.AreEqual(Tile.Redirect (Some(Coord (11, 1)), None), tile)
        
    [<TestMethod>]
    member this.Part1ParseEmptyRightEdge () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let map, inst = day22.parseLines(input)
        printfn "%A" map
        printfn "%A" inst
        let tile = parseEmpty(map, (12, 1))
        Assert.AreEqual(Tile.Redirect (Some(Coord (8, 1)), None), tile)
        
    [<TestMethod>]
    member this.Part1ParseEmptyRightEdgeWall () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let map, inst = day22.parseLines(input)
        printfn "%A" map
        printfn "%A" inst
        let tile = parseEmpty(map, (12, 2))
        Assert.AreEqual(Tile.Redirect (None, None), tile)
        
    [<TestMethod>]
    member this.Part1ParseEmptyRightEdgeMapEdge () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let map, _ = day22.parseLines(input)
        let tile = parseEmpty(map, (12, 5))
        Assert.AreEqual(Tile.Redirect (Some(Coord (0, 5)), None), tile)
        
    [<TestMethod>]
    member this.Part1ParseEmptyTopEdge () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let map, _ = day22.parseLines(input)
        let tile = parseEmpty(map, (2, 3))
        Assert.AreEqual(Tile.Redirect (None, Some(Coord (2, 7))), tile)
        
    [<TestMethod>]
    member this.Part1ParseEmptyTopEdgeWall () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let map, _ = day22.parseLines(input)
        let tile = parseEmpty(map, (14, 7))
        Assert.AreEqual(Tile.Redirect (None, None), tile)
        
    [<TestMethod>]
    member this.Part1ParseEmptyBottomEdge () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let map, _ = day22.parseLines(input)
        let tile = parseEmpty(map, (2, 8))
        Assert.AreEqual(Tile.Redirect (None, Some(Coord (2, 4))), tile)
        
    [<TestMethod>]
    member this.Part1ParseEmptyBottomEdgeWall () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let map, _ = day22.parseLines(input)
        let tile = parseEmpty(map, (3, 8))
        Assert.AreEqual(Tile.Redirect (None, None), tile)
        
    [<TestMethod>]
    member this.Part1ParseEmptyBottomRightCorner () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let map, _ = day22.parseLines(input)
        let tile = parseEmpty(map, (7, 3))
        Assert.AreEqual(Tile.Redirect (Some(Coord (11, 3)), Some(Coord (7, 7))), tile)
        
    [<TestMethod>]
    member this.Part1ParseEmptyBottomLeftCorner () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let map, _ = day22.parseLines(input)
        let tile = parseEmpty(map, (12, 7))
        Assert.AreEqual(Tile.Redirect (Some(Coord (0, 7)), Some(Coord (12, 11))), tile)
        
    [<TestMethod>]
    member this.Part1ParseEmptyTopRightCorner () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let map, _ = day22.parseLines(input)
        let tile = parseEmpty(map, (7, 8))
        Assert.AreEqual(Tile.Redirect (Some(Coord (15, 8)), Some(Coord (7, 4))), tile)
        
    // [<TestMethod>]
    // member this.Part2Example () =
    //     let input = IO.File.ReadAllText("../../../example.txt")
    //     let part2 = day22.part2(input)
    //     Assert.AreEqual(, part2)
