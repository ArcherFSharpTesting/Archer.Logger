module Archer.Logger.Detail

open System
open System.IO
open System.Reflection
open Archer
open Archer.CoreTypes.InternalTypes
open Archer.Logger.StringHelpers
open Archer.Logger.LocationHelpers

let rec private getExceptionString (indenter: IIndentTransformer) (ex : exn) =
    let inner =
        if (ex.InnerException = null) then ""
        else getExceptionString (indenter.Indent ()) ex.InnerException
        
    let exType = ex.GetType()
    [
        indenter.Transform $"%s{exType.Namespace}.%s{exType.Name}: %s{ex.Message}"
        indenter.Indent().Transform (ex.StackTrace |> trim)
        inner
    ]
    |> String.concat Environment.NewLine
    |> trimEnd
    
let getExceptionDetail (indenter: IIndentTransformer) name (ex: Exception) =
    [
        indenter.Transform name
        getExceptionString (indenter.Indent ()) ex
    ]
    |> String.concat Environment.NewLine
    
let private getSetupTeardownFailureMessage (assembly: Assembly) (indenter: IIndentTransformer) name (failure: SetupTeardownFailure) =
    match failure with
    | SetupTeardownExceptionFailure ex ->
        [
            getExceptionDetail indenter name ex
        ]
    | SetupTeardownCanceledFailure ->
        [
            indenter.Transform $"%s{name}: (Canceled)"
        ]
    | GeneralSetupTeardownFailure (message, location) ->
        let path = getRelativePath assembly (DirectoryInfo $"%s{location.FilePath}%c{Path.DirectorySeparatorChar}")
        let path = Path.Combine (path, location.FileName)
        [
            indenter.Transform $"%s{name} @ \"%s{path}@%d{location.LineNumber}\""
            indenter.Indent().Transform $"General Failure:"
            indenter.Indent().Indent().Transform message
        ]
    |> String.concat Environment.NewLine
    |> trimEnd
    
let rec private getTestExpectationMessage (indenter: IIndentTransformer) (codeLocation: CodeLocation) (failure: TestExpectationFailure) =
    match failure with
    | ExpectationOtherFailure message ->
        [
            indenter.Transform "Expectation Failure (Other)"
            indenter.Indent().Transform message
        ]
    | ExpectationVerificationFailure failure ->
        [
            indenter.Transform "Validation Failure"
            indenter.Indent().Transform $"Expected: %s{failure.Expected}"
            indenter.Indent().Transform $"Actual:   %s{failure.Actual}"
        ]
    | FailureWithMessage (message, failure) ->
        [
            getTestExpectationMessage indenter codeLocation failure
            indenter.Transform "Failure Comment:"
            indenter.Indent().Transform message
        ]
    |> String.concat Environment.NewLine

let private getTestFailureMessage assembly (indenter: IIndentTransformer) (failure: TestFailure) =
    
    let rec getTestFailureMessage (indenter: IIndentTransformer) (failure: TestFailure) =
        let message =
            match failure with
            | TestExceptionFailure ex ->
                [
                    getExceptionDetail indenter "Test Failure" ex
                ]
            | TestIgnored (message, codeLocation) ->
                let msg =
                    match message with
                    | None -> "Ignored"
                    | Some value -> $"Ignored %A{value}"
                [
                    indenter.Transform $"Test Failure: (%s{msg}) @%d{codeLocation.LineNumber}"
                ]
            | TestExpectationFailure (failure, codeLocation) ->
                [
                    getTestExpectationMessage indenter codeLocation failure
                ]
            | CombinationFailure ((failureA, maybeLocationA), (failureB, maybeLocationB)) ->
                let getInfo (maybeLocation: CodeLocation option) =
                    match maybeLocation with
                    | None -> ""
                    | Some value ->
                        let path = getRelativePath assembly (DirectoryInfo value.FilePath)
                        let fullPath = Path.Combine (path, value.FileName)
                        $"%s{fullPath}@%d{value.LineNumber}"
            
                let idA = maybeLocationA |> getInfo
                let idB = maybeLocationB |> getInfo
                
                let lengthMessageA =
                    if 0 < idA.Length
                    then indenter.Indent().Indent().Transform idA
                    else ""
                    
                let lengthMessageB =
                    if 0 < idB.Length
                    then indenter.Indent().Indent().Transform idB
                    else ""
                
                [
                    [
                        indenter.Transform "Combination Failure"
                    ]
                    [
                        getTestFailureMessage (indenter.Indent ()) failureA
                        lengthMessageA
                    ] |> List.filter (fun (v: string) -> 0 < v.Length)
                    [
                        getTestFailureMessage (indenter.Indent ()) failureB
                        lengthMessageB
                    ] |> List.filter (fun v -> 0 < v.Length)
                ]
                |> List.concat
        message                
        |> String.concat Environment.NewLine
        
    getTestFailureMessage indenter failure

