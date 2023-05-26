module Archer.Logger.TestSuccessContainerTransformer

open System.Reflection
open Archer.CoreTypes.InternalTypes.RunnerTypes
open Archer.Logger.Detail
open Archer.Logger.StringHelpers
open Archer.Logger.LocationHelpers

let defaultTestSuccessContainerTransformer (indenter: IIndentTransformer) (successContainer: TestSuccessContainer) =
    let assembly = Assembly.GetCallingAssembly ()
    let rec defaultTestSuccessContainerTransformer (indenter: IIndentTransformer) (successContainer: TestSuccessContainer) =
        match successContainer with
        | EmptySuccesses -> ""
        | SucceededTests tests ->
            tests
            |> List.map (fun testInfo ->
                let title = shortTestTitleFormatter "" testInfo
                let path = getRelativeFilePath assembly testInfo

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
                |> List.map (defaultTestSuccessContainerTransformer (indenter.Indent ()) >> appendNewLine)
                |> linesToString
                
            [
                indenter.Transform name
                details
            ]
            |> linesToString
        
    defaultTestSuccessContainerTransformer indenter successContainer
    |> trimEnd
    
let defaultAllTestSuccessContainerTransformer (indenter: IIndentTransformer) (successContainers: TestSuccessContainer list) =
    successContainers
    |> List.map (defaultTestSuccessContainerTransformer indenter >> appendNewLine)
    |> linesToString
    |> trimEnd