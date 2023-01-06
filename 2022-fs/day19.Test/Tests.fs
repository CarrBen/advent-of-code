namespace day19.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open day19


[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.Part1ExampleBlueprint1 () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let lines = input.Split('\n')
        let blueprint = day19.parseBlueprint(lines[0])
        let state = day19.scoreBlueprint(blueprint, State.initialPart1(), State.initialPart1())
        printfn "%A %A" blueprint state
        Assert.AreEqual(9, blueprint.id * state.resources.geode)

    [<TestMethod>]
    member this.Part1ExampleBlueprint2 () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let lines = input.Split('\n')
        let blueprint = day19.parseBlueprint(lines[1])
        let state = day19.scoreBlueprint(blueprint, State.initialPart1(), State.initialPart1())
        printfn "%A %A" blueprint state
        Assert.AreEqual(24, blueprint.id * state.resources.geode)
        
    [<TestMethod>]
    member this.ResourcesCanAfford () =
        let pairs = [
            ((10,0,0,0),(1,0,0,0));
            ((0,10,0,0),(0,1,0,0));
            ((0,0,10,0),(0,0,1,0));
            ((0,0,0,10),(0,0,0,1));
            ((10,10,10,10),(1,0,0,0));
            ((10,10,10,10),(0,1,0,0));
            ((10,10,10,10),(0,0,1,0));
            ((10,10,10,10),(0,0,0,1));
            ((10,10,10,10),(1,1,0,0));
            ((10,10,10,10),(0,1,1,0));
            ((10,10,10,10),(0,0,1,1));
            ((10,10,10,10),(1,1,1,1));
        ]
        for a, b in pairs do
            let ra = day19.Resources.make(a)
            let rb = day19.Resources.make(b)
            Assert.IsTrue(ra.canAfford(rb))
            
    [<TestMethod>]
    member this.ResourcesCannotAfford () =
        let pairs = [
            ((1,0,0,0),(10,0,0,0));
            ((0,1,0,0),(0,10,0,0));
            ((0,0,1,0),(0,0,10,0));
            ((0,0,0,1),(0,0,0,10));
            ((1,1,1,1),(10,0,0,0));
            ((1,1,1,1),(0,10,0,0));
            ((1,1,1,1),(0,0,10,0));
            ((1,1,1,1),(0,0,0,10));
            ((1,1,1,1),(10,10,0,0));
            ((1,1,1,1),(0,10,10,0));
            ((1,1,1,1),(0,0,10,10));
            ((1,1,1,1),(10,10,10,10));
        ]
        for a, b in pairs do
            let ra = day19.Resources.make(a)
            let rb = day19.Resources.make(b)
            Assert.IsFalse(ra.canAfford(rb))
        
    [<TestMethod>]
    member this.ResourcesIncrement () =
        let a = day19.Resources.make(0,2,4,6)
        let b = day19.Resources.make(1,3,5,7)
        Assert.IsTrue(a.increment(1,1,1,1) = b)
        
    [<TestMethod>]
    member this.ResourcesSubtract () =
        let a = day19.Resources.make(0,2,4,6)
        let b = day19.Resources.make(1,3,5,7)
        let c = day19.Resources.make(1,1,1,1)
        Assert.AreEqual(a, b - c)

    [<TestMethod>]
    member this.Part1Example () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let part1 = day19.part1(input)
        Assert.AreEqual(33, part1)
        
    [<TestMethod>]
    member this.CanBeat1Remaining () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let lines = input.Split('\n')
        let blueprint = day19.parseBlueprint(lines[0])
        let state = { remaining=1; resources=Resources.make(0,0,0,4); geodeBots=1; lastResources=Resources.make(0,0,0,0); oreBots=0; clayBots=0; obsidianBots=0; actions=[] }
        let bench = { remaining=0; resources=Resources.make(0,0,0,5); geodeBots=1; lastResources=Resources.make(0,0,0,0); oreBots=0; clayBots=0; obsidianBots=0; actions=[] }
        Assert.AreEqual(true, day19.canBeat(blueprint, state, bench))
        
    [<TestMethod>]
    member this.CantBeat1Remaining () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let lines = input.Split('\n')
        let blueprint = day19.parseBlueprint(lines[0])
        let state = { remaining=1; resources=Resources.make(0,0,0,3); geodeBots=1; lastResources=Resources.make(0,0,0,0); oreBots=0; clayBots=0; obsidianBots=0; actions=[] }
        let bench = { remaining=0; resources=Resources.make(0,0,0,5); geodeBots=1; lastResources=Resources.make(0,0,0,0); oreBots=0; clayBots=0; obsidianBots=0; actions=[] }
        Assert.AreEqual(false, day19.canBeat(blueprint, state, bench))

    [<TestMethod>]
    member this.Part2ExampleBlueprint1 () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let lines = input.Split('\n')
        let blueprint = day19.parseBlueprint(lines[0])
        let state = day19.scoreBlueprint(blueprint, State.initialPart2(), State.initialPart2())
        printfn "%A %A" blueprint state
        Assert.AreEqual(56, state.resources.geode)

    [<TestMethod>]
    member this.Part2ExampleBlueprint2 () =
        let input = IO.File.ReadAllText("../../../example.txt")
        let lines = input.Split('\n')
        let blueprint = day19.parseBlueprint(lines[1])
        let state = day19.scoreBlueprint(blueprint, State.initialPart2(), State.initialPart2())
        printfn "%A %A" blueprint state
        Assert.AreEqual(62, state.resources.geode)