module day1

let part1 (input : string) =
    let elves = input.Split "\n\n"
    let elf_seqs = Seq.map (fun (x : string) -> x.Split '\n') elves
    Seq.map (fun (x : string[]) -> x |> Seq.map int |> Seq.sum) elf_seqs |> Seq.max
    
    
let part2 (input: string) =
    let elves = input.Split "\n\n"
    let elf_seqs = Seq.map (fun (x : string) -> x.Split '\n') elves
    Seq.map (fun (x : string[]) -> x |> Seq.map int |> Seq.sum) elf_seqs |> Seq.sortDescending |> Seq.take 3 |> Seq.sum
