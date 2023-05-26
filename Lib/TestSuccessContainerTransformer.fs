module Archer.Logger.TestSuccessContainerTransformer

open System
open System.Reflection
open Archer
open Archer.CoreTypes.InternalTypes.RunnerTypes
open Archer.Logger.Detail
open Archer.Logger.StringHelpers
open Archer.Logger.LocationHelpers

let testAssemblySuccessContainerTransformer (titleFormatter: string -> ITestInfo -> string) (pathFormatter: Assembly -> ITestLocationInfo -> string) (assembly: Assembly) (indenter: IIndentTransformer) (successContainer: TestSuccessContainer) =
    let rec testSuccessContainerTransformer (indenter: IIndentTransformer) (successContainer: TestSuccessContainer) =
        match successContainer with
        | EmptySuccesses -> ""
        | SucceededTests tests ->
            tests
            |> List.map (fun testInfo ->
                let title = titleFormatter "" testInfo
                let path = pathFormatter assembly testInfo

                [
                    indenter.Transform $"%s{title}: Success"
                    indenter.Indent().Transform $"(%s{path})"
                ]
                |> linesToString
            )
            |> linesToString
        | SuccessContainer (name, testSuccessContainers) ->
            let details =
                testSuccessContainers
                |> List.map (testSuccessContainerTransformer (indenter.Indent ()) >> appendNewLine)
                |> List.filter (String.IsNullOrWhiteSpace >> not)
                |> linesToString
                
            [
                indenter.Transform name
                details
            ]
            |> linesToString
        
    testSuccessContainerTransformer indenter successContainer
    |> trimEnd
    
let singleTestAssemblySuccessContainerTransformer (titleFormatter: string -> ITestInfo -> string) (pathFormatter: Assembly -> ITestLocationInfo -> string) (indenter: IIndentTransformer) (successContainer: TestSuccessContainer) =
    let assembly = Assembly.GetCallingAssembly ()
    
    testAssemblySuccessContainerTransformer titleFormatter pathFormatter assembly indenter successContainer

let defaultSingleTestSuccessContainerTransformer (indenter: IIndentTransformer) (successContainer: TestSuccessContainer) =
    singleTestAssemblySuccessContainerTransformer shortTestTitleFormatter getRelativeFilePath indenter successContainer
    
let defaultTestSuccessContainer (assembly: Assembly) (indenter: IIndentTransformer) (successContainer: TestSuccessContainer) =
    testAssemblySuccessContainerTransformer shortTestTitleFormatter getRelativeFilePath assembly indenter successContainer
    
let defaultAllTestSuccessContainerTransformer (indenter: IIndentTransformer) (successContainers: TestSuccessContainer list) =
    successContainers
    |> List.map (defaultSingleTestSuccessContainerTransformer indenter >> appendNewLine)
    |> linesToString
    |> trimEnd