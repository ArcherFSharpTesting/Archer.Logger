
/// <summary>
/// Contains core types for the Archer.Reporting library, including reporting density, scope, and reporting interfaces.
/// </summary>
[<AutoOpen>]
module Archer.Reporting.Types

open Archer
open Archer.Types.InternalTypes.RunnerTypes

/// <summary>
/// Represents the level of detail to include in report output.
/// </summary>
type InformationDensity =
    /// <summary>Summary-level information density (minimal output).</summary>
    | SummaryDensity
    /// <summary>Detailed information density (verbose output).</summary>
    | DetailedDensity

/// <summary>
/// Represents the scope or category of a report entry.
/// </summary>
type LogScope =
    /// <summary>Indicates an error report entry.</summary>
    | ErrorScope
    /// <summary>Indicates a test failure report entry.</summary>
    | TestFailureScope
    /// <summary>Indicates a test ignore report entry.</summary>
    | TestIgnoreScope
    /// <summary>Indicates a test success report entry.</summary>
    | TestSuccessScope

/// <summary>
/// Interface for reporting general test information.
/// </summary>
type ITestLogger =
    /// <summary>
    /// Reports a message with the specified information density and scope.
    /// </summary>
    /// <param name="density">The level of detail for the report output.</param>
    /// <param name="scope">The category or scope of the report entry.</param>
    /// <param name="message">The message to report.</param>
    abstract member Log: density: InformationDensity -> scope: LogScope -> string -> unit

/// <summary>
/// Interface for reporting test results.
/// </summary>
type ITestResultLogger =
    /// <summary>
    /// Reports a test result with the specified information density.
    /// </summary>
    /// <param name="density">The level of detail for the report output.</param>
    /// <param name="result">The test result to report.</param>
    abstract member LogTestResult: density: InformationDensity -> result: TestResult -> unit

/// <summary>
/// Interface for reporting test execution results.
/// </summary>
type ITestExecutionResultLogger =
    /// <summary>
    /// Reports a test execution result with the specified information density.
    /// </summary>
    /// <param name="density">The level of detail for the report output.</param>
    /// <param name="result">The test execution result to report.</param>
    abstract member LogExecutionResult: density: InformationDensity -> result: TestExecutionResult -> unit

/// <summary>
/// Interface for reporting collections of test results, such as failures, successes, and ignored tests.
/// </summary>
type ITestContainerLogger =
    /// <summary>
    /// Reports a list of test failures with the specified information density.
    /// </summary>
    /// <param name="density">The level of detail for the report output.</param>
    /// <param name="failures">The list of test failure containers to report.</param>
    abstract member LogFailures: density: InformationDensity -> failures: TestFailContainer list -> unit
    /// <summary>
    /// Reports a list of test successes with the specified information density.
    /// </summary>
    /// <param name="density">The level of detail for the report output.</param>
    /// <param name="successes">The list of test success containers to report.</param>
    abstract member LogSuccesses: density: InformationDensity -> successes: TestSuccessContainer list -> unit
    /// <summary>
    /// Reports a list of ignored tests with the specified information density.
    /// </summary>
    /// <param name="density">The level of detail for the report output.</param>
    /// <param name="ignored">The list of ignored test containers to report.</param>
    abstract member LogIgnored: density: InformationDensity -> ignored: TestIgnoreContainer list -> unit