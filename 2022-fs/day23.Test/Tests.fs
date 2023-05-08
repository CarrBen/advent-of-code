namespace day23.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open day23


[<TestClass>]
type TestClass () =

    // [<TestMethod>]
    // member this.Part1Example () =
    //     let input = IO.File.ReadAllText("../../../example.txt")
    //     let part1 = day23.part1(input)
    //     Assert.AreEqual(110, part1)
        
    [<TestMethod>]
    member this.Part1ExampleSmall () =
        let input = IO.File.ReadAllText("../../../example_small.txt")
        let part1 = day23.part1(input)
        Assert.AreEqual(25, part1)
        
    [<TestMethod>]
    member this.Part1ExampleLarge () =
        let input = IO.File.ReadAllText("../../../example_large.txt")
        let part1 = day23.part1(input)
        Assert.AreEqual(110, part1)
        
    [<TestMethod>]
    member this.Part2ExampleSmall () =
        let input = IO.File.ReadAllText("../../../example_small.txt")
        let part2 = day23.part2(input)
        Assert.AreEqual(4, part2)
        
    [<TestMethod>]
    member this.Part2ExampleLarge () =
        let input = IO.File.ReadAllText("../../../example_large.txt")
        let part2 = day23.part2(input)
        Assert.AreEqual(20, part2)
