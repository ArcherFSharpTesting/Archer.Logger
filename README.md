<!-- GENERATED DOCUMENT DO NOT EDIT! -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->

<!-- Compiled with doculisp https://www.npmjs.com/package/doculisp -->

# Archer.Logger: Test Output Logger for F# #

1. Overview: [Archer.Logger Overview](#archerlogger-overview)
2. Types: [Archer.Logger Types](#archerlogger-types)
3. StringHelpers: [Archer.Logger String Helpers](#archerlogger-string-helpers)
4. Summaries: [Archer.Logger Summaries](#archerlogger-summaries)

## Archer.Logger Overview ##

Archer.Logger is a component of the Archer F# test framework responsible for formatting and outputting test results. It provides clear, structured, and customizable output for test execution, making it easier to interpret test results and debug issues.

### Features ###

- Formats test results for readability
- Supports summaries and detailed output
- Integrates with Archer test framework

### Usage ###

Refer to the main README for setup and integration instructions.

---

*This is an overview document for the Archer.Logger component. For more details, see the main documentation and source code.*

## Archer.Logger Types ##

This page provides an overview of the core types and interfaces used by Archer.Logger for formatting and outputting test results. These types define the structure and contracts for logging, reporting, and handling test execution outcomes in the Archer F# test framework.

### InformationDensity ###

Represents the level of detail for logging information.
- `SummaryDensity`: Summary-level information.
- `DetailedDensity`: Detailed information.

### LogScope ###

Specifies the context or scope for log messages.
- `ErrorScope`: Error messages.
- `TestFailureScope`: Test failure messages.
- `TestIgnoreScope`: Ignored test messages.
- `TestSuccessScope`: Successful test messages.

### ITestLogger ###

Interface for logging test information.
- `Log: density: InformationDensity -> scope: LogScope -> string -> unit`

### ITestResultLogger ###

Interface for logging test results.
- `LogTestResult: density: InformationDensity -> result: TestResult -> unit`

### ITestExecutionResultLogger ###

Interface for logging test execution results.
- `LogExecutionResult: density: InformationDensity -> result: TestExecutionResult -> unit`

### ITestContainerLogger ###

Interface for logging collections of test results.
- `LogFailures: density: InformationDensity -> failures: TestFailContainer list -> unit`
- `LogSuccesses: density: InformationDensity -> successes: TestSuccessContainer list -> unit`
- `LogIgnored: density: InformationDensity -> ignored: TestIgnoreContainer list -> unit`

## Archer.Logger String Helpers ##

This document describes the string helper functions provided in the Archer.Logger library. These helpers are used for common string manipulations and formatting tasks within the logger.

### trim ###

**trim**
- Removes whitespace from both ends of a string. Returns the original value if it is null.

### trimEnd ###

**trimEnd**
- Removes whitespace from the end of a string. Returns the original value if it is null.

### replace ###

**replace**
- Replaces all occurrences of a substring with another substring in the given string.
- Parameters: `toBeReplaced`, `toReplace`, `inValue`

### appendNewLine ###

**appendNewLine**
- Appends a newline character to the end of the given string.

### appendNewLineIfNotEmpty ###

**appendNewLineIfNotEmpty**
- Appends a newline character to the string only if it is not empty or null.

### removeLastChar ###

**removeLastChar**
- Removes the last character from the string.

### linesToString ###

**linesToString**
- Joins a list of strings into a single string separated by newlines, and trims any trailing whitespace.

## Archer.Logger Summaries ##

This document describes the summary transformation functions in Archer.Logger. These functions are responsible for generating concise, human-readable summaries of test results and execution outcomes.

### resultMessageSummaryTransformer ###

**resultMessageSummaryTransformer**
- Formats a summary string for a test result, including the container name, test name, result message, and line number.
- Signature: `ITestInfo -> string -> string`

### getTestResultMessage ###

**getTestResultMessage**
- Returns a string representing the outcome of a test result: "Ignored", "Failure", or "Success".
- Signature: `TestResult -> string`

### getGeneralExecutionFailureMessage ###

**getGeneralExecutionFailureMessage**
- Returns a string for general execution failures: "Canceled" or "General Failure".
- Signature: `GeneralTestingFailure -> string`

### getSetupTeardownFailureMessage ###

**getSetupTeardownFailureMessage**
- Returns a string for setup or teardown failures, using the provided name ("Setup" or "Teardown").
- Signature: `string -> SetupTeardownFailure -> string`

### getTestExecutionResultMessage ###

**getTestExecutionResultMessage**
- Returns a string describing the result of a test execution, handling general, setup, and teardown failures.
- Signature: `TestExecutionResult -> string`

### resultSummaryTransformer ###

**resultSummaryTransformer**
- Composes a summary string for a result using provided message and summary functions.
- Signature: `('a -> string) -> (ITestInfo -> string -> string) -> 'a -> ITestInfo -> string`

### defaultTestResultSummaryTransformer ###

**defaultTestResultSummaryTransformer**
- Default summary transformer for test results.
- Signature: `TestResult -> ITestInfo -> string`

### defaultTestExecutionResultSummaryTransformer ###

**defaultTestExecutionResultSummaryTransformer**
- Default summary transformer for test execution results.
- Signature: `TestExecutionResult -> ITestInfo -> string`

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->
<!-- GENERATED DOCUMENT DO NOT EDIT! -->