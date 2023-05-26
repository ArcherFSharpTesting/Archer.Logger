[<AutoOpen>]
module Archer.Logger.Indent

open System
open Archer.Logger.StringHelpers

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
        |> Array.toList
        |> linesToString
        
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
    
type IIndentTransformer =
    abstract member Transform: value: string -> string
    abstract member Indent: unit -> IIndentTransformer
    abstract member Indent: indentCount: int -> IIndentTransformer
        
type IndentTransformer (indentionCount: int, indentionType: IndentionType) =
    let indent = indentionType |> indentionToString
    let indenter = indenter indent indentionCount
    
    new () = IndentTransformer (0, Tabs)
    new (indentionCount: int) = IndentTransformer (indentionCount, Tabs)
    
    member _.Transform value = indenter (if value = null then "" else value)
    member _.Indent () = IndentTransformer (indentionCount + 1, indentionType) :> IIndentTransformer
    member _.Indent indentCount = IndentTransformer (indentionCount + indentCount, indentionType) :> IIndentTransformer
    
    interface IIndentTransformer with
        member this.Transform value = this.Transform value
        member this.Indent () = this.Indent ()
        member this.Indent indentCount = this.Indent indentCount