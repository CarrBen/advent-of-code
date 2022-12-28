namespace day15.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open day15


[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.Parse () =
        let input = "Sensor at x=12, y=14: closest beacon is at x=10, y=16"
        let sensor = day15.parseSensor(input)
        Assert.AreEqual({ x=12; y=14 }, sensor.loc)
        Assert.AreEqual({ x=10; y=16 }, sensor.beacon)
        
    [<TestMethod>]
    member this.Distance () =
        let dist = day15.dist({ x=8; y=7 }, { x=2; y=10 })
        Assert.AreEqual(9, dist)

    [<TestMethod>]
    member this.RowRegionNoneBelow () =
        let input = "Sensor at x=12, y=14: closest beacon is at x=10, y=16"
        let sensor = day15.parseSensor(input)
        let region = day15.rowRegion(20, sensor)
        Assert.AreEqual(None, region)
        
    [<TestMethod>]
    member this.RowRegionNoneAbove () =
        let input = "Sensor at x=12, y=14: closest beacon is at x=10, y=16"
        let sensor = day15.parseSensor(input)
        let region = day15.rowRegion(7, sensor)
        Assert.AreEqual(None, region)
        
    [<TestMethod>]
    member this.RowRegionEdgeBelow () =
        let input = "Sensor at x=12, y=14: closest beacon is at x=10, y=16"
        let sensor = day15.parseSensor(input)
        let region = day15.rowRegion(18, sensor).Value
        Assert.AreEqual(sensor.loc.x, region.left)
        Assert.AreEqual(sensor.loc.x, region.right)
        
    [<TestMethod>]
    member this.RowRegionEdgeAbove () =
        let input = "Sensor at x=12, y=14: closest beacon is at x=10, y=16"
        let sensor = day15.parseSensor(input)
        let region = day15.rowRegion(10, sensor).Value
        Assert.AreEqual(sensor.loc.x, region.left)
        Assert.AreEqual(sensor.loc.x, region.right)
        
    [<TestMethod>]
    member this.RowRegionNearBelow () =
        let input = "Sensor at x=12, y=14: closest beacon is at x=10, y=16"
        let sensor = day15.parseSensor(input)
        let region = day15.rowRegion(15, sensor).Value
        Assert.AreEqual(sensor.loc.x - 3, region.left)
        Assert.AreEqual(sensor.loc.x + 3, region.right)
        
    [<TestMethod>]
    member this.RowRegionNearAbove () =
        let input = "Sensor at x=12, y=14: closest beacon is at x=10, y=16"
        let sensor = day15.parseSensor(input)
        let region = day15.rowRegion(13, sensor).Value
        Assert.AreEqual(sensor.loc.x - 3, region.left)
        Assert.AreEqual(sensor.loc.x + 3, region.right)
        
    [<TestMethod>]
    member this.RowRegionExact () =
        let input = "Sensor at x=12, y=14: closest beacon is at x=10, y=16"
        let sensor = day15.parseSensor(input)
        let region = day15.rowRegion(sensor.loc.y, sensor).Value
        Assert.AreEqual(sensor.loc.x - 4, region.left)
        Assert.AreEqual(sensor.loc.x + 4, region.right)

    [<TestMethod>]
    member this.Part1Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part1 = day15.part1(input, 10)
        Assert.AreEqual(26, part1)
        
    [<TestMethod>]
    member this.Part2Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part2 = day15.part2(input, 20)
        Assert.AreEqual(56000011L, part2)
