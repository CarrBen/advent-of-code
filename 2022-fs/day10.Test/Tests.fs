namespace day10.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open day10


[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.Part1ExampleCycle1 () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let cycle = 1
        let register = day10.registerAt(input, cycle)
        Assert.AreEqual(1, register)

    [<TestMethod>]
    member this.Part1ExampleCycle2 () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let cycle = 2
        let register = day10.registerAt(input, cycle)
        Assert.AreEqual(1, register)

    [<TestMethod>]
    member this.Part1ExampleCycle3 () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let cycle = 3
        let register = day10.registerAt(input, cycle)
        Assert.AreEqual(1, register)

    [<TestMethod>]
    member this.Part1ExampleCycle4 () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let cycle = 4
        let register = day10.registerAt(input, cycle)
        Assert.AreEqual(4, register)

    [<TestMethod>]
    member this.Part1ExampleCycle5 () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let cycle = 5
        let register = day10.registerAt(input, cycle)
        Assert.AreEqual(4, register)

    [<TestMethod>]
    member this.Part1ExampleCycle6 () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let cycle = 6
        let register = day10.registerAt(input, cycle)
        Assert.AreEqual(-1, register)
        
    [<TestMethod>]
    member this.Part1Example1Cycle20 () =
        let input = IO.File.ReadAllText("../../../example1.txt")
        let cycle = 20
        let register = day10.registerAt(input, cycle)
        Assert.AreEqual(420, cycle * register)
        
    [<TestMethod>]
    member this.Part1Example1Cycle60 () =
        let input = IO.File.ReadAllText("../../../example1.txt")
        let cycle = 60
        let register = day10.registerAt(input, cycle)
        Assert.AreEqual(1140, cycle * register)
        
    [<TestMethod>]
    member this.Part1Example1Cycle100 () =
        let input = IO.File.ReadAllText("../../../example1.txt")
        let cycle = 100
        let register = day10.registerAt(input, cycle)
        Assert.AreEqual(1800, cycle * register)
        
    [<TestMethod>]
    member this.Part1Example1Cycle140 () =
        let input = IO.File.ReadAllText("../../../example1.txt")
        let cycle = 140
        let register = day10.registerAt(input, cycle)
        Assert.AreEqual(2940, cycle * register)
        
    [<TestMethod>]
    member this.Part1Example1Cycle180 () =
        let input = IO.File.ReadAllText("../../../example1.txt")
        let cycle = 180
        let register = day10.registerAt(input, cycle)
        Assert.AreEqual(2880, cycle * register)
        
    [<TestMethod>]
    member this.Part1Example1Cycle220 () =
        let input = IO.File.ReadAllText("../../../example1.txt")
        let cycle = 220
        let register = day10.registerAt(input, cycle)
        Assert.AreEqual(3960, cycle * register)
        
    [<TestMethod>]
    member this.Part1Example1 () =
        let input = IO.File.ReadAllText("../../../example1.txt")
        let part1 = day10.part1(input)
        Assert.AreEqual(13140, part1)
   
    [<TestMethod>]
    member this.Part2Cycle10 () =
        let input = IO.File.ReadAllText("../../../example1.txt")
        let part2 = day10.registerAt(input, 10)
        Assert.AreEqual(8, part2)
        
    [<TestMethod>]
    member this.Part2Example () =
        let input = IO.File.ReadAllText("../../../example1.txt")
        let part2 = day10.part2(input)
        Assert.AreEqual(1, part2)
