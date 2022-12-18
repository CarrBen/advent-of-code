namespace day8.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open day8


[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.Part1Visibility1_1 () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let grid = day8.parseGrid(input)
        let vis = day8.testVisibility(grid, (1, 1))
        Assert.AreEqual(true, vis)
        
    [<TestMethod>]
    member this.Part1Visibility2_1 () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let grid = day8.parseGrid(input)
        let vis = day8.testVisibility(grid, (2, 1))
        Assert.AreEqual(true, vis)
        
    [<TestMethod>]
    member this.Part1Visibility3_1 () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let grid = day8.parseGrid(input)
        let vis = day8.testVisibility(grid, (3, 1))
        Assert.AreEqual(false, vis)
        
    [<TestMethod>]
    member this.Part1Visibility1_2 () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let grid = day8.parseGrid(input)
        let vis = day8.testVisibility(grid, (1, 2))
        Assert.AreEqual(true, vis)
        
    [<TestMethod>]
    member this.Part1Visibility2_2 () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let grid = day8.parseGrid(input)
        let vis = day8.testVisibility(grid, (2, 2))
        Assert.AreEqual(false, vis)
        
    [<TestMethod>]
    member this.Part1Visibility3_2 () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let grid = day8.parseGrid(input)
        let vis = day8.testVisibility(grid, (3, 2))
        Assert.AreEqual(true, vis)
        
    [<TestMethod>]
    member this.Part1Visibility1_3 () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let grid = day8.parseGrid(input)
        let vis = day8.testVisibility(grid, (1, 3))
        Assert.AreEqual(false, vis)
        
    [<TestMethod>]
    member this.Part1Visibility2_3 () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let grid = day8.parseGrid(input)
        let vis = day8.testVisibility(grid, (2, 3))
        Assert.AreEqual(true, vis)
        
    [<TestMethod>]
    member this.Part1Visibility3_3 () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let grid = day8.parseGrid(input)
        let vis = day8.testVisibility(grid, (3, 3))
        Assert.AreEqual(false, vis)
        
    [<TestMethod>]
    member this.Part1VisibilityTop () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let grid = day8.parseGrid(input)
        let vis = day8.testVisibility(grid, (2, 0))
        Assert.AreEqual(true, vis)
                
    [<TestMethod>]
    member this.Part1VisibilityLeft () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let grid = day8.parseGrid(input)
        let vis = day8.testVisibility(grid, (0, 2))
        Assert.AreEqual(true, vis)
                
    [<TestMethod>]
    member this.Part1VisibilityBot () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let grid = day8.parseGrid(input)
        let vis = day8.testVisibility(grid, (2, 4))
        Assert.AreEqual(true, vis)
                
    [<TestMethod>]
    member this.Part1VisibilityRight () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let grid = day8.parseGrid(input)
        let vis = day8.testVisibility(grid, (4, 2))
        Assert.AreEqual(true, vis)

    [<TestMethod>]
    member this.Part1Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part1 = day8.part1(input)
        Assert.AreEqual(21, part1)
        
    [<TestMethod>]
    member this.Part2ScenicScore2_1 () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let grid = day8.parseGrid(input)
        let score = day8.score(grid, (2, 1))
        Assert.AreEqual(4, score)
        
    [<TestMethod>]
    member this.Part2ScenicScore2_3 () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let grid = day8.parseGrid(input)
        let score = day8.score(grid, (2, 3))
        Assert.AreEqual(8, score)  
    
    [<TestMethod>]
    member this.Part2Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part2 = day8.part2(input)
        Assert.AreEqual(8, part2)
