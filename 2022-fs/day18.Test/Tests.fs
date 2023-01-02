namespace day18.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open day18


[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.Part1SmallExample () =
        let input = "1,1,1\n2,1,1\n"
        let part1 = day18.part1(input)
        Assert.AreEqual(10, part1)
        
    [<TestMethod>]
    member this.Part1SmallExampleScorePoint () =
        let input = "1,1,1\n2,1,1\n"
        let world = day18.parseWorld(input)
        let score = day18.scorePoint(world.grid) (1s,1s,1s)
        Assert.AreEqual(5, score)

    [<TestMethod>]
    member this.Part1LargeExample () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part1 = day18.part1(input)
        Assert.AreEqual(64, part1)
        
    [<TestMethod>]
    member this.Part2LargeExample () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part2 = day18.part2(input)
        Assert.AreEqual(58, part2)
