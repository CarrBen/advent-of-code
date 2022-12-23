namespace day12.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open day12


[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.GridUpdate () =
        let grid = day12.update([[0; 0; 0]; [0; 0; 0]; [0; 0; 0]], (1, 1), 1)
        Assert.AreEqual([[0; 0; 0]; [0; 1; 0]; [0; 0; 0]], grid)

    [<TestMethod>]
    member this.Part1ParseLine () =
        let input = "SabqpoE"
        let map = day12.parse(input)
        Assert.AreEqual([[0; 0; 1; 16; 15; 14; 25]], map.grid)
        
    [<TestMethod>]
    member this.Part1ParseLines () =
        let input = "Sabcd\neqpoE"
        let map = day12.parse(input)
        Assert.AreEqual([[0; 0; 1; 2; 3]; [4; 16; 15; 14; 25]], map.grid)

    [<TestMethod>]
    member this.Part1ExamplePositions () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let map = day12.parse(input)
        Assert.AreEqual((0, 0), map.low)
        Assert.AreEqual((5, 2), map.high)

    [<TestMethod>]
    member this.Part1Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part1 = day12.part1(input)
        Assert.AreEqual(31, part1)
        
    [<TestMethod>]
    member this.Part2Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part2 = day12.part2(input)
        Assert.AreEqual(29, part2)
