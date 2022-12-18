open System
open day10

let input = IO.File.ReadAllText("./input.txt")
let part1 = day10.part1(input)
printfn "Part1 %A" part1

let part2 = day10.part2(input)
printfn "Part2\n%A" part2
