namespace day3.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open day3


[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.Part1Score_p () =
        Assert.AreEqual(16, day3.scoreItem('p'))
        
    [<TestMethod>]
    member this.Part1Score_L () =
        Assert.AreEqual(38, day3.scoreItem('L'))
        
    [<TestMethod>]
    member this.Part1Score_P () =
        Assert.AreEqual(42, day3.scoreItem('P'))
        
    [<TestMethod>]
    member this.Part1Score_v () =
        Assert.AreEqual(22, day3.scoreItem('v'))
    [<TestMethod>]
    member this.Part1Score_t () =
        Assert.AreEqual(20, day3.scoreItem('t'))
        
    [<TestMethod>]
    member this.Part1Score_s () =
        Assert.AreEqual(19, day3.scoreItem('s'))

    [<TestMethod>]
    member this.Part1Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part1 = day3.part1(input)
        Assert.AreEqual(157, part1)

    [<TestMethod>]
    member this.Part2ScoreGroup1 () =
        let input = """vJrwpWtwJgWrhcsFMMfFFhFp
jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL
PmmdzqPrVvPwwTWBwg"""
        let part2 = day3.scoreGroup(input.Split('\n'))
        Assert.AreEqual(18, part2)

    [<TestMethod>]
    member this.Part2ScoreGroup2 () =
        let input = """wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn
ttgJtRGJQctTZtZT
CrZsJsPPZsGzwwsLwLmpwMDw"""
        let part2 = day3.scoreGroup(input.Split('\n'))
        Assert.AreEqual(52, part2)

    [<TestMethod>]
    member this.Part2Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part2 = day3.part2(input)
        Assert.AreEqual(70, part2)
