namespace day14t.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open day14

open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running


[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.SimpleWallContains () =
        let input = "490,5 -> 500,5"
        let wall = day14.Wall.parse(input)
        Assert.IsTrue(wall.Contains({ x = 495; y = 5 }))
        
        Assert.IsFalse(wall.Contains({ x = 495; y = 6 }))
        Assert.IsFalse(wall.Contains({ x = 495; y = 4 }))
        Assert.IsFalse(wall.Contains({ x = 485; y = 4 }))
        Assert.IsFalse(wall.Contains({ x = 485; y = 6 }))
        Assert.IsFalse(wall.Contains({ x = 485; y = 5 }))
        Assert.IsFalse(wall.Contains({ x = 505; y = 4 }))
        Assert.IsFalse(wall.Contains({ x = 505; y = 6 }))
        Assert.IsFalse(wall.Contains({ x = 505; y = 5 }))

    [<TestMethod>]
    member this.Part1Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part1 = day14.part1(input)
        Assert.AreEqual(24, part1)
        
    [<TestMethod>]
    member this.Part2Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part2 = day14.part2(input)
        Assert.AreEqual(93, part2)

module Day14Bench =
    type SandBench() =
        [<Benchmark(Baseline=true)>]
        member this.SandV1 () = day14.containsSandV1({ x=500; y=0 }, (seq { 0 .. 500 } |> Seq.map (fun x -> { x=x; y=0 }) |> Array.ofSeq))
        
    type WallBench() =
        [<Benchmark(Baseline=true)>]
        member this.WallV1 () = day14.containsWallV1({ x=500; y=0 }, (seq { 0 .. 500 } |> Seq.map (fun x -> Wall([{ x=x; y=0 }; { x=x+20; y=0 }])) |> Array.ofSeq))
        
    [<EntryPoint>]
    let main argv =
        BenchmarkRunner.Run<SandBench>() |> ignore
        BenchmarkRunner.Run<WallBench>() |> ignore
        0 // return an integer exit code