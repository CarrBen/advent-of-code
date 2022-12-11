namespace day2.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open day2


[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.Part1FirstRoundExample () =
        let input = "A Y"
        let round = day2.scoreRoundPart1(input)
        Assert.AreEqual(8, round)
        
    [<TestMethod>]
    member this.Part1SecondRoundExample () =
        let input = "B X"
        let round = day2.scoreRoundPart1(input)
        Assert.AreEqual(1, round)
        
    [<TestMethod>]
    member this.Part1ThirdRoundExample () =
        let input = "C Z"
        let round = day2.scoreRoundPart1(input)
        Assert.AreEqual(6, round)

    [<TestMethod>]
    member this.Part1Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part1 = day2.part1(input)
        Assert.AreEqual(15, part1)


    [<TestMethod>]
    member this.Part2FirstRoundExample () =
        let input = "A Y"
        let round = day2.scoreRoundPart2(input)
        Assert.AreEqual(4, round)
        
    [<TestMethod>]
    member this.Part2SecondRoundExample () =
        let input = "B X"
        let round = day2.scoreRoundPart2(input)
        Assert.AreEqual(1, round)
        
    [<TestMethod>]
    member this.Part2ThirdRoundExample () =
        let input = "C Z"
        let round = day2.scoreRoundPart2(input)
        Assert.AreEqual(7, round)

    [<TestMethod>]
    member this.Part2Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part1 = day2.part2(input)
        Assert.AreEqual(12, part1)