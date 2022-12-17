namespace day5.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open day5


[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.ExampleLoadMoves () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let crates, moves = day5.splitInput input
        let part1 = day5.parseMoves moves
        Assert.AreEqual([{ day5.Move.Count = 1; From = 1; To = 0 }; { day5.Move.Count = 3; From = 0; To = 2 }; { day5.Move.Count = 2; From = 1; To = 0 }; { day5.Move.Count = 1; From = 0; To = 1 }], part1)

    [<TestMethod>]
    member this.ExampleLoadCrates () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let crates, moves = day5.splitInput input
        let part1 = day5.parseCrates crates 
        Assert.AreEqual([['N'; 'Z']; ['D'; 'C'; 'M']; ['P']], part1)

    [<TestMethod>]
    member this.Part1Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part1 = day5.part1(input)
        Assert.AreEqual("CMZ", part1)
        
    [<TestMethod>]
    member this.Part2Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part2 = day5.part2(input)
        Assert.AreEqual("MCD", part2)
