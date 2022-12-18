module day7

type TreeItem =
    | File of FileItem
    | Dir of DirItem
and FileItem = { Size: uint; Name: string}
and DirItem = { Children: List<TreeItem>; Name: string }

type Command = { cwd: List<string>; cmd: string }

let matchName (target: string): TreeItem -> bool =
    fun (item: TreeItem) ->
        match item with
            | File f -> f.Name=target
            | Dir d -> d.Name=target

let rec findDir (dir: DirItem, target: string): Option<DirItem> =
    if dir.Name = target then Some(dir) else
    let localMatch = dir.Children |> Seq.tryFind (matchName target)
    if localMatch.IsSome then 
        match localMatch.Value with
            | Dir d -> Some(d)
    else
        dir.Children |> Seq.tryPick (
            fun i -> 
            match i with 
                | Dir d -> findDir(d, target) 
                | File f -> None
            )
            
let parseLs (input: string): TreeItem =
    let a::b = input.Split(' ') |> List.ofArray
    if a = "dir" then TreeItem.Dir { Name = b |> List.exactlyOne; Children = [] }
    else TreeItem.File { Name = b |> List.exactlyOne; Size = uint a }

let rec parseCommands (input: List<string>, commands: List<Command>, path: List<string>): List<Command> =
    if input.Length = 0 then commands else
    let command::output = input.Head.Split('\n') |> List.ofArray
    match command.TrimStart(' ').Split(' ') |> List.ofArray with
        | cmd::args & "cd"::_ -> parseCommands(input.Tail, commands, if args = [".."] then path |> Seq.take (path.Length - 1) |> List.ofSeq else if args =["/"] then [] else List.append path args)
        | cmd::args & "ls"::_ -> parseCommands(input.Tail, List.append commands [{ Command.cwd = path; cmd = output |> String.concat "\n" }], path)

let rec applyCommand (input: Command, root: DirItem): DirItem =
    if input.cwd = [] then { DirItem.Name = root.Name; Children = input.cmd.Split('\n') |> Seq.filter (fun l -> l.Length > 0) |> Seq.map parseLs |> List.ofSeq }  
    else
    { root with Children = root.Children |> Seq.map (fun d ->
        match d with
            | File f-> f |> TreeItem.File
            | Dir d -> (if d.Name = input.cwd.Head then applyCommand({ input with cwd = input.cwd |> Seq.skip 1 |> List.ofSeq }, d) else d) |> TreeItem.Dir ) |> List.ofSeq
    }

let rec applyCommands (input: List<Command>, root: DirItem): DirItem =
    if input.Length = 0 then root else
    applyCommands(input.Tail, applyCommand(input.Head, root))

let parseTree (input: string): DirItem =
    let commandStrings = input.Split('$') |> Seq.filter (fun s -> s.Length > 0) |> List.ofSeq
    let commands = parseCommands(commandStrings, [], [])
    applyCommands(commands, { DirItem.Name = ""; Children = [] })
    
let rec dirSize (input: DirItem): uint =
    input.Children |> Seq.map (fun child -> 
        match child with
            | File f -> f.Size
            | Dir d -> dirSize(d)
    ) |> Seq.sum
    
let targetSize (dir: DirItem, target: string): uint =
    let root = findDir(dir, target)
    dirSize(root.Value)
    
let rec findDirs (input: DirItem): List<DirItem> =
    input.Children 
    |> Seq.map (fun child -> match child with
                                            | File f -> []
                                            | Dir d -> List.append [d] (findDirs(d))
    ) |> Seq.concat |> List.ofSeq
    
let part1 (input : string) =
    let tree = parseTree(input)
    let dirs = findDirs(tree)
    dirs |> Seq.map dirSize |> Seq.filter (fun size -> size < 100000u) |> Seq.sum
    
let part2 (input : string) =
    let tree = parseTree(input)
    let freeSpace = 70_000_000u - dirSize tree
    let spaceToFree = 30_000_000u - freeSpace
    let dirs = findDirs(tree)
    dirs |> Seq.map dirSize |> Seq.filter (fun size -> size > spaceToFree) |> Seq.min