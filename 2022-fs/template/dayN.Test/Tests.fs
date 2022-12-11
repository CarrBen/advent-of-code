namespace dayN.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open dayN


[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.Part1Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part1 = dayN.part1(input)
        Assert.AreEqual(, part1)
        
    [<TestMethod>]
    member this.Part2Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part2 = dayN.part2(input)
        Assert.AreEqual(, part2)
