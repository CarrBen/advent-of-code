namespace day4.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open day4


[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.Part1_NoOverlap () =
        let input = "11-15,16-19"
        let part1 = day4.checkLinePart1(input)
        Assert.AreEqual(0, part1)
        
    [<TestMethod>]
    member this.Part1_SomeOverlap () =
        let input = "11-15,15-19"
        let part1 = day4.checkLinePart1(input)
        Assert.AreEqual(0, part1)
        
    [<TestMethod>]
    member this.Part1_FullOverlap () =
        let input = "11-25,16-19"
        let part1 = day4.checkLinePart1(input)
        Assert.AreEqual(1, part1)
        
    [<TestMethod>]
    member this.Part1_EmptyLine () =
        let input = ""
        let part1 = day4.checkLinePart1(input)
        Assert.AreEqual(0, part1)

    [<TestMethod>]
    member this.Part1Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part1 = day4.part1(input)
        Assert.AreEqual(2, part1)


    [<TestMethod>]
    member this.Part2_NoOverlap () =
        let input = "11-15,16-19"
        let part2 = day4.checkLinePart2(input)
        Assert.AreEqual(0, part2)
        
    [<TestMethod>]
    member this.Part2_SomeOverlap () =
        let input = "11-15,15-19"
        let part2 = day4.checkLinePart2(input)
        Assert.AreEqual(1, part2)
        
    [<TestMethod>]
    member this.Part2_FullOverlap () =
        let input = "11-25,16-19"
        let part2 = day4.checkLinePart2(input)
        Assert.AreEqual(1, part2)
        
    [<TestMethod>]
    member this.Part2_EmptyLine () =
        let input = ""
        let part2 = day4.checkLinePart2(input)
        Assert.AreEqual(0, part2)
            
    [<TestMethod>]
    member this.Part2Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part2 = day4.part2(input)
        Assert.AreEqual(4, part2)
