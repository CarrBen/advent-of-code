module day19

let regexGroup (result: System.Text.RegularExpressions.Match, name: string): string =
    let index = result.Groups.Keys |> Seq.tryFindIndex (fun s -> s = name)
    if index.IsNone then "0" else
    (result.Groups.Values |> Seq.item index.Value).Value
    
let regexCount (input: string, name: string): int =
    let regex = new System.Text.RegularExpressions.Regex($"(?<count>\d+) {name}")
    let m = regex.Match(input)
    let group = regexGroup(m, "count")
    int(group)

type Resources = { ore: int; clay: int; obsidian: int; geode: int } with
    member r.increment (ore: int, clay: int, obs: int, geode: int): Resources =
        { Resources.ore=(r.ore + ore); clay=(r.clay + clay); obsidian=(r.obsidian + obs); geode=(r.geode + geode) }
    member r.canAfford (cost: Resources): bool =
        (r.ore >= cost.ore) && (r.clay >= cost.clay) && (r.obsidian >= cost.obsidian) && (r.geode >= cost.geode)
    static member make (ore:int, clay:int, obs:int, geode: int): Resources =
        { Resources.ore=ore; clay=clay; obsidian=obs; geode=geode; }
    static member regex (input: string): Resources =
        Resources.make(regexCount(input, "ore"), regexCount(input, "clay"), regexCount(input, "obsidian"), regexCount(input, "geode"))
    static member (-) (a: Resources, b: Resources): Resources =
        { Resources.ore=(a.ore - b.ore); clay=(a.clay - b.clay); obsidian=(a.obsidian - b.obsidian); geode=(a.geode - b.geode) }
type Blueprint = { id: int; oreCost: Resources; clayCost: Resources; obsidianCost: Resources; geodeCost: Resources }
type Action = Wait | BuyOreBot | BuyClayBot | BuyObsidianBot | BuyGeodeBot
type State = { resources: Resources; lastResources: Resources; remaining: int; oreBots: int; clayBots: int; obsidianBots: int; geodeBots: int; actions: Action list } with
    static member initialPart1 (): State =
        { State.remaining=24; resources=Resources.make(0,0,0,0); lastResources=Resources.make(0,0,0,0); oreBots=1; clayBots=0; obsidianBots=0; geodeBots=0; actions=[] }
    static member initialPart2 (): State =
        { State.remaining=32; resources=Resources.make(0,0,0,0); lastResources=Resources.make(0,0,0,0); oreBots=1; clayBots=0; obsidianBots=0; geodeBots=0; actions=[] }
    member s.needOreBots (bp: Blueprint): bool =
        s.oreBots < bp.oreCost.ore || s.oreBots < bp.clayCost.ore || s.oreBots < bp.obsidianCost.ore || s.oreBots < bp.geodeCost.ore
    member s.needClayBots (bp: Blueprint): bool =
        s.clayBots < bp.oreCost.clay || s.clayBots < bp.clayCost.clay || s.clayBots < bp.obsidianCost.clay || s.clayBots < bp.geodeCost.clay
    member s.needObsBots (bp: Blueprint): bool =
        s.obsidianBots < bp.oreCost.obsidian || s.obsidianBots < bp.clayCost.obsidian || s.obsidianBots < bp.obsidianCost.obsidian || s.obsidianBots < bp.geodeCost.obsidian

let buyActions = [BuyOreBot; BuyClayBot; BuyObsidianBot; BuyGeodeBot]

let parseBlueprint (line: string): Blueprint =
    let r = new System.Text.RegularExpressions.Regex(@"Blueprint (?<id>\d+)\: .*ore robot costs (?<ore>.*)\. .*clay robot costs (?<clay>.*)\. .*obsidian robot costs (?<obsidian>.*)\. .*geode robot costs (?<geode>.*)\.")
    let m = r.Match(line)
    { 
        Blueprint.id=int(regexGroup(m, "id"));
        oreCost=Resources.regex(regexGroup(m, "ore"));
        clayCost=Resources.regex(regexGroup(m, "clay"));
        obsidianCost=Resources.regex(regexGroup(m, "obsidian"));
        geodeCost=Resources.regex(regexGroup(m, "geode"));
    }
    
