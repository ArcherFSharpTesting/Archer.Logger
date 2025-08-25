<!-- GENERATED DOCUMENT DO NOT EDIT! -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->

<!-- Compiled with doculisp https://www.npmjs.com/package/doculisp -->

# Archer.Logger: Test Output Logger for F# #

1. Overview: [Archer.Logger Overview](#archerlogger-overview)
2. Types: [Archer.Logger Types](#archerlogger-types)

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

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->
<!-- GENERATED DOCUMENT DO NOT EDIT! -->