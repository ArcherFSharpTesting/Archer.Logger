/// <summary>
/// Provides functions for transforming test results and execution results into summary strings.
/// </summary>
module Archer.Logger.Summaries

open Archer

/// <summary>
/// Formats a summary string for a test result message, including container, test name, result, and line number.
/// </summary>
/// <param name="testInfo">The test information object.</param>
/// <param name="resultMsg">The result message to include.</param>
/// <returns>A formatted summary string for the test result.</returns>
let resultMessageSummaryTransformer (testInfo: ITestInfo) resultMsg =
    $"%s{testInfo.ContainerName}.%s{testInfo.TestName} (%s{resultMsg}) @ %d{testInfo.Location.LineNumber}"

/// <summary>
/// Gets a string message representing the outcome of a test result.
/// </summary>
/// <param name="result">The test result to evaluate.</param>
/// <returns>"Ignored", "Failure", or "Success" depending on the result.</returns>
let getTestResultMessage = function
    | TestFailure (TestIgnored _) -> "Ignored"
    | TestFailure _ -> "Failure"
    | TestSuccess -> "Success"

/// <summary>
/// Gets a string message for a general execution failure.
/// </summary>
/// <param name="failure">The general execution failure value.</param>
/// <returns>"Canceled" or "General Failure" depending on the failure type.</returns>
let getGeneralExecutionFailureMessage = function
    | GeneralCancelFailure -> "Canceled"
    | _ -> "General Failure"

/// <summary>
/// Gets a string message for a setup or teardown failure.
/// </summary>
/// <param name="name">The name of the phase (e.g., "Setup" or "Teardown").</param>
/// <param name="failure">The setup or teardown failure value.</param>
/// <returns>"Canceled" or a formatted failure message.</returns>
let getSetupTeardownFailureMessage name (failure: SetupTeardownFailure) =
    match failure with
    | SetupTeardownCanceledFailure -> "Canceled"
    | _ -> $"%s{name} Failure"

/// <summary>
/// Gets a string message representing the outcome of a test execution result.
/// </summary>
/// <param name="result">The test execution result to evaluate.</param>
/// <returns>A string describing the execution result.</returns>
let getTestExecutionResultMessage = function
    | GeneralExecutionFailure generalTestingFailure -> generalTestingFailure |> getGeneralExecutionFailureMessage
    | TestExecutionResult testResult -> testResult |> getTestResultMessage
    | SetupExecutionFailure failure -> failure |> getSetupTeardownFailureMessage "Setup"
    | TeardownExecutionFailure failure -> failure |> getSetupTeardownFailureMessage "Teardown"

/// <summary>
/// Transforms a test result or execution result into a summary string using the provided message and summary functions.
/// </summary>
/// <param name="getMessage">A function to get the result message.</param>
/// <param name="getSummary">A function to format the summary string.</param>
/// <param name="result">The result value to summarize.</param>
/// <param name="testInfo">The test information object.</param>
/// <returns>A formatted summary string for the result.</returns>
let resultSummaryTransformer (getMessage: 'a -> string) (getSummary: ITestInfo -> string -> string) (result: 'a) (testInfo: ITestInfo) =
    result
    |> getMessage
    |> getSummary testInfo

/// <summary>
/// The default summary transformer for test results.
/// </summary>
let defaultTestResultSummaryTransformer: TestResult -> ITestInfo -> string =
    resultSummaryTransformer getTestResultMessage resultMessageSummaryTransformer

/// <summary>
/// The default summary transformer for test execution results.
/// </summary>
let defaultTestExecutionResultSummaryTransformer: TestExecutionResult -> ITestInfo -> string =
    resultSummaryTransformer getTestExecutionResultMessage resultMessageSummaryTransformer