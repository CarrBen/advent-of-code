open System
open day15

let input = IO.File.ReadAllText("./input.txt")
let part1 = day15.part1(input, 2000000)
printfn "Part1 %A" part1

let part2 = day15.part2(input, 4000000)
printfn "Part2 %A" part2
