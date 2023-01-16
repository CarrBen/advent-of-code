namespace day20.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open day20


[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.MixExample1 () =
        let input = [4; 5; 6; 1; 7; 8; 9]
        let output = day20.mix(input |> List.map int64 |> List.indexed, (3, 1L))
        printfn "%A" output
        Assert.AreEqual([4; 5; 6; 7; 1; 8; 9] |> List.map int64, output |> List.map snd)
        
    [<TestMethod>]
    member this.MixExample2 () =
        let input = [4; -2; 5; 6; 7; 8; 9]
        let output = day20.mix(input |> List.map int64 |> List.indexed, (1, -2L))
        printfn "%A" output
        Assert.AreEqual([4; 5; 6; 7; 8; -2; 9] |> List.map int64, output |> List.map snd)
        
    [<TestMethod>]
    member this.Mix3 () =
        let input = [4; -2; 5; 6; 7; 8; 9]
        let output = day20.mix(input |> List.map int64 |> List.indexed, (2, 5L))
        printfn "%A" output
        Assert.AreEqual([4; 5; -2; 6; 7; 8; 9] |> List.map int64, output |> List.map snd)
        
    [<TestMethod>]
    member this.Mix4 () =
        let input = [4; 5; 6; 7; 8; 2; 9]
        let output = day20.mix(input |> List.map int64 |> List.indexed, (5, 2L))
        printfn "%A" output
        Assert.AreEqual([4; 2; 5; 6; 7; 8; 9] |> List.map int64, output |> List.map snd)
        
    [<TestMethod>]
    member this.Mix5 () =
        let input = [1; 2; 3; 1; 2; 3]
        let output = day20.mix(input |> List.map int64 |> List.indexed, (4, 2L))
        printfn "%A" output
        Assert.AreEqual([1; 2; 2; 3; 1; 3] |> List.map int64, output |> List.map snd)
        
    [<TestMethod>]
    member this.Mix6 () =
        let input = [1; 2; -3; 0; 3; 4; -2]
        let output = day20.mix(input |> List.map int64 |> List.indexed, (3, 0L))
        printfn "%A" output
        Assert.AreEqual(input |> List.map int64, output |> List.map snd)
        
    [<TestMethod>]
    member this.Mix7 () =
        let input = [1; 2; 8; 1]
        let output = day20.mix(input |> List.map int64 |> List.indexed, (2, 8L))
        printfn "%A" output
        Assert.AreEqual([1; 8; 2; 1] |> List.map int64, output |> List.map snd)
        
    [<TestMethod>]
    member this.Mix8 () =
        let input = [1; 2; 9; 1]
        let output = day20.mix(input |> List.map int64 |> List.indexed, (2, 9L))
        printfn "%A" output
        Assert.AreEqual([1; 2; 9; 1] |> List.map int64, output |> List.map snd)
        
    [<TestMethod>]
    member this.Mix9 () =
        let input = [1; 2; 1; 30; 1]
        let output = day20.mix(input |> List.map int64 |> List.indexed, (3, 30L))
        printfn "%A" output
        Assert.AreEqual([1; 30; 2; 1; 1] |> List.map int64, output |> List.map snd)
        
    [<TestMethod>]
    member this.Mix10 () =
        let input = [1; 2; 1; 31; 1]
        let output = day20.mix(input |> List.map int64 |> List.indexed, (3, 31L))
        printfn "%A" output
        Assert.AreEqual([1; 2; 31; 1; 1] |> List.map int64, output |> List.map snd)
        
    [<TestMethod>]
    member this.Mix11 () =
        let input = [1; 2; 1; 1; 32]
        let output = day20.mix(input |> List.map int64 |> List.indexed, (4, 32L))
        printfn "%A" output
        Assert.AreEqual([32; 1; 2; 1; 1] |> List.map int64, output |> List.map snd)
        
    [<TestMethod>]
    member this.Mix12 () =
        let input = [1; 2; 1; 32; 1]
        let output = day20.mix(input |> List.map int64 |> List.indexed, (3, 32L))
        printfn "%A" output
        Assert.AreEqual([1; 2; 1; 32; 1] |> List.map int64, output |> List.map snd)
        
    [<TestMethod>]
    member this.Mix13 () =
        let input = [1; 2; 32; 1; 1]
        let output = day20.mix(input |> List.map int64 |> List.indexed, (2, 32L))
        printfn "%A" output
        Assert.AreEqual([1; 2; 32; 1; 1] |> List.map int64, output |> List.map snd)
        
    [<TestMethod>]
    member this.Mix14 () =
        let input = [1; 32; 2; 1; 1]
        let output = day20.mix(input |> List.map int64 |> List.indexed, (1, 32L))
        printfn "%A" output
        Assert.AreEqual([1; 32; 2; 1; 1] |> List.map int64, output |> List.map snd)
        
    [<TestMethod>]
    member this.Mix15 () =
        let input = [1; 2; 1; 18; 1]
        let output = day20.mix(input |> List.map int64 |> List.indexed, (3, 18L))
        printfn "%A" output
        Assert.AreEqual([1; 18; 2; 1; 1] |> List.map int64, output |> List.map snd)
        
    [<TestMethod>]
    member this.Mix16 () =
        let input = [1; 2; -16; 1]
        let output = day20.mix(input |> List.map int64 |> List.indexed, (2, -16L))
        printfn "%A" output
        Assert.AreEqual([1; -16; 2; 1] |> List.map int64, output |> List.map snd)
        
    [<TestMethod>]
    member this.Get1000 () =
        let input = [1; 2; -3; 4; 0; 3; -2]
        let output = day20.get (input |> List.map int64 |> List.indexed) 1000
        printfn "%A" output
        Assert.AreEqual(4L, output)
        
    [<TestMethod>]
    member this.Get2000 () =
        let input = [1; 2; -3; 4; 0; 3; -2]
        let output = day20.get (input |> List.map int64 |> List.indexed) 2000
        printfn "%A" output
        Assert.AreEqual(-3L, output)
        
    [<TestMethod>]
    member this.Get3000 () =
        let input = [1; 2; -3; 4; 0; 3; -2]
        let output = day20.get (input |> List.map int64 |> List.indexed) 3000
        printfn "%A" output
        Assert.AreEqual(2L, output)

    [<TestMethod>]
    member this.Part1Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part1 = day20.part1(input)
        Assert.AreEqual(3L, part1)
        
    [<TestMethod>]
    member this.Part2Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part2 = day20.part2(input)
        Assert.AreEqual(1623178306L, part2)
