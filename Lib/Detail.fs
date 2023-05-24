module Archer.Logger.Detail

open System
open System.IO
open System.Reflection
open Archer
open Archer.CoreTypes.InternalTypes
open Archer.Logger.StringHelpers
open Archer.Logger.LocationHelpers

let rec private getExceptionString (indentReporter: IIndentReporter) (ex : exn) =
    let inner =
        if (ex.InnerException = null) then ""
        else getExceptionString (indentReporter.Indent ()) ex.InnerException
        
    let exType = ex.GetType()
    [
        indentReporter.Report $"%s{exType.Namespace}.%s{exType.Name}: %s{ex.Message}"
        indentReporter.Indent().Report (ex.StackTrace |> trim)
        inner
    ]
    |> String.concat Environment.NewLine
    |> trimEnd
    
let getExceptionDetail (indentReporter: IIndentReporter) name (ex: Exception) =
    [
        indentReporter.Report name
        getExceptionString (indentReporter.Indent ()) ex
    ]
    |> String.concat Environment.NewLine
    
let private getSetupTeardownFailureMessage (assembly: Assembly) (indentReporter: IIndentReporter) name (failure: SetupTeardownFailure) =
    match failure with
    | SetupTeardownExceptionFailure ex ->
        [
            getExceptionDetail indentReporter name ex
        ]
    | SetupTeardownCanceledFailure ->
        [
            indentReporter.Report $"%s{name}: (Canceled)"
        ]
    | GeneralSetupTeardownFailure (message, location) ->
        let path = getRelativePath assembly (DirectoryInfo $"%s{location.FilePath}%c{Path.DirectorySeparatorChar}")
        let path = Path.Combine (path, location.FileName)
        [
            indentReporter.Report $"%s{name} @ \"%s{path}@%d{location.LineNumber}\""
            indentReporter.Indent().Report $"General Failure:"
            indentReporter.Indent().Indent().Report message
        ]
    |> String.concat Environment.NewLine
    |> trimEnd
    
let rec private getTestExpectationMessage (indentReporter: IIndentReporter) (codeLocation: CodeLocation) (failure: TestExpectationFailure) =
    match failure with
    | ExpectationOtherFailure message ->
        [
            indentReporter.Report "Expectation Failure (Other)"
            indentReporter.Indent().Report message
        ]
    | ExpectationVerificationFailure failure ->
        [
            indentReporter.Report "Validation Failure"
            indentReporter.Indent().Report $"Expected: %A{failure.Expected}" |> replace "\"\"" "\""
            indentReporter.Indent().Report $"Actual:   %A{failure.Actual}" |> replace "\"\"" "\""
        ]
    | FailureWithMessage (message, failure) ->
        [
            getTestExpectationMessage indentReporter codeLocation failure
            indentReporter.Report "Failure Comment:"
            indentReporter.Indent().Report message
        ]
    |> String.concat Environment.NewLine

let private getTestFailureMessage assembly (indentReporter: IIndentReporter) (failure: TestFailure) =
    
    let rec getTestFailureMessage (indentReporter: IIndentReporter) (failure: TestFailure) =
        let message =
            match failure with
            | TestExceptionFailure ex ->
                [
                    getExceptionDetail indentReporter "Test Failure" ex
                ]
            | TestIgnored (message, codeLocation) ->
                let msg =
                    match message with
                    | None -> "Ignored"
                    | Some value -> $"Ignored %A{value}"
                [
                    indentReporter.Report $"Test Failure: (%s{msg}) @%d{codeLocation.LineNumber}"
                ]
            | TestExpectationFailure (failure, codeLocation) ->
                [
                    getTestExpectationMessage indentReporter codeLocation failure
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
                    then indentReporter.Indent().Indent().Report idA
                    else ""
                    
                let lengthMessageB =
                    if 0 < idB.Length
                    then indentReporter.Indent().Indent().Report idB
                    else ""
                
                [
                    [
                        indentReporter.Report "Combination Failure"
                    ]
                    [
                        getTestFailureMessage (indentReporter.Indent ()) failureA
                        lengthMessageA
                    ] |> List.filter (fun (v: string) -> 0 < v.Length)
                    [
                        getTestFailureMessage (indentReporter.Indent ()) failureB
                        lengthMessageB
                    ] |> List.filter (fun v -> 0 < v.Length)
                ]
                |> List.concat
        message                
        |> String.concat Environment.NewLine
        
    getTestFailureMessage indentReporter failure

let private getTestResultMessage assembly (indentReporter: IIndentReporter) (testResult: TestResult) =
    match testResult with
    | TestFailure failure -> getTestFailureMessage assembly indentReporter failure
    | TestSuccess -> indentReporter.Report "Test Result: Success"
    
let private getGeneralTestingFailureMessage (indentReporter: IIndentReporter) (testResult: GeneralTestingFailure) =
    match testResult with
    | GeneralCancelFailure ->
        [
            indentReporter.Report "General Failure: (Canceled)"
        ]
    | GeneralFailure message ->
        [
            indentReporter.Report "General Failure:"
            indentReporter.Indent().Report message
        ]
    | GeneralExceptionFailure ex ->
        [
            getExceptionDetail indentReporter "General Failure" ex
        ]
    |> String.concat Environment.NewLine
    
let getExecutionResultMessage assembly (indentReporter: IIndentReporter) = function
    | SetupExecutionFailure failure ->
        getSetupTeardownFailureMessage assembly (indentReporter.Indent ()) "SetupExecutionFailure" failure
    | TestExecutionResult testResult ->
        getTestResultMessage assembly (indentReporter.Indent ()) testResult
    | TeardownExecutionFailure failure ->
        getSetupTeardownFailureMessage assembly (indentReporter.Indent ()) "TeardownExecutionFailure" failure
    | GeneralExecutionFailure failure ->
        getGeneralTestingFailureMessage (indentReporter.Indent ()) failure
        
let getTitleTimingString = function
    | None -> ""
    | Some value -> $" [%A{value.Total}]"
    
let shortTestTitleFormatter (timingHeader: string) (testInfo: ITestInfo) =
    $"%s{testInfo.TestName} @ %d{testInfo.Location.LineNumber}%s{timingHeader}"
    
let fullTestTitleFormatter (timingHeader: string) (testInfo: ITestInfo) =
    $"%s{testInfo.ContainerName}.%s{shortTestTitleFormatter timingHeader testInfo}"
    
type TimingHeader = string
    
let detailedTestExecutionResultReporter (timingFormatter: TestTiming option -> TimingHeader) (titleFormatter: TimingHeader -> ITestInfo -> string) (pathFormatter: Assembly -> ITestInfo -> string) (resultReporter: Assembly -> IIndentReporter -> TestExecutionResult -> string) (assembly: Assembly) (indentReporter: IIndentReporter) (testInfo: ITestInfo) (timing: TestTiming option) (result: TestExecutionResult) =
    let resultReport = resultReporter assembly indentReporter result
    let timingStr = timingFormatter timing
    let title = titleFormatter timingStr testInfo
    let path = pathFormatter assembly testInfo
    
    [
        indentReporter.Report title
        indentReporter.Report $"(%s{path})"
        resultReport
    ]
    |> String.concat Environment.NewLine
    
let defaultDetailedTestExecutionResultReporter (indentReporter: IIndentReporter) (testInfo: ITestInfo) (timing: TestTiming option) (result: TestExecutionResult) =
    let assembly = Assembly.GetCallingAssembly ()
    detailedTestExecutionResultReporter getTitleTimingString fullTestTitleFormatter getRelativeFilePath getExecutionResultMessage assembly indentReporter testInfo timing result