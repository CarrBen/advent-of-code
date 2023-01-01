open System
open day17

let input = IO.File.ReadAllText("./input.txt")
let part1 = day17.part1(input, 2022)
printfn "Part1 %A" part1

let part2 = day17.part2(input, 1000000000000L)
printfn "Part2 %A" part2
