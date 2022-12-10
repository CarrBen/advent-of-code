namespace day1.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open day1


[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.Part1Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part1 = day1.part1(input)
        Assert.AreEqual(24000, part1)
        
    [<TestMethod>]
    member this.Part2Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part2 = day1.part2(input)
        Assert.AreEqual(45000, part2)
