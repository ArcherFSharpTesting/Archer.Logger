module Archer.Logger.Summaries

open Archer

let resultMessageSummaryTransformer (testInfo: ITestInfo) resultMsg =
    $"%s{testInfo.ContainerName}.%s{testInfo.TestName} (%s{resultMsg}) @ %d{testInfo.Location.LineNumber}"
    
let getTestResultMessage = function
    | TestFailure (TestIgnored _) -> "Ignored"
    | TestFailure _ -> "Failure"
    | TestSuccess -> "Success"
    
let getGeneralExecutionFailureMessage = function
    | GeneralCancelFailure -> "Canceled"
    | _ -> "General Failure"
    
let getSetupTeardownFailureMessage name (failure: SetupTeardownFailure) =
    match failure with
    | SetupTeardownCanceledFailure -> "Canceled"
    | _ -> $"%s{name} Failure"
    
let getTestExecutionResultMessage = function
    | GeneralExecutionFailure generalTestingFailure -> generalTestingFailure |> getGeneralExecutionFailureMessage
    | TestExecutionResult testResult -> testResult |> getTestResultMessage
    | SetupExecutionFailure failure -> failure |> getSetupTeardownFailureMessage "Setup"
    | TeardownExecutionFailure failure -> failure |> getSetupTeardownFailureMessage "Teardown"
    
let resultSummaryTransformer (getMessage: 'a -> string) (getSummary: ITestInfo -> string -> string) (result: 'a) (testInfo: ITestInfo) =
    result
    |> getMessage
    |> getSummary testInfo
    
let defaultTestResultSummaryTransformer: TestResult -> ITestInfo -> string =
    resultSummaryTransformer getTestResultMessage resultMessageSummaryTransformer
    
let defaultTestExecutionResultSummaryTransformer: TestExecutionResult -> ITestInfo -> string =
    resultSummaryTransformer getTestExecutionResultMessage resultMessageSummaryTransformer