// let timeNeeded (bp: Blueprint, state: State): int =
//     let clayTime = if state.clayBots < 1 then ((bp.clayCost.ore / (max 1 state.oreBots)) / 2) + 1 else 0
//     let obsidianTime = if state.obsidianBots < 1 then ((bp.obsidianCost.clay / (max 1 state.clayBots)) / 2) + 1 else 0
//     let geodeTime = if state.geodeBots < 1 then ((bp.geodeCost.obsidian / ( max 1 state.obsidianBots)) / 2) + 1 else 0
//     clayTime + obsidianTime + geodeTime

let timeNeeded (bp: Blueprint, state: State): int =
    let clayTime = if state.clayBots < 1 then ((bp.clayCost.ore / (max 1 state.oreBots)) / 2) + 0 else 0
    let obsidianTime = if state.obsidianBots < 1 then ((bp.obsidianCost.clay / (max 1 state.clayBots)) / 2) + 0 else 0
    let geodeTime = if state.geodeBots < 1 then ((bp.geodeCost.obsidian / ( max 1 state.obsidianBots)) / 2) + 0 else 0
    clayTime + obsidianTime + geodeTime
    
let canWait (bp: Blueprint, state: State): bool =
    if state.geodeBots > 0 then
        not(state.resources.canAfford(bp.oreCost) && state.resources.canAfford(bp.clayCost) &&
        state.resources.canAfford(bp.obsidianCost) && state.resources.canAfford(bp.geodeCost))        
    else if state.obsidianBots > 0 then
        not(state.resources.canAfford(bp.oreCost) && state.resources.canAfford(bp.clayCost) &&
        state.resources.canAfford(bp.obsidianCost) && state.resources.canAfford(bp.geodeCost))        
    else if state.clayBots > 0 then
        not(state.resources.canAfford(bp.oreCost) && state.resources.canAfford(bp.clayCost) &&
        state.resources.canAfford(bp.obsidianCost))         
    else
    not(state.resources.canAfford(bp.oreCost) && state.resources.canAfford(bp.clayCost))   
    
let canBeat (bp: Blueprint, challenger: State, benchmark: State): bool =
    let gb = challenger.geodeBots
    let rem = challenger.remaining
    if rem = 1 && gb < (benchmark.resources.geode - challenger.resources.geode) then false else
    if rem >= 2 && ((gb * rem) + (((rem)) * (rem-1))/2) <= (benchmark.resources.geode - challenger.resources.geode) then false else
    true
    
let mutable scoreCount = 0

