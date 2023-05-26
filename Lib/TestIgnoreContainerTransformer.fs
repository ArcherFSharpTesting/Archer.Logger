module Archer.Logger.TestIgnoreContainerTransformer

open System
open System.Reflection
open Archer
open Archer.CoreTypes.InternalTypes.RunnerTypes
open Archer.Logger.Detail
open Archer.Logger.LocationHelpers
open Archer.Logger.StringHelpers
open Microsoft.FSharp.Core

let transformIgnoredTest (assembly: Assembly) (indenter: IIndentTransformer) (message: string option, location: CodeLocation, test: ITestInfo) =
    let msg =
        match message with
        | None -> ""
        | Some value -> $" %A{value}"
        
    let name = shortTestTitleFormatter "" test
    let locationStr =
        if location = test.Location then ""
        else $" @ %d{location.LineNumber}"
        
    let relativeFilePath = (getRelativeFilePath assembly test)
        
    [
        indenter.Transform $"%s{name}: (Ignored%s{msg}%s{locationStr})"
        indenter.Indent().Transform $"(%s{relativeFilePath})"
    ]
    |> linesToString
    
let assemblyTestIgnoreContainerTransformer (ignoreTransformer: Assembly -> IIndentTransformer -> string option * CodeLocation * ITestInfo -> string) (assembly: Assembly) (indenter: IIndentTransformer) (container: TestIgnoreContainer) =
    let rec assemblyTestIgnoreContainerTransformer (indenter: IIndentTransformer) (container: TestIgnoreContainer) =
        match container with
        | EmptyIgnore -> ""
        | IgnoredTests tuples ->
            tuples
            |> List.map (ignoreTransformer assembly indenter)
            |> linesToString
            
        | IgnoreContainer (_, []) -> ""
        | IgnoreContainer (name, containers) ->
            let body = 
                containers |> List.map (assemblyTestIgnoreContainerTransformer (indenter.Indent ()) >> trimEnd >> appendNewLineIfNotEmpty)
                |> List.filter (String.IsNullOrWhiteSpace >> not)
                |> linesToString
                |> trimEnd
                
            [
                indenter.Transform name
                body
            ]
            |> linesToString
        
    assemblyTestIgnoreContainerTransformer indenter container
    |> trimEnd
    
let defaultAssemblyTestIgnoreContainerTransformer (assembly: Assembly) (indenter: IIndentTransformer) (container: TestIgnoreContainer) =
    assemblyTestIgnoreContainerTransformer transformIgnoredTest assembly indenter container 
    
let defaultTestIgnoreContainerTransformer (indenter: IIndentTransformer) (container: TestIgnoreContainer) =
    let assembly = Assembly.GetCallingAssembly ()
    defaultAssemblyTestIgnoreContainerTransformer assembly indenter container
    |> trimEnd
    
let defaultAllTestIgnoreContainerTransformer (indenter: IIndentTransformer) (containers: TestIgnoreContainer list) =
    let assembly = Assembly.GetCallingAssembly ()
    
    containers
    |> List.map (defaultAssemblyTestIgnoreContainerTransformer assembly indenter >> appendNewLine)
    |> List.filter (String.IsNullOrWhiteSpace >> not)
    |> linesToString
    |> trimEnd