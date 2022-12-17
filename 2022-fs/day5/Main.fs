open System
open day5

let input = IO.File.ReadAllText("./input.txt")
let part1 = day5.part1(input)
printfn "Part1 %A" part1

let part2 = day5.part2(input)
printfn "Part2 %A" part2
