[<AutoOpen>]
module Archer.Logger.Indent

open System

let private indentReporter tabCount (value: string) =
    if 0 < tabCount
    then
        let lines = value.Split ([|Environment.NewLine|], StringSplitOptions.None)
        lines
        |> Array.map (fun line ->
            let tabs = "".PadLeft (tabCount, '\t')
            $"%s{tabs}%s{line}"
        )
        |> String.concat Environment.NewLine
        
    else
        value
        
type IndentReporter (tabCount) =
    let reporter = indentReporter tabCount
    
    new () = IndentReporter 0
    
    member _.Report value = reporter value
    member _.Indent () = IndentReporter (tabCount + 1)
    member _.Indent count = IndentReporter (tabCount + count)