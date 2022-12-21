namespace day11.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open day11


[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.Part1ExampleParse () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let monkeys = day11.parseMonkeys(input)
        Assert.AreEqual([79L; 98], monkeys[0].levels)
        Assert.AreEqual([54L; 65; 75; 74], monkeys[1].levels)
        Assert.AreEqual([79L; 60; 97], monkeys[2].levels)
        Assert.AreEqual([74L], monkeys[3].levels)

    [<TestMethod>]
    member this.Part1ExampleRound1 () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let monkeys = day11.parseMonkeys(input)
        let round1 = day11.runRoundPart1(monkeys)
        Assert.AreEqual([20L; 23; 27; 26], round1[0].levels)
        Assert.AreEqual([2080L; 25; 167; 207; 401; 1046], round1[1].levels)
        Assert.AreEqual(Seq.empty |> Seq.cast<int64> |> List.ofSeq, round1[2].levels)
        Assert.AreEqual(Seq.empty |> Seq.cast<int64> |> List.ofSeq, round1[3].levels)

    [<TestMethod>]
    member this.Part1ExampleRound20 () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let monkeys = day11.parseMonkeys(input)
        let round20 = (monkeys, { 0 .. 19 } ) ||> Seq.fold (fun m i -> day11.runRoundPart1(m))
        Assert.AreEqual(101, round20[0].inspectionCount)
        Assert.AreEqual(95, round20[1].inspectionCount)
        Assert.AreEqual(7, round20[2].inspectionCount)
        Assert.AreEqual(105, round20[3].inspectionCount)

    [<TestMethod>]
    member this.Part1Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part1 = day11.part1(input)
        Assert.AreEqual(10605, part1)
        
    [<TestMethod>]
    member this.Part2ExampleRound1 () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let monkeys = day11.parseMonkeys(input)
        let round1 = day11.runRoundPart2(monkeys)
        Assert.AreEqual(2, round1[0].inspectionCount)
        Assert.AreEqual(4, round1[1].inspectionCount)
        Assert.AreEqual(3, round1[2].inspectionCount)
        Assert.AreEqual(6, round1[3].inspectionCount)
        
    [<TestMethod>]
    member this.Part2ExampleRound20 () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let monkeys = day11.parseMonkeys(input)
        let round20 = (monkeys, { 0 .. 19 } ) ||> Seq.fold (fun m i -> day11.runRoundPart2(m))
        Assert.AreEqual(99, round20[0].inspectionCount)
        Assert.AreEqual(97, round20[1].inspectionCount)
        Assert.AreEqual(8, round20[2].inspectionCount)
        Assert.AreEqual(103, round20[3].inspectionCount)
        
    [<TestMethod>]
    member this.Part2ExampleRound1000 () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let monkeys = day11.parseMonkeys(input)
        let round20 = (monkeys, { 0 .. 999 } ) ||> Seq.fold (fun m i -> day11.runRoundPart2(m))
        Assert.AreEqual(5204, round20[0].inspectionCount)
        Assert.AreEqual(4792, round20[1].inspectionCount)
        Assert.AreEqual(199, round20[2].inspectionCount)
        Assert.AreEqual(5192, round20[3].inspectionCount)
        
    [<TestMethod>]
    member this.Part2Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part2 = day11.part2(input)
        Assert.AreEqual(2713310158L, part2)
