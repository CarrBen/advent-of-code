namespace day16.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open day16


[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.Part1Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part1 = day16.part1(input)
        Assert.AreEqual(1651, part1)
        
    [<TestMethod>]
    member this.PruneNodes () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let nodes = day16.parseNodes input
        let pruned = day16.pruneNodes nodes
        Assert.AreEqual(10, nodes.Values |> Seq.length)
        Assert.AreEqual(7, pruned.Values |> Seq.length)
        
    [<TestMethod>]
    member this.Permutations () =
        let output = day16.permutations [1; 2; 3]
        for item in output do
            printfn "%A\n" item
        Assert.IsTrue(output |> Seq.contains [1; 2; 3])
        Assert.IsTrue(output |> Seq.contains [1; 3; 2])
        Assert.IsTrue(output |> Seq.contains [2; 1; 3])
        Assert.IsTrue(output |> Seq.contains [2; 3; 1])
        Assert.IsTrue(output |> Seq.contains [3; 2; 1])
        Assert.IsTrue(output |> Seq.contains [3; 1; 2])
        
    [<TestMethod>]
    member this.IncrementDFSSimple () =
        let output = day16.incrementDFS([0; 0; 0; 0], 4)
        Assert.AreEqual([0; 0; 1; 0], output)
        
    [<TestMethod>]
    member this.IncrementDFSCarry () =
        let output = day16.incrementDFS([0; 0; 1; 0], 4)
        Assert.AreEqual([0; 1; 0; 0], output)
        
    [<TestMethod>]
    member this.IncrementDFSMultiCarry () =
        let output = day16.incrementDFS([0; 2; 1; 0], 4)
        Assert.AreEqual([1; 0; 0; 0], output)
        
    [<TestMethod>]
    member this.IncrementDFSMultiple () =
        let mutable output = day16.incrementDFS([0; 0; 0; 0], 4)
        Assert.AreEqual([0; 0; 1; 0], output)
        output <- day16.incrementDFS(output, 4)
        Assert.AreEqual([0; 1; 0; 0], output)
        output <- day16.incrementDFS(output, 4)
        Assert.AreEqual([0; 1; 1; 0], output)
        output <- day16.incrementDFS(output, 4)
        Assert.AreEqual([0; 2; 0; 0], output)
        output <- day16.incrementDFS(output, 4)
        Assert.AreEqual([0; 2; 1; 0], output)
        output <- day16.incrementDFS(output, 4)
        Assert.AreEqual([1; 0; 0; 0], output)
        output <- day16.incrementDFS(output, 4)
        Assert.AreEqual([1; 0; 1; 0], output)
        output <- day16.incrementDFS(output, 4)
        Assert.AreEqual([1; 1; 0; 0], output)
        output <- day16.incrementDFS(output, 4)
        Assert.AreEqual([1; 1; 1; 0], output)
        output <- day16.incrementDFS(output, 4)
        Assert.AreEqual([1; 2; 0; 0], output)
        output <- day16.incrementDFS(output, 4)
        Assert.AreEqual([1; 2; 1; 0], output)
        output <- day16.incrementDFS(output, 4)
        Assert.AreEqual([2; 0; 0; 0], output)
        
    [<TestMethod>]
    member this.IncrementDFSTerminates () =
        let output = day16.incrementDFS([3; 2; 1; 0], 4)
        Assert.AreEqual([0; 0; 0; 0], output)
     
    [<TestMethod>]
    member this.NodeDistMultipath () =
        let nodes = Map([("AA", { Node.name="AA"; flow=0; connections=[("BB", 1); ("CC", 5)] }); ("BB", { Node.name="BB"; flow=2; connections=[("AA", 1); ("CC", 1)] }); ("CC", { Node.name="CC"; flow=3; connections=[("BB", 1); ("AA",5)] })])
        let dist = day16.nodeDist(nodes, nodes["AA"], nodes["CC"], [])
        Assert.AreEqual(2, dist)
        
    [<TestMethod>]
    member this.NodeDistAABB () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let nodes = day16.parseNodes input
        let dist = day16.nodeDist(nodes, nodes["AA"], nodes["BB"], [])
        Assert.AreEqual(1, dist)
        
    [<TestMethod>]
    member this.NodeDistAACC () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let nodes = day16.parseNodes input
        let dist = day16.nodeDist(nodes, nodes["AA"], nodes["CC"], [])
        Assert.AreEqual(2, dist)
        
    [<TestMethod>]
    member this.NodeDistAAJJ () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let nodes = day16.parseNodes input
        let dist = day16.nodeDist(nodes, nodes["AA"], nodes["JJ"], [])
        Assert.AreEqual(2, dist)
        
    [<TestMethod>]
    member this.NodeDistAAHH () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let nodes = day16.parseNodes input
        let dist = day16.nodeDist(nodes, nodes["AA"], nodes["HH"], [])
        Assert.AreEqual(5, dist)
        
    [<TestMethod>]
    member this.PrunedNodeDistAABB () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let nodes = day16.pruneNodes(day16.parseNodes input)
        let dist = day16.nodeDist(nodes, nodes["AA"], nodes["BB"], [])
        Assert.AreEqual(1, dist)
        
    [<TestMethod>]
    member this.PrunedNodeDistAACC () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let nodes = day16.pruneNodes(day16.parseNodes input)
        let dist = day16.nodeDist(nodes, nodes["AA"], nodes["CC"], [])
        Assert.AreEqual(2, dist)
        
    [<TestMethod>]
    member this.PrunedNodeDistAAJJ () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let nodes = day16.pruneNodes(day16.parseNodes input)
        let dist = day16.nodeDist(nodes, nodes["AA"], nodes["JJ"], [])
        Assert.AreEqual(2, dist)
        
    [<TestMethod>]
    member this.PrunedNodeDistAAHH () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let nodes = day16.pruneNodes(day16.parseNodes input)
        let dist = day16.nodeDist(nodes, nodes["AA"], nodes["HH"], [])
        Assert.AreEqual(5, dist)
        
    // [<TestMethod>]
    // member this.Part2Example () =
    //     let input = IO.File.ReadAllText("../../../example.txt")
    //     let part2 = day16.part2(input)
    //     Assert.AreEqual(, part2)
