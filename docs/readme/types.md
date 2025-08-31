<!-- (dl
(section-meta
    (title Archer.Reporting Types)
)
) -->

This page provides an overview of the core types and interfaces used by Archer.Reporting for formatting and outputting test results. These types define the structure and contracts for reporting and handling test execution outcomes in the Archer F# test framework.

<!-- (dl (# InformationDensity)) -->
Represents the level of detail for logging information.
- `SummaryDensity`: Summary-level information.
- `DetailedDensity`: Detailed information.

<!-- (dl (# LogScope)) -->
Specifies the context or scope for log messages.
- `ErrorScope`: Error messages.
- `TestFailureScope`: Test failure messages.
- `TestIgnoreScope`: Ignored test messages.
- `TestSuccessScope`: Successful test messages.

<!-- (dl (# ITestReporting)) -->
Interface for logging test information.
- `Log: density: InformationDensity -> scope: LogScope -> string -> unit`

<!-- (dl (# ITestResultReporting)) -->
Interface for logging test results.
- `LogTestResult: density: InformationDensity -> result: TestResult -> unit`

<!-- (dl (# ITestExecutionResultReporting)) -->
Interface for logging test execution results.
- `LogExecutionResult: density: InformationDensity -> result: TestExecutionResult -> unit`

<!-- (dl (# ITestContainerReporting)) -->
Interface for logging collections of test results.
- `LogFailures: density: InformationDensity -> failures: TestFailContainer list -> unit`
- `LogSuccesses: density: InformationDensity -> successes: TestSuccessContainer list -> unit`
- `LogIgnored: density: InformationDensity -> ignored: TestIgnoreContainer list -> unit`