let private getTestResultMessage assembly (indenter: IIndentTransformer) (testResult: TestResult) =
    match testResult with
    | TestFailure failure -> getTestFailureMessage assembly indenter failure
    | TestSuccess -> indenter.Transform "Test Result: Success"
    
let private getGeneralTestingFailureMessage (indenter: IIndentTransformer) (testResult: GeneralTestingFailure) =
    match testResult with
    | GeneralCancelFailure ->
        [
            indenter.Transform "General Failure: (Canceled)"
        ]
    | GeneralFailure message ->
        [
            indenter.Transform "General Failure:"
            indenter.Indent().Transform message
        ]
    | GeneralExceptionFailure ex ->
        [
            getExceptionDetail indenter "General Failure" ex
        ]
    |> String.concat Environment.NewLine
    
let getExecutionResultMessage assembly (indenter: IIndentTransformer) = function
    | SetupExecutionFailure failure ->
        getSetupTeardownFailureMessage assembly (indenter.Indent ()) "SetupExecutionFailure" failure
    | TestExecutionResult testResult ->
        getTestResultMessage assembly (indenter.Indent ()) testResult
    | TeardownExecutionFailure failure ->
        getSetupTeardownFailureMessage assembly (indenter.Indent ()) "TeardownExecutionFailure" failure
    | GeneralExecutionFailure failure ->
        getGeneralTestingFailureMessage (indenter.Indent ()) failure
        
let getTitleTimingString = function
    | None -> ""
    | Some value -> $" [%A{value.Total}]"
    
let shortTestTitleFormatter (timingHeader: string) (testInfo: ITestInfo) =
    $"%s{testInfo.TestName} @ %d{testInfo.Location.LineNumber}%s{timingHeader}"
    
let fullTestTitleFormatter (timingHeader: string) (testInfo: ITestInfo) =
    $"%s{testInfo.ContainerName}.%s{shortTestTitleFormatter timingHeader testInfo}"
    
type TimingHeader = string
    
let detailedTestExecutionResultTransformer (timingTransformer: TestTiming option -> TimingHeader) (titleTransformer: TimingHeader -> ITestInfo -> string) (pathTransformer: Assembly -> ITestInfo -> string) (resultTransformer: Assembly -> IIndentTransformer -> TestExecutionResult -> string) (assembly: Assembly) (indenter: IIndentTransformer) (testInfo: ITestInfo) (timing: TestTiming option) (result: TestExecutionResult) =
    let transformedResult = resultTransformer assembly indenter result
    let timingStr = timingTransformer timing
    let title = titleTransformer timingStr testInfo
    let path = pathTransformer assembly testInfo
    
    [
        indenter.Transform title
        indenter.Transform $"(%s{path})"
        transformedResult
    ]
    |> String.concat Environment.NewLine
    
let defaultDetailedTestExecutionResultTransformer (indenter: IIndentTransformer) (testInfo: ITestInfo) (timing: TestTiming option) (result: TestExecutionResult) =
    let assembly = Assembly.GetCallingAssembly ()
    detailedTestExecutionResultTransformer getTitleTimingString fullTestTitleFormatter getRelativeFilePath getExecutionResultMessage assembly indenter testInfo timing result