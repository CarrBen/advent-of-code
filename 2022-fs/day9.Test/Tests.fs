namespace day9.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open day9


[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.Part1MoveParsing () =
        let input = "R 3\nU 2"
        let moves = day9.parseMoves(input)
        Assert.AreEqual([Right; Right; Right; Up; Up], moves)
        
    [<TestMethod>]
    member this.Part1MoveUp () =
        let pos = day9.applyMove((0, 0), Up)
        Assert.AreEqual((0, 1), pos)
        
    [<TestMethod>]
    member this.Part1MoveDown () =
        let pos = day9.applyMove((0, 0), Down)
        Assert.AreEqual((0, -1), pos)
        
    [<TestMethod>]
    member this.Part1MoveLeft () =
        let pos = day9.applyMove((0, 0), Left)
        Assert.AreEqual((-1, 0), pos)
        
    [<TestMethod>]
    member this.Part1MoveRight () =
        let pos = day9.applyMove((0, 0), Right)
        Assert.AreEqual((1, 0), pos)
        
    [<TestMethod>]
    member this.Part1TailRight () =
        let pos = day9.updateTail((2, 0), (0, 0))
        Assert.AreEqual((1, 0), pos)
        
    [<TestMethod>]
    member this.Part1TailDown () =
        let pos = day9.updateTail((0, -2), (0, 0))
        Assert.AreEqual((0, -1), pos)
        
    [<TestMethod>]
    member this.Part1TailDiagUp () =
        let pos = day9.updateTail((1, 2), (0, 0))
        Assert.AreEqual((1, 1), pos)
        
    [<TestMethod>]
    member this.Part1TailDiagRight () =
        let pos = day9.updateTail((2, 1), (0, 0))
        Assert.AreEqual((1, 1), pos)

    [<TestMethod>]
    member this.Part1Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part1 = day9.part1(input)
        Assert.AreEqual(13, part1)
        
    [<TestMethod>]
    member this.Part2Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part2 = day9.part2(input)
        Assert.AreEqual(1, part2)
        
    [<TestMethod>]
    member this.Part2Example1 () =
        let input = IO.File.ReadAllText("../../../example1.txt")
        let part2 = day9.part2(input)
        Assert.AreEqual(36, part2)
