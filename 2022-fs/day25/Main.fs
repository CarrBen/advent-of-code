open System
open day25

let input = IO.File.ReadAllText("./input.txt")
let part1 = day25.part1(input)
printfn "Part1 %A" part1

let part2 = day25.part2(input)
printfn "Part2 %A" part2
