open System
open day19

let input = IO.File.ReadAllText("./input.txt")
let part1 = day19.part1(input)
printfn "Part1 %A" part1

let part2 = day19.part2(input)
printfn "Part2 %A" part2
