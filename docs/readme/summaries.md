<!-- (dl
(section-meta
    (title Archer.Reporting Summaries)
)
) -->

This document describes the summary transformation functions in Archer.Reporting. These functions are responsible for generating concise, human-readable summaries of test results and execution outcomes.

<!-- (dl (# resultMessageSummaryTransformer)) -->
**resultMessageSummaryTransformer**
- Formats a summary string for a test result, including the container name, test name, result message, and line number.
- Signature: `ITestInfo -> string -> string`

<!-- (dl (# getTestResultMessage)) -->
**getTestResultMessage**
- Returns a string representing the outcome of a test result: "Ignored", "Failure", or "Success".
- Signature: `TestResult -> string`

<!-- (dl (# getGeneralExecutionFailureMessage)) -->
**getGeneralExecutionFailureMessage**
- Returns a string for general execution failures: "Canceled" or "General Failure".
- Signature: `GeneralTestingFailure -> string`

<!-- (dl (# getSetupTeardownFailureMessage)) -->
**getSetupTeardownFailureMessage**
- Returns a string for setup or teardown failures, using the provided name ("Setup" or "Teardown").
- Signature: `string -> SetupTeardownFailure -> string`

<!-- (dl (# getTestExecutionResultMessage)) -->
**getTestExecutionResultMessage**
- Returns a string describing the result of a test execution, handling general, setup, and teardown failures.
- Signature: `TestExecutionResult -> string`

<!-- (dl (# resultSummaryTransformer)) -->
**resultSummaryTransformer**
- Composes a summary string for a result using provided message and summary functions.
- Signature: `('a -> string) -> (ITestInfo -> string -> string) -> 'a -> ITestInfo -> string`

<!-- (dl (# defaultTestResultSummaryTransformer)) -->
**defaultTestResultSummaryTransformer**
- Default summary transformer for test results.
- Signature: `TestResult -> ITestInfo -> string`

<!-- (dl (# defaultTestExecutionResultSummaryTransformer)) -->
**defaultTestExecutionResultSummaryTransformer**
- Default summary transformer for test execution results.
- Signature: `TestExecutionResult -> ITestInfo -> string`
