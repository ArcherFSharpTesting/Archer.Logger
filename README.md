<!-- GENERATED DOCUMENT DO NOT EDIT! -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->

<!-- Compiled with doculisp https://www.npmjs.com/package/doculisp -->

# Archer.Reporting: Test Output Reporter for F# #

1. Overview: [Archer.Reporting Overview](#archerreporting-overview)
2. Types: [Archer.Reporting Types](#archerreporting-types)
3. Helpers: [Archer.Reporting String Helpers](#archerreporting-string-helpers)
4. Helpers: [Archer.Reporting Summaries](#archerreporting-summaries)
5. Helpers: [Archer.Reporting Location Helpers](#archerreporting-location-helpers)
6. Transformers: [Archer.Reporting Test Fail Container Transformer](#archerreporting-test-fail-container-transformer)
7. Transformers: [Archer.Reporting Test Ignore Container Transformer](#archerreporting-test-ignore-container-transformer)
8. Transformers: [Archer.Reporting Test Success Container Transformer](#archerreporting-test-success-container-transformer)

## Archer.Reporting Overview ##

Archer.Reporting is a component of the Archer F# test framework responsible for formatting and outputting test results. It provides clear, structured, and customizable output for test execution, making it easier to interpret test results and debug issues.

### Features ###

- Formats test results for readability
- Supports summaries and detailed output
- Integrates with Archer test framework

### Usage ###

Refer to the main README for setup and integration instructions.

---

*This is an overview document for the Archer.Reporting component. For more details, see the main documentation and source code.*

## Archer.Reporting Types ##

This page provides an overview of the core types and interfaces used by Archer.Reporting for formatting and outputting test results. These types define the structure and contracts for reporting and handling test execution outcomes in the Archer F# test framework.

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

## Archer.Reporting String Helpers ##

This document describes the string helper functions provided in the Archer.Reporting library. These helpers are used for common string manipulations and formatting tasks within the reporter.

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

## Archer.Reporting Summaries ##

This document describes the summary transformation functions in Archer.Reporting. These functions are responsible for generating concise, human-readable summaries of test results and execution outcomes.

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

## Archer.Reporting Location Helpers ##

This document describes the location helper functions in Archer.Reporting. These helpers are used to determine solution roots and generate relative file paths for test reporting and output.

### getSolutionRoot ###

**getSolutionRoot**
- Finds the root directory of the solution by searching for a `.sln` file, starting from the given assembly's location and moving up the directory tree.
- Signature: `Assembly -> string`

### getRelativePath ###

**getRelativePath**
- Returns the relative path from the solution root to the given directory.
- Signature: `Assembly -> DirectoryInfo -> string`

### getRelativePathFromPath ###

**getRelativePathFromPath**
- Returns the relative path from the solution root to the directory specified by a path string.
- Signature: `Assembly -> string -> string`

### getRelativePathFromTest ###

**getRelativePathFromTest**
- Returns the relative path from the solution root to the file location of a test.
- Signature: `Assembly -> ITestLocationInfo -> string`

### getRelativeFilePath ###

**getRelativeFilePath**
- Returns the relative file path (including file name) for a test's location.
- Signature: `Assembly -> ITestLocationInfo -> string`

## Archer.Reporting Test Fail Container Transformer ##

This document describes the Test Fail Container Transformer in Archer.Reporting. This module provides functions to transform and format test failure containers for reporting and output.

### getWrappedTestFailureMessage ###

**getWrappedTestFailureMessage**
- Wraps a test failure in a formatted message using the provided indenter and assembly.
- Signature: `Assembly -> IIndentTransformer -> TestFailure -> string`

### getIgnoreAssemblyGeneralTestingFailureMessage ###

**getIgnoreAssemblyGeneralTestingFailureMessage**
- Formats a general testing failure message for ignored assemblies.
- Signature: `Assembly -> IIndentTransformer -> GeneralTestingFailure -> string`

### transformTestFailureType ###

**transformTestFailureType**
- Transforms a test failure type and test into a detailed string using the appropriate transformer for each failure type.
- Signature: `IIndentTransformer -> (TestFailureType * ITest) -> string`

### testFailContainerTransformer ###

**testFailContainerTransformer**
- Recursively transforms a `TestFailContainer` into a formatted string, handling nested containers and failures.
- Signature: `(IIndentTransformer -> (TestFailureType * ITest) -> string) -> IIndentTransformer -> TestFailContainer -> string`

### defaultTestFailContainerTransformer ###

**defaultTestFailContainerTransformer**
- Default transformer for a single `TestFailContainer` using the standard failure type transformer.
- Signature: `IIndentTransformer -> TestFailContainer -> string`

### defaultTestFailContainerAllTransformer ###

**defaultTestFailContainerAllTransformer**
- Transforms a list of `TestFailContainer` values into a single formatted string.
- Signature: `IIndentTransformer -> TestFailContainer list -> string`

## Archer.Reporting Test Ignore Container Transformer ##

This document describes the Test Ignore Container Transformer in Archer.Reporting. This module provides functions to transform and format test ignore containers for reporting and output.

### getWrappedTestIgnoreMessage ###

**getWrappedTestIgnoreMessage**
- Wraps a test ignore in a formatted message using the provided indenter and assembly.
- Signature: `Assembly -> IIndentTransformer -> TestIgnore -> string`

### transformTestIgnoreType ###

**transformTestIgnoreType**
- Transforms a test ignore type and test into a detailed string using the appropriate transformer for each ignore type.
- Signature: `IIndentTransformer -> (TestIgnoreType * ITest) -> string`

### testIgnoreContainerTransformer ###

**testIgnoreContainerTransformer**
- Recursively transforms a `TestIgnoreContainer` into a formatted string, handling nested containers and ignores.
- Signature: `(IIndentTransformer -> (TestIgnoreType * ITest) -> string) -> IIndentTransformer -> TestIgnoreContainer -> string`

### defaultTestIgnoreContainerTransformer ###

**defaultTestIgnoreContainerTransformer**
- Default transformer for a single `TestIgnoreContainer` using the standard ignore type transformer.
- Signature: `IIndentTransformer -> TestIgnoreContainer -> string`

### defaultTestIgnoreContainerAllTransformer ###

**defaultTestIgnoreContainerAllTransformer**
- Transforms a list of `TestIgnoreContainer` values into a single formatted string.
- Signature: `IIndentTransformer -> TestIgnoreContainer list -> string`

## Archer.Reporting Test Success Container Transformer ##

This document describes the Test Success Container Transformer in Archer.Reporting. This module provides functions to transform and format test success containers for reporting and output.

### getWrappedTestSuccessMessage ###

**getWrappedTestSuccessMessage**
- Wraps a test success in a formatted message using the provided indenter and assembly.
- Signature: `Assembly -> IIndentTransformer -> TestSuccess -> string`

### transformTestSuccessType ###

**transformTestSuccessType**
- Transforms a test success type and test into a detailed string using the appropriate transformer for each success type.
- Signature: `IIndentTransformer -> (TestSuccessType * ITest) -> string`

### testSuccessContainerTransformer ###

**testSuccessContainerTransformer**
- Recursively transforms a `TestSuccessContainer` into a formatted string, handling nested containers and successes.
- Signature: `(IIndentTransformer -> (TestSuccessType * ITest) -> string) -> IIndentTransformer -> TestSuccessContainer -> string`

### defaultTestSuccessContainerTransformer ###

**defaultTestSuccessContainerTransformer**
- Default transformer for a single `TestSuccessContainer` using the standard success type transformer.
- Signature: `IIndentTransformer -> TestSuccessContainer -> string`

### defaultTestSuccessContainerAllTransformer ###

**defaultTestSuccessContainerAllTransformer**
- Transforms a list of `TestSuccessContainer` values into a single formatted string.
- Signature: `IIndentTransformer -> TestSuccessContainer list -> string`

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->
<!-- GENERATED DOCUMENT DO NOT EDIT! -->