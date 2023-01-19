namespace day21.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open day21


[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.Part1Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part1 = day21.part1(input)
        Assert.AreEqual(152L, part1)
        
    [<TestMethod>]
    member this.Part2Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part2 = day21.part2(input)
        Assert.AreEqual(301L, part2)
