namespace day6.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open day6


[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.Part1Example1 () =
        let input = "mjqjpqmgbljsphdztnvjfqwrcgsmlb"
        let part1 = day6.part1(input)
        Assert.AreEqual(7, part1)
        

    [<TestMethod>]
    member this.Part1Example2 () =
        let input = "bvwbjplbgvbhsrlpgdmjqwftvncz"
        let part1 = day6.part1(input)
        Assert.AreEqual(5, part1)
        

    [<TestMethod>]
    member this.Part1Example3 () =
        let input = "nppdvjthqldpwncqszvftbrmjlhg"
        let part1 = day6.part1(input)
        Assert.AreEqual(6, part1)
        

    [<TestMethod>]
    member this.Part1Example4 () =
        let input = "nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg"
        let part1 = day6.part1(input)
        Assert.AreEqual(10, part1)
        

    [<TestMethod>]
    member this.Part1Example5 () =
        let input = "zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw"
        let part1 = day6.part1(input)
        Assert.AreEqual(11, part1)

    [<TestMethod>]
    member this.Part2Example1 () =
        let input = "mjqjpqmgbljsphdztnvjfqwrcgsmlb"
        let part2 = day6.part2(input)
        Assert.AreEqual(19, part2)
        

    [<TestMethod>]
    member this.Part2Example2 () =
        let input = "bvwbjplbgvbhsrlpgdmjqwftvncz"
        let part2 = day6.part2(input)
        Assert.AreEqual(23, part2)
        

    [<TestMethod>]
    member this.Part2Example3 () =
        let input = "nppdvjthqldpwncqszvftbrmjlhg"
        let part2 = day6.part2(input)
        Assert.AreEqual(23, part2)
        

    [<TestMethod>]
    member this.Part2Example4 () =
        let input = "nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg"
        let part2 = day6.part2(input)
        Assert.AreEqual(29, part2)
        

    [<TestMethod>]
    member this.part2Example5 () =
        let input = "zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw"
        let part2 = day6.part2(input)
        Assert.AreEqual(26, part2)
