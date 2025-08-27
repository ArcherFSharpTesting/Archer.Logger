/// <summary>
/// Provides detailed formatting and transformation functions for test results, failures, and execution details.
/// </summary>
module Archer.Logger.Detail

open System
open System.IO
open System.Reflection
open Archer
open Archer.CoreTypes.InternalTypes
open Archer.Logger.StringHelpers
open Archer.Logger.LocationHelpers

/// <summary>
/// Recursively formats an exception and its inner exceptions as a string with indentation.
/// </summary>
/// <param name="indenter">The indenter used for formatting.</param>
/// <param name="ex">The exception to format.</param>
/// <returns>A formatted string representing the exception and its inner exceptions.</returns>
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
    |> linesToString
    |> trimEnd

/// <summary>
/// Formats exception details with a custom name and indentation.
/// </summary>
/// <param name="indenter">The indenter used for formatting.</param>
/// <param name="name">The name or label for the exception.</param>
/// <param name="ex">The exception to format.</param>
/// <returns>A formatted string representing the exception details.</returns>
let getExceptionDetail (indenter: IIndentTransformer) name (ex: Exception) =
    [
        indenter.Transform name
        getExceptionString (indenter.Indent ()) ex
    ]
    |> linesToString

/// <summary>
/// Formats a setup or teardown failure message with details and location.
/// </summary>
/// <param name="name">The name of the setup or teardown phase.</param>
/// <param name="assembly">The assembly context.</param>
/// <param name="indenter">The indenter used for formatting.</param>
/// <param name="failure">The setup or teardown failure to format.</param>
/// <returns>A formatted string representing the failure message.</returns>
let getSetupTeardownFailureMessage name (assembly: Assembly) (indenter: IIndentTransformer) (failure: SetupTeardownFailure) =
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
    |> linesToString
    |> trimEnd

/// <summary>
/// Recursively formats a test expectation failure message.
/// </summary>
/// <param name="indenter">The indenter used for formatting.</param>
/// <param name="codeLocation">The code location of the failure.</param>
/// <param name="failure">The test expectation failure to format.</param>
/// <returns>A formatted string representing the expectation failure.</returns>
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
    |> linesToString

/// <summary>
/// Recursively formats a test failure message, including exceptions, ignored tests, expectation failures, and combination failures.
/// </summary>
/// <param name="assembly">The assembly context.</param>
/// <param name="indenter">The indenter used for formatting.</param>
/// <param name="failure">The test failure to format.</param>
/// <returns>A formatted string representing the test failure.</returns>
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
        |> linesToString

    getTestFailureMessage indenter failure

/// <summary>
/// Formats a test result message, including failures and successes.
/// </summary>
/// <param name="assembly">The assembly context.</param>
/// <param name="indenter">The indenter used for formatting.</param>
/// <param name="testResult">The test result to format.</param>
/// <returns>A formatted string representing the test result.</returns>
let getTestResultMessage assembly (indenter: IIndentTransformer) (testResult: TestResult) =
    match testResult with
    | TestFailure failure -> getTestFailureMessage assembly indenter failure
    | TestSuccess -> indenter.Transform "Test Result: Success"

/// <summary>
/// Formats a general testing failure message, including cancel, general, and exception failures.
/// </summary>
/// <param name="indenter">The indenter used for formatting.</param>
/// <param name="testResult">The general testing failure to format.</param>
/// <returns>A formatted string representing the general testing failure.</returns>
let getGeneralTestingFailureMessage (indenter: IIndentTransformer) (testResult: GeneralTestingFailure) =
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
    |> linesToString

