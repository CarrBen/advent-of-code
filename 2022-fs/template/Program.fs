open System

// From http://fssnip.net/gO/title/Copy-a-directory-of-files
let rec directoryCopy srcPath dstPath =
    if not <| IO.Directory.Exists(dstPath) then
        IO.Directory.CreateDirectory(dstPath) |> ignore
        
    let srcDir = IO.DirectoryInfo(srcPath)
    
    for file in srcDir.GetFiles() do
        let temppath = IO.Path.Combine(dstPath, file.Name)
        file.CopyTo(temppath, true) |> ignore
        
    for subdir in srcDir.GetDirectories() do
        let dstSubDir = IO.Path.Combine(dstPath, subdir.Name)
        directoryCopy subdir.FullName dstSubDir
        
let rec replaceInFileNames (path, find: string, replace: string) = 
    let srcDir = IO.DirectoryInfo(path)
    
    for file in srcDir.GetFiles() do
        let temppath = IO.Path.Combine(path, file.Name)
        file.MoveTo(temppath.Replace(find, replace)) |> ignore
        
    for subdir in srcDir.GetDirectories() do
        replaceInFileNames (subdir.FullName, find, replace)
        
let rec replaceInFiles (path, find: string, replace: string) = 
    let srcDir = IO.DirectoryInfo(path)
    
    for file in srcDir.GetFiles() do
        let contents = IO.File.ReadAllLines(IO.Path.Combine(file.DirectoryName, file.Name))
        let newContents = contents |> Array.map (fun line -> line.Replace(find, replace))
        IO.File.WriteAllLines(IO.Path.Combine(file.DirectoryName, file.Name), newContents)
        
    for subdir in srcDir.GetDirectories() do
        replaceInFiles (subdir.FullName, find, replace)

[<EntryPoint>]
let main(args) =
    let dayNum = args.[0]
    directoryCopy "./dayN" $"../day{dayNum}" |> ignore
    directoryCopy "./dayN.Test" $"../day{dayNum}.Test" |> ignore
    
    replaceInFileNames ($"../day{dayNum}", "dayN", $"day{dayNum}") |> ignore
    replaceInFileNames ($"../day{dayNum}.Test", "dayN", $"day{dayNum}") |> ignore
    replaceInFileNames ($"../day{dayNum}", "DayN", $"Day{dayNum}") |> ignore
    replaceInFileNames ($"../day{dayNum}.Test", "DayN", $"Day{dayNum}") |> ignore
    
    replaceInFiles ($"../day{dayNum}", "dayN", $"day{dayNum}") |> ignore
    replaceInFiles ($"../day{dayNum}.Test", "dayN", $"day{dayNum}") |> ignore
    replaceInFiles ($"../day{dayNum}", "DayN", $"Day{dayNum}") |> ignore
    replaceInFiles ($"../day{dayNum}.Test", "DayN", $"Day{dayNum}") |> ignore
    
    0;;
    