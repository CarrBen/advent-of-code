module day2

open System


type Shape =
    | Rock = 0
    | Paper = 1
    | Scissors = 2
    
    
type Outcome = Win | Draw | Lose


let scoreShape (input : Shape) =
    match input with
        | Shape.Rock -> 1
        | Shape.Paper -> 2
        | Shape.Scissors -> 3
        
        
let parseShape (input : char) =
    match input with
        | 'A' | 'X' -> Shape.Rock
        | 'B' | 'Y' -> Shape.Paper
        | 'C' | 'Z' -> Shape.Scissors
        
        
let parseOutcome (input : char) =
    match input with
        | 'X' -> Outcome.Lose
        | 'Y' -> Outcome.Draw
        | 'Z' -> Outcome.Win
            

let findOutcomeShapes (myShape : Shape, oppShape : Shape) =
    if (myShape = oppShape) then Outcome.Draw  // Draw
    else
        match myShape with
            | Shape.Rock ->
                if (oppShape = Shape.Paper) then Outcome.Lose
                else Outcome.Win  // Opp Scissors
            | Shape.Paper ->
                if (oppShape = Shape.Scissors) then Outcome.Lose
                else Outcome.Win  // Opp Rock
            | Shape.Scissors ->
                if (oppShape = Shape.Rock) then Outcome.Lose
                else Outcome.Win  // Opp Paper

    
let findOutcome (input : string) =
    let myShape = parseShape (input.[2])
    let oppShape = parseShape (input.[0])
    findOutcomeShapes (myShape, oppShape)

        
let findShape (input : string) =
    let oppShape = parseShape (input.[0])
    let desOutcome = parseOutcome (input.[2])   
    
    ((Enum.GetValues(typeof<Shape>)) |> Seq.cast<Shape> |> Seq.tryFind (fun shp -> findOutcomeShapes (shp, oppShape) = desOutcome)).Value
       
    
let scoreOutcome (outcome : Outcome) =
    match outcome with
        | Outcome.Win -> 6
        | Outcome.Draw -> 3
        | Outcome.Lose -> 0
    
    
let scoreRoundPart1 (input : string) =
    if input.Length < 1 then 0
    else
        let myShape = parseShape(input.[2])
        let oppShape = parseShape(input.[0])
        scoreShape(myShape) + scoreOutcome(findOutcomeShapes(myShape, oppShape))
        
        
let scoreRoundPart2 (input : string) =
    if input.Length < 1 then 0
    else
        let myShape = findShape(input)
        let oppShape = parseShape(input.[0])
        scoreShape(myShape) + scoreOutcome(findOutcomeShapes(myShape, oppShape))


let part1 (input : string) =
    let rounds = input.Split "\n"
    let roundScores = Seq.map scoreRoundPart1 rounds
    roundScores |> Seq.sum
    
    
let part2 (input : string) =
    let rounds = input.Split "\n"
    let roundScores = Seq.map scoreRoundPart2 rounds
    roundScores |> Seq.sum