let rec scoreBlueprint (bp: Blueprint, state: State, bestState: State): State =
    if state.remaining <= 0 then state else
    scoreCount <- scoreCount + 1
    if not(canBeat(bp, state, bestState)) then state else
    //if state.actions.Length = 8 then printfn "%A" state.actions else ()
    let waitAction = if canWait(bp, state)
                                                    then [(Wait, { state with
                                                                        remaining=state.remaining - 1;
                                                                        actions=List.append [Wait] state.actions;
                                                                        lastResources=state.resources;
                                                                        resources=state.resources.increment(state.oreBots, state.clayBots, state.obsidianBots, state.geodeBots)
                                                    })] else []
    let buyOreBotAction = if state.resources.canAfford(bp.oreCost) && state.needOreBots(bp)
                                                    && (not(state.lastResources.canAfford(bp.oreCost)) || (buyActions |> Seq.contains state.actions.Head))
                                                    then [(BuyOreBot, { state with
                                                                            remaining=state.remaining - 1;
                                                                            actions=List.append [BuyOreBot] state.actions;
                                                                            oreBots=state.oreBots + 1;
                                                                            lastResources=state.resources;
                                                                            resources=(state.resources.increment(state.oreBots, state.clayBots, state.obsidianBots, state.geodeBots) - bp.oreCost)
                                                    })] else []
    let buyClayBotAction = if state.resources.canAfford(bp.clayCost) && state.needClayBots(bp)
                                                    && (not(state.lastResources.canAfford(bp.clayCost)) || (buyActions |> Seq.contains state.actions.Head))
                                                    then [(BuyClayBot, { state with
                                                                            remaining=state.remaining - 1;
                                                                            actions=List.append [BuyClayBot] state.actions;
                                                                            clayBots=state.clayBots + 1;
                                                                            lastResources=state.resources;
                                                                            resources=(state.resources.increment(state.oreBots, state.clayBots, state.obsidianBots, state.geodeBots) - bp.clayCost)
                                                    })] else []
    let buyObsidianBotAction = if state.resources.canAfford(bp.obsidianCost) && state.needObsBots(bp)
                                                    && (not(state.lastResources.canAfford(bp.obsidianCost)) || (buyActions |> Seq.contains state.actions.Head))
                                                    then [(BuyObsidianBot, { state with
                                                                                remaining=state.remaining - 1;
                                                                                actions=List.append [BuyObsidianBot] state.actions;
                                                                                obsidianBots=state.obsidianBots + 1;
                                                                                lastResources=state.resources;
                                                                                resources=(state.resources.increment(state.oreBots, state.clayBots, state.obsidianBots, state.geodeBots) - bp.obsidianCost)
                                                    })] else []
    let buyGeodeBotAction = if state.resources.canAfford(bp.geodeCost)
                                                    && (not(state.lastResources.canAfford(bp.geodeCost)) || (buyActions |> Seq.contains state.actions.Head))
                                                    then [(BuyGeodeBot, { state with
                                                                            remaining=state.remaining - 1;
                                                                            actions=List.append [BuyGeodeBot] state.actions;
                                                                            geodeBots=state.geodeBots + 1;
                                                                            lastResources=state.resources;
                                                                            resources=(state.resources.increment(state.oreBots, state.clayBots, state.obsidianBots, state.geodeBots) - bp.geodeCost)
                                                    })] else []
    let actions = Seq.concat [buyOreBotAction; buyClayBotAction; buyObsidianBotAction; buyGeodeBotAction; waitAction]
    //printfn "%A" (actions |> List.ofSeq)
    if state.actions.Length = 0 then printfn "Score Calls %A" scoreCount else ()
    if actions |> Seq.length = 0 then state else
    //actions |> Seq.map (fun (action, newState) -> scoreBlueprint(bp, newState)) |> Seq.maxBy (fun s -> s.resources.geode)
    
    let mutable best = bestState
    for action, nextState in actions do
        let result = scoreBlueprint(bp, nextState, best)
        best <- if result.resources.geode > best.resources.geode then result else best
    best

let part1 (input : string) =
    let lines = input.Split('\n') |> Seq.filter (fun line -> line.Length > 0)
    let blueprints = lines |> Seq.map parseBlueprint
    blueprints 
    |> Seq.map (fun bp -> (bp, scoreBlueprint(bp, State.initialPart1(), State.initialPart1()))) 
    |> Seq.map (fun (bp, state) -> printfn "Finished %A %A" state.resources.geode state.actions; bp.id * state.resources.geode) 
    |> Seq.sum

let part2 (input : string) =
    let lines = input.Split('\n') |> Seq.filter (fun line -> line.Length > 0)
    let blueprints = lines |> Seq.take 3 |> Seq.map parseBlueprint
    blueprints 
    |> Seq.map (fun bp -> (bp, scoreBlueprint(bp, State.initialPart2(), State.initialPart2()))) 
    |> Seq.map (fun (bp, state) -> printfn "Finished %A %A" state.resources.geode state.actions; state.resources.geode) 
    |> Seq.reduce (*)