namespace day7.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open day7


[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.Part1DirESize () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let tree = day7.parseTree(input)
        let part1 = day7.targetSize(tree, "e")
        Assert.AreEqual(584u, part1)

    [<TestMethod>]
    member this.Part1DirASize () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let tree = day7.parseTree(input)
        let part1 = day7.targetSize(tree, "a")
        Assert.AreEqual(94853u, part1)
        
    [<TestMethod>]
    member this.Part1DirDSize () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let tree = day7.parseTree(input)
        let part1 = day7.targetSize(tree, "d")
        Assert.AreEqual(24933642u, part1)
        
    [<TestMethod>]
    member this.Part1DirRootSize () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let tree = day7.parseTree(input)
        let part1 = day7.targetSize(tree, "")
        Assert.AreEqual(48381165u, part1)

    [<TestMethod>]
    member this.Part1Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part1 = day7.part1(input)
        Assert.AreEqual(95437u, part1)
        
    [<TestMethod>]
    member this.Part2Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part2 = day7.part2(input)
        Assert.AreEqual(24933642u, part2)
