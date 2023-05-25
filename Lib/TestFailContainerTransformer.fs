module Archer.Logger.TestFailContainerTransformer

open System
open System.Reflection
open Archer
open Archer.CoreTypes.InternalTypes
open Archer.CoreTypes.InternalTypes.RunnerTypes
open Archer.Logger.Detail
open Archer.Logger.LocationHelpers
open Archer.Logger.StringHelpers

let getIndentedSetupTeardownFailureMessage name (assembly: Assembly) (indenter: IIndentTransformer) (failure: SetupTeardownFailure) =
    getSetupTeardownFailureMessage name assembly (indenter.Indent ()) failure
    
let getIndentedTestFailureMessage assembly (indenter: IIndentTransformer) (failure: TestFailure) =
    getTestResultMessage assembly (indenter.Indent ()) (TestFailure failure)
    
let getIndentedGeneralTestingFailureMessage (_: Assembly) (indenter: IIndentTransformer) (failure: GeneralTestingFailure) =
    getGeneralTestingFailureMessage (indenter.Indent ()) failure

let transformTestFailureType (indenter: IIndentTransformer) (failure: TestFailureType, test: ITest) =
    let assembly = Assembly.GetAssembly typeof<IndentTransformer>
    
    let baseTransformer (resultTransformer: Assembly -> IIndentTransformer -> 'itemType -> string) (result: 'itemType) =
        detailedTestItemTransformer getTitleTimingString shortTestTitleFormatter getRelativeFilePath resultTransformer assembly indenter test None result
        
    match failure with
    | SetupFailureType failure ->
        baseTransformer (getIndentedSetupTeardownFailureMessage "SetupExecutionFailure") failure
    | TestRunFailureType testFailure ->
        baseTransformer getIndentedTestFailureMessage testFailure
    | TeardownFailureType failure ->
        baseTransformer (getIndentedSetupTeardownFailureMessage "TeardownExecutionFailure") failure
    | GeneralFailureType failure ->
        baseTransformer getIndentedGeneralTestingFailureMessage failure

let defaultTestFailContainerTransformer (indenter: IIndentTransformer) (failures: TestFailContainer) =
    let rec defaultTestFailContainerTransformer acc (indenter: IIndentTransformer) (failures: TestFailContainer) =
        match failures with
        | EmptyFailures -> acc
        | FailedTests failures ->
            let transformer = transformTestFailureType indenter
            
            let failureString =
                failures
                |> List.map transformer
                |> linesToString
                
            [
                acc |> appendNewLineIfNotEmpty
                failureString
            ]
            |> List.filter (String.IsNullOrWhiteSpace >> not)
            |> linesToString
            
        | FailContainer (_, []) -> acc
        | FailContainer (name, testFailContainers) ->
            let transforms =
                testFailContainers
                |> List.map (defaultTestFailContainerTransformer "" (indenter.Indent ()))
                |> List.filter (String.IsNullOrWhiteSpace >> not)
            
            if transforms.IsEmpty then acc
            else
                [
                    acc |> appendNewLineIfNotEmpty
                    indenter.Transform name
                    transforms |> String.concat Environment.NewLine
                ]
                |> linesToString
                |> trimEnd
        
    defaultTestFailContainerTransformer "" indenter failures
    
let defaultTestFailContainerAllTransformer (indenter: IIndentTransformer) (failures: TestFailContainer list) =
    failures
    |> List.map (defaultTestFailContainerTransformer indenter)
    |> linesToString