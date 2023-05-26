module Archer.Logger.TestFailContainerTransformer

open System
open System.Reflection
open Archer
open Archer.CoreTypes.InternalTypes
open Archer.CoreTypes.InternalTypes.RunnerTypes
open Archer.Logger.Detail
open Archer.Logger.LocationHelpers
open Archer.Logger.StringHelpers

let getWrappedTestFailureMessage assembly (indenter: IIndentTransformer) (failure: TestFailure) =
    getTestResultMessage assembly indenter (TestFailure failure)
    
let getIgnoreAssemblyGeneralTestingFailureMessage (_: Assembly) (indenter: IIndentTransformer) (failure: GeneralTestingFailure) =
    getGeneralTestingFailureMessage indenter failure

let transformTestFailureType (indenter: IIndentTransformer) (failure: TestFailureType, test: ITest) =
    let assembly = Assembly.GetAssembly typeof<IndentTransformer>
    
    let baseTransformer (resultTransformer: Assembly -> IIndentTransformer -> 'itemType -> string) (result: 'itemType) =
        detailedTestItemTransformer getTitleTimingString shortTestTitleFormatter getRelativeFilePath resultTransformer assembly indenter test None result
        
    match failure with
    | SetupFailureType failure ->
        baseTransformer (getSetupTeardownFailureMessage "SetupExecutionFailure") failure
    | TestRunFailureType testFailure ->
        baseTransformer getWrappedTestFailureMessage testFailure
    | TeardownFailureType failure ->
        baseTransformer (getSetupTeardownFailureMessage "TeardownExecutionFailure") failure
    | GeneralFailureType failure ->
        baseTransformer getIgnoreAssemblyGeneralTestingFailureMessage failure

let testFailContainerTransformer (testFailureTypeTransformer: IIndentTransformer -> TestFailureType * ITest -> string) (indenter: IIndentTransformer) (failures: TestFailContainer) =
    let rec testFailContainerTransformer acc (indenter: IIndentTransformer) (failures: TestFailContainer) =
        match failures with
        | EmptyFailures -> acc
        | FailedTests failures ->
            let transformer = testFailureTypeTransformer indenter
            
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
                |> List.map (testFailContainerTransformer "" (indenter.Indent ()))
                |> List.filter (String.IsNullOrWhiteSpace >> not)
            
            if transforms.IsEmpty then acc
            else
                [
                    acc |> appendNewLineIfNotEmpty
                    indenter.Transform name
                    transforms |> linesToString
                ]
                |> linesToString
                |> trimEnd
        
    testFailContainerTransformer "" indenter failures

let defaultTestFailContainerTransformer (indenter: IIndentTransformer) (failures: TestFailContainer) =
    testFailContainerTransformer transformTestFailureType indenter failures
    
let defaultTestFailContainerAllTransformer (indenter: IIndentTransformer) (failures: TestFailContainer list) =
    failures
    |> List.map (defaultTestFailContainerTransformer indenter)
    |> linesToString