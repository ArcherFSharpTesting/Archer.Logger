[<AutoOpen>]
module Archer.Logger.Indent

open System

let private indentReporter (indent: string) indentCount (value: string) =
    if 0 < indentCount
    then
        let lines = value.Split ([|Environment.NewLine; "\n"; "\r"|], StringSplitOptions.None)
        lines
        |> Array.map (fun line ->
            let tabs =
                [for _ in 1..indentCount do yield indent]
                |> String.concat ""
                
            $"%s{tabs}%s{line}"
        )
        |> String.concat Environment.NewLine
        
    else
        value
        
type IndentionType =
    | Tabs
    | TwoSpaces
    
let indentionToString = function
    | TwoSpaces -> "  "
    | _ -> "\t"
        
type IndentReporter (indentionCount: int, indentionType: IndentionType) =
    let indent = indentionType |> indentionToString
    let reporter = indentReporter indent indentionCount
    
    new () = IndentReporter (0, Tabs)
    new (indentionCount: int) = IndentReporter (indentionCount, Tabs)
    
    member _.Report value = reporter (if value = null then "" else value)
    member _.Indent () = IndentReporter (indentionCount + 1, indentionType)
    member _.Indent count = IndentReporter (indentionCount + count, indentionType)