/// <summary>
/// Formats an execution result message, including setup, test, teardown, and general execution failures.
/// </summary>
/// <param name="assembly">The assembly context.</param>
/// <param name="indenter">The indenter used for formatting.</param>
/// <param name="result">The test execution result to format.</param>
/// <returns>A formatted string representing the execution result.</returns>
let getExecutionResultMessage assembly (indenter: IIndentTransformer) = function
    | SetupExecutionFailure failure ->
        getSetupTeardownFailureMessage "SetupExecutionFailure" assembly indenter failure
    | TestExecutionResult testResult ->
        getTestResultMessage assembly indenter testResult
    | TeardownExecutionFailure failure ->
        getSetupTeardownFailureMessage "TeardownExecutionFailure" assembly indenter failure
    | GeneralExecutionFailure failure ->
        getGeneralTestingFailureMessage indenter failure

/// <summary>
/// Formats a timing string for a test, if timing information is available.
/// </summary>
/// <param name="timing">The optional timing information.</param>
/// <returns>A formatted timing string, or empty if none.</returns>
let getTitleTimingString = function
    | None -> ""
    | Some value -> $" [%A{value.Total}]"

/// <summary>
/// Formats a short test title string with timing and location.
/// </summary>
/// <param name="timingHeader">The timing header string.</param>
/// <param name="testInfo">The test information object.</param>
/// <returns>A formatted short test title string.</returns>
let shortTestTitleFormatter (timingHeader: string) (testInfo: ITestInfo) =
    $"%s{testInfo.TestName} @ %d{testInfo.Location.LineNumber}%s{timingHeader}"

/// <summary>
/// Formats a full test title string with container, timing, and location.
/// </summary>
/// <param name="timingHeader">The timing header string.</param>
/// <param name="testInfo">The test information object.</param>
/// <returns>A formatted full test title string.</returns>
let fullTestTitleFormatter (timingHeader: string) (testInfo: ITestInfo) =
    $"%s{testInfo.ContainerName}.%s{shortTestTitleFormatter timingHeader testInfo}"

/// <summary>
/// Represents a timing header string for test formatting.
/// </summary>
type TimingHeader = string

/// <summary>
/// Transforms a detailed test item, including timing, title, path, and result, into a formatted string.
/// </summary>
/// <param name="timingTransformer">A function to format the timing string.</param>
/// <param name="titleTransformer">A function to format the test title.</param>
/// <param name="pathTransformer">A function to format the test path.</param>
/// <param name="resultTransformer">A function to format the test result.</param>
/// <param name="assembly">The assembly context.</param>
/// <param name="indenter">The indenter used for formatting.</param>
/// <param name="testInfo">The test information object.</param>
/// <param name="timing">The optional timing information.</param>
/// <param name="result">The test result to format.</param>
/// <returns>A formatted string representing the detailed test item.</returns>
let detailedTestItemTransformer (timingTransformer: TestTiming option -> TimingHeader) (titleTransformer: TimingHeader -> ITestInfo -> string) (pathTransformer: Assembly -> ITestInfo -> string) (resultTransformer: Assembly -> IIndentTransformer -> 'itemType -> string) (assembly: Assembly) (indenter: IIndentTransformer) (testInfo: ITestInfo) (timing: TestTiming option) (result: 'itemType) =
    let transformedResult = resultTransformer assembly (indenter.Indent ()) result
    let timingStr = timingTransformer timing
    let title = titleTransformer timingStr testInfo
    let path = pathTransformer assembly testInfo

    [
        indenter.Transform title
        indenter.Transform $"(%s{path})"
        transformedResult
    ]
    |> linesToString

/// <summary>
/// The default detailed transformer for a test execution result, using the calling assembly.
/// </summary>
/// <param name="indenter">The indenter used for formatting.</param>
/// <param name="testInfo">The test information object.</param>
/// <param name="timing">The optional timing information.</param>
/// <param name="result">The test execution result to format.</param>
/// <returns>A formatted string representing the detailed test execution result.</returns>
let defaultDetailedTestExecutionResultTransformer (indenter: IIndentTransformer) (testInfo: ITestInfo) (timing: TestTiming option) (result: TestExecutionResult) =
    let assembly = Assembly.GetCallingAssembly ()
    detailedTestItemTransformer getTitleTimingString fullTestTitleFormatter getRelativeFilePath getExecutionResultMessage assembly indenter testInfo timing result