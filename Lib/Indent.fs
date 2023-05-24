[<AutoOpen>]
module Archer.Logger.Indent

open System

let private indenter (indent: string) indentCount (value: string) =
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
    | FourSpaces
    
let indentionToString = function
    | TwoSpaces -> "  "
    | FourSpaces -> "    "
    | _ -> "\t"
    
type IIndentReporter =
    abstract member Report: value: string -> string
    abstract member Indent: unit -> IIndentReporter
    abstract member Indent: indentCount: int -> IIndentReporter
        
type IndentReporter (indentionCount: int, indentionType: IndentionType) =
    let indent = indentionType |> indentionToString
    let reporter = indenter indent indentionCount
    
    new () = IndentReporter (0, Tabs)
    new (indentionCount: int) = IndentReporter (indentionCount, Tabs)
    
    member _.Report value = reporter (if value = null then "" else value)
    member _.Indent () = IndentReporter (indentionCount + 1, indentionType) :> IIndentReporter
    member _.Indent indentCount = IndentReporter (indentionCount + indentCount, indentionType) :> IIndentReporter
    
    interface IIndentReporter with
        member this.Report value = this.Report value
        member this.Indent () = this.Indent ()
        member this.Indent indentCount = this.Indent indentCount