namespace day25.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open day25


[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.Part1Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part1 = day25.part1(input)
        Assert.AreEqual("2=-1=0", part1)
        
    [<DataTestMethod>]
    [<DataRow("1=-0-2", 1747L)>]
    [<DataRow("12111", 906L)>]
    [<DataRow("2=0=", 198L)>]
    [<DataRow("21", 11L)>]
    [<DataRow("2=01", 201L)>]
    [<DataRow("111", 31L)>]
    [<DataRow("20012", 1257L)>]
    [<DataRow("112", 32L)>]
    [<DataRow("1=-1=", 353L)>]
    [<DataRow("1-12", 107L)>]
    [<DataRow("12", 7L)>]
    [<DataRow("1=", 3L)>]
    [<DataRow("122", 37L)>]
    [<DataRow("1", 1L)>]
    [<DataRow("2", 2L)>]
    [<DataRow("1=", 3L)>]
    [<DataRow("1-", 4L)>]
    [<DataRow("10", 5L)>]
    [<DataRow("11", 6L)>]
    [<DataRow("12", 7L)>]
    [<DataRow("2=", 8L)>]
    [<DataRow("2-", 9L)>]
    [<DataRow("20", 10L)>]
    [<DataRow("1=0", 15L)>]
    [<DataRow("1-0", 20L)>]
    [<DataRow("1=11-2", 2022L)>]
    [<DataRow("1-0---0", 12345L)>]
    [<DataRow("1121-1110-1=0", 314159265L)>]
    member this.SnafuToDec (input:string, output: int64) =
        let result = day25.snafuToDec(input)
        Assert.AreEqual(output, result)
        
    [<DataTestMethod>]
    [<DataRow("1=-0-2", 1747L)>]
    [<DataRow("12111", 906L)>]
    [<DataRow("2=0=", 198L)>]
    [<DataRow("21", 11L)>]
    [<DataRow("2=01", 201L)>]
    [<DataRow("111", 31L)>]
    [<DataRow("20012", 1257L)>]
    [<DataRow("112", 32L)>]
    [<DataRow("1=-1=", 353L)>]
    [<DataRow("1-12", 107L)>]
    [<DataRow("12", 7L)>]
    [<DataRow("1=", 3L)>]
    [<DataRow("122", 37L)>]
    [<DataRow("1", 1L)>]
    [<DataRow("2", 2L)>]
    [<DataRow("1=", 3L)>]
    [<DataRow("1-", 4L)>]
    [<DataRow("10", 5L)>]
    [<DataRow("11", 6L)>]
    [<DataRow("12", 7L)>]
    [<DataRow("2=", 8L)>]
    [<DataRow("2-", 9L)>]
    [<DataRow("20", 10L)>]
    [<DataRow("1=0", 15L)>]
    [<DataRow("1-0", 20L)>]
    [<DataRow("1=11-2", 2022L)>]
    [<DataRow("1-0---0", 12345L)>]
    [<DataRow("1121-1110-1=0", 314159265L)>]
    member this.SnafuRoundtrip(input: string, output: int64) =
        let dec = day25.snafuToDec(input)
        let result = day25.decToSnafu(dec, 0)
        Assert.AreEqual(input, result)
        
    // [<TestMethod>]
    // member this.Part2Example () =
    //     let input = IO.File.ReadAllText("../../../example.txt")
    //     let part2 = day25.part2(input)
    //     Assert.AreEqual(, part2)
