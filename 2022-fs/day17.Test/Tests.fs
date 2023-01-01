namespace day17.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open day17


[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.Part1Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part1 = day17.part1(input, 2022)
        Assert.AreEqual(3068, part1)
        
    [<TestMethod>]
    member this.RockStartEmpty () =
        let grid = [|
            0b00000000uy;
            0b00000000uy;
            0b00000000uy;
            0b00000000uy;
        |]
        Assert.AreEqual(3, day17.rockStart(grid, 0))
        
    [<TestMethod>]
    member this.RockStartFlat () =
        let grid = [|
            0b11110000uy;
            0b00000000uy;
            0b00000000uy;
            0b00000000uy;
        |]
        Assert.AreEqual(4, day17.rockStart(grid, 0))
        
    [<TestMethod>]
    member this.RockStartCross () =
        let grid = [|
            0b00001000uy;
            0b00011100uy;
            0b00001000uy;
            0b00000000uy;
        |]
        Assert.AreEqual(6, day17.rockStart(grid, 0))
        
    [<TestMethod>]
    member this.UpdateEmptyGrid () =
        let targetGrid = [|
            0b00111100uy;
            0b00000000uy;
            0b00000000uy;
            0b00000000uy;
            0b00000000uy;
            0b00000000uy;
            0b00000000uy;
            0b00000000uy;
        |]
        let grid = 0b00000000uy |> Array.replicate 8
        let state = { State.y=0; rock=day17.rocks[0]; jetIndex=0; atRest=true }
        let updated = day17.updateGrid(grid, state)
        CollectionAssert.AreEqual(targetGrid, updated)
        
    [<TestMethod>]
    member this.UpdateSimpleGrid () =
        let targetGrid = [|
            0b00111100uy;
            0b00100000uy;
            0b00100000uy;
            0b00100000uy;
            0b00100000uy;
            0b00000000uy;
            0b00000000uy;
            0b00000000uy;
        |]
        let grid = [|
            0b00111100uy;
            0b00000000uy;
            0b00000000uy;
            0b00000000uy;
            0b00000000uy;
            0b00000000uy;
            0b00000000uy;
            0b00000000uy;
        |]
        let state = { State.y=1; rock=day17.rocks[3]; jetIndex=0; atRest=true }
        let updated = day17.updateGrid(grid, state)
        CollectionAssert.AreEqual(targetGrid, updated)
        
    [<TestMethod>]
    member this.UpdateComplexGrid () =
        let targetGrid = [|
            0b00111100uy;
            0b00100000uy;
            0b00100000uy;
            0b00100000uy;
            0b00110000uy;
            0b00111000uy;
            0b00010000uy;
            0b00000000uy;
        |]
        let grid = [|
            0b00111100uy;
            0b00100000uy;
            0b00100000uy;
            0b00100000uy;
            0b00100000uy;
            0b00000000uy;
            0b00000000uy;
            0b00000000uy;
        |]
        let state = { State.y=4; rock=day17.rocks[1]; jetIndex=0; atRest=true }
        let updated = day17.updateGrid(grid, state)
        CollectionAssert.AreEqual(targetGrid, updated)
        
    [<TestMethod>]
    member this.VerticalCollisionHit () =
        let grid = [|
            0b00111100uy;
            0b00100000uy;
            0b00100000uy;
            0b00100000uy;
            0b00110000uy;
            0b00111000uy;
            0b00010000uy;
            0b00000000uy;
            0b00000000uy;
            0b00000000uy;
            0b00000000uy;
            0b00000000uy;
        |]
        Assert.IsTrue(day17.rockCollision(6, day17.rocks[1], grid))
        
    [<TestMethod>]
    member this.VerticalCollisionMiss () =
        let grid = [|
            0b00111100uy;
            0b00100000uy;
            0b00100000uy;
            0b00100000uy;
            0b00110000uy;
            0b00111000uy;
            0b00010000uy;
            0b00000000uy;
            0b00000000uy;
            0b00000000uy;
            0b00000000uy;
            0b00000000uy;
        |]
        Assert.IsFalse(day17.rockCollision(7, day17.rocks[1], grid))
        
    [<TestMethod>]
    member this.Part2Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part2 = day17.part2(input, 1000000000000L)
        Assert.AreEqual(1514285714288L, part2)
