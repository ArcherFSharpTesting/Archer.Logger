[<AutoOpen>]
module Archer.Logger.Defaults

open Archer

let resultSummaryReporter (testInfo: ITestInfo) resultMsg =
    $"%s{testInfo.ContainerName}.%s{testInfo.TestName} (%s{resultMsg}) @ %d{testInfo.Location.LineNumber}"
    
let getTestResultMessage = function
    | TestFailure (TestIgnored _) -> "Ignored"
    | TestFailure _ -> "Failure"
    | TestSuccess -> "Success"
    
let testResultSummaryReporter (testResult: TestResult) (testInfo: ITestInfo) =
    testResult
    |> getTestResultMessage
    |> resultSummaryReporter testInfo
    
let getGeneralExecutionFailureMessage = function
    | GeneralCancelFailure -> "Canceled"
    | _ -> "General Failure"
    
let getSetupTeardownFailureMessage name (failure: SetupTeardownFailure) =
    match failure with
    | SetupTeardownCanceledFailure -> "Canceled"
    | _ -> $"%s{name} Failure"
    
let testExecutionResultSummaryReporter (testExecutionResult: TestExecutionResult) (testInfo: ITestInfo) =
    match testExecutionResult with
    | GeneralExecutionFailure generalTestingFailure -> generalTestingFailure |> getGeneralExecutionFailureMessage
    | TestExecutionResult testResult -> testResult |> getTestResultMessage
    | SetupExecutionFailure failure -> failure |> getSetupTeardownFailureMessage "Setup"
    | TeardownExecutionFailure failure -> failure |> getSetupTeardownFailureMessage "Teardown"

    |> resultSummaryReporter testInfo