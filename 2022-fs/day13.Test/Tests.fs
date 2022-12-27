namespace day13.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open day13


[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.Part1CheckPairs () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let pairs = day13.parsePairs(input)
        let results = pairs |> Seq.map day13.pairValid |> List.ofSeq
        Assert.AreEqual(true, results[0])
        Assert.AreEqual(true, results[1])
        Assert.AreEqual(false, results[2])
        Assert.AreEqual(true, results[3])
        Assert.AreEqual(false, results[4])
        Assert.AreEqual(true, results[5])
        Assert.AreEqual(false, results[6])
        Assert.AreEqual(false, results[7])

    [<TestMethod>]
    member this.Part1Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part1 = day13.part1(input)
        Assert.AreEqual(13, part1)
        
    [<TestMethod>]
    member this.Part2Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part2 = day13.part2(input)
        Assert.AreEqual(140, part2)
