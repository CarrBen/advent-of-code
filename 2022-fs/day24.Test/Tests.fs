namespace day24.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open day24


[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.Part1Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part1 = day24.part1(input)
        Assert.AreEqual(18, part1)
        
    [<TestMethod>]
    member this.ParseSimple () =
        let input = IO.File.ReadAllText("../../../simple.txt")
        let valley, blizzards = day24.parse(input)
        Assert.AreEqual(5, valley.width)
        Assert.AreEqual(5, valley.height)
        Assert.AreEqual(2, blizzards.Length)
        
    [<TestMethod>]
    member this.Step1Simple () =
        let input = IO.File.ReadAllText("../../../simple.txt")
        let valley, blizzards = day24.parse(input)
        let result = day24.stepBlizzards(valley, blizzards)
        Assert.AreEqual(2, result[0].x)
        Assert.AreEqual(2, result[0].y)
        
        Assert.AreEqual(4, result[1].x)
        Assert.AreEqual(5, result[1].y)
        
    [<TestMethod>]
    member this.Step2Simple () =
        let input = IO.File.ReadAllText("../../../simple.txt")
        let valley, blizzards = day24.parse(input)
        let first = day24.stepBlizzards(valley, blizzards)
        let result = day24.stepBlizzards(valley, first)
        Assert.AreEqual(3, result[0].x)
        Assert.AreEqual(2, result[0].y)
        
        Assert.AreEqual(4, result[1].x)
        Assert.AreEqual(1, result[1].y)
        
    [<TestMethod>]
    member this.Step5Simple () =
        let input = IO.File.ReadAllText("../../../simple.txt")
        let valley, blizzards = day24.parse(input)
        let first = day24.stepBlizzards(valley, blizzards)
        let second = day24.stepBlizzards(valley, first)
        let third = day24.stepBlizzards(valley, second)
        let fourth = day24.stepBlizzards(valley, third)
        let result = day24.stepBlizzards(valley, fourth)
        Assert.AreEqual(1, result[0].x)
        Assert.AreEqual(2, result[0].y)
        
        Assert.AreEqual(4, result[1].x)
        Assert.AreEqual(4, result[1].y)
        
    [<TestMethod>]
    member this.ParseExample () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let valley, blizzards = day24.parse(input)
        Assert.AreEqual(6, valley.width)
        Assert.AreEqual(4, valley.height)
        Assert.AreEqual(19, blizzards.Length)
        
    [<TestMethod>]
    member this.Part2Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part2 = day24.part2(input)
        Assert.AreEqual(54, part2)
