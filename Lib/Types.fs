
/// <summary>
/// Contains core types for the Archer.Logger library, including logging density, scope, and logger interfaces.
/// </summary>
[<AutoOpen>]
module Archer.Logger.Types

open Archer
open Archer.Types.InternalTypes.RunnerTypes

/// <summary>
/// Represents the level of detail to include in log output.
/// </summary>
type InformationDensity =
    /// <summary>Summary-level information density (minimal output).</summary>
    | SummaryDensity
    /// <summary>Detailed information density (verbose output).</summary>
    | DetailedDensity

/// <summary>
/// Represents the scope or category of a log entry.
/// </summary>
type LogScope =
    /// <summary>Indicates an error log entry.</summary>
    | ErrorScope
    /// <summary>Indicates a test failure log entry.</summary>
    | TestFailureScope
    /// <summary>Indicates a test ignore log entry.</summary>
    | TestIgnoreScope
    /// <summary>Indicates a test success log entry.</summary>
    | TestSuccessScope

/// <summary>
/// Interface for logging general test information.
/// </summary>
type ITestLogger =
    /// <summary>
    /// Logs a message with the specified information density and scope.
    /// </summary>
    /// <param name="density">The level of detail for the log output.</param>
    /// <param name="scope">The category or scope of the log entry.</param>
    /// <param name="message">The message to log.</param>
    abstract member Log: density: InformationDensity -> scope: LogScope -> string -> unit

/// <summary>
/// Interface for logging test results.
/// </summary>
type ITestResultLogger =
    /// <summary>
    /// Logs a test result with the specified information density.
    /// </summary>
    /// <param name="density">The level of detail for the log output.</param>
    /// <param name="result">The test result to log.</param>
    abstract member LogTestResult: density: InformationDensity -> result: TestResult -> unit

/// <summary>
/// Interface for logging test execution results.
/// </summary>
type ITestExecutionResultLogger =
    /// <summary>
    /// Logs a test execution result with the specified information density.
    /// </summary>
    /// <param name="density">The level of detail for the log output.</param>
    /// <param name="result">The test execution result to log.</param>
    abstract member LogExecutionResult: density: InformationDensity -> result: TestExecutionResult -> unit

/// <summary>
/// Interface for logging collections of test results, such as failures, successes, and ignored tests.
/// </summary>
type ITestContainerLogger =
    /// <summary>
    /// Logs a list of test failures with the specified information density.
    /// </summary>
    /// <param name="density">The level of detail for the log output.</param>
    /// <param name="failures">The list of test failure containers to log.</param>
    abstract member LogFailures: density: InformationDensity -> failures: TestFailContainer list -> unit
    /// <summary>
    /// Logs a list of test successes with the specified information density.
    /// </summary>
    /// <param name="density">The level of detail for the log output.</param>
    /// <param name="successes">The list of test success containers to log.</param>
    abstract member LogSuccesses: density: InformationDensity -> successes: TestSuccessContainer list -> unit
    /// <summary>
    /// Logs a list of ignored tests with the specified information density.
    /// </summary>
    /// <param name="density">The level of detail for the log output.</param>
    /// <param name="ignored">The list of ignored test containers to log.</param>
    abstract member LogIgnored: density: InformationDensity -> ignored: TestIgnoreContainer list -> unit