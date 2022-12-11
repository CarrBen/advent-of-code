open System
open dayN

let input = IO.File.ReadAllText("./input.txt")
let part1 = dayN.part1(input)
printfn "Part1 %A" part1

let part2 = dayN.part2(input)
printfn "Part2 %A" part2