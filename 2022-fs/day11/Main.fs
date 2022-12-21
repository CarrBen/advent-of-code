open System
open day11

let input = IO.File.ReadAllText("./input.txt")
let part1 = day11.part1(input)
printfn "Part1 %A" part1

let part2 = day11.part2(input)
printfn "Part2 %A" part2
