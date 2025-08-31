/// <summary>
/// Provides functions for transforming test failure containers into formatted strings for logging.
/// </summary>
module Archer.Reporting.TestFailContainerTransformer

open System
open System.Reflection
open Archer
open Archer.Types.InternalTypes
open Archer.Types.InternalTypes.RunnerTypes
open Archer.Reporting.Detail
open Archer.Reporting.LocationHelpers
open Archer.Reporting.StringHelpers

/// <summary>
/// Gets a formatted message for a wrapped test failure.
/// </summary>
/// <param name="assembly">The assembly context.</param>
/// <param name="indenter">The indenter used for formatting.</param>
/// <param name="failure">The test failure to format.</param>
/// <returns>A formatted string representing the test failure.</returns>
let getWrappedTestFailureMessage assembly (indenter: IIndentTransformer) (failure: TestFailure) =
    getTestResultMessage assembly indenter (TestFailure failure)

/// <summary>
/// Gets a formatted message for a general testing failure that should be ignored for assembly context.
/// </summary>
/// <param name="_">The assembly context (ignored).</param>
/// <param name="indenter">The indenter used for formatting.</param>
/// <param name="failure">The general testing failure to format.</param>
/// <returns>A formatted string representing the general testing failure.</returns>
let getIgnoreAssemblyGeneralTestingFailureMessage (_: Assembly) (indenter: IIndentTransformer) (failure: GeneralTestingFailure) =
    getGeneralTestingFailureMessage indenter failure

/// <summary>
/// Transforms a test failure type and test into a formatted string for logging.
/// </summary>
/// <param name="indenter">The indenter used for formatting.</param>
/// <param name="failure">A tuple of the test failure type and the test.</param>
/// <returns>A formatted string representing the test failure.</returns>
let transformTestFailureType (indenter: IIndentTransformer) (failure: TestFailureType, test: ITest) =
    let assembly = Assembly.GetAssembly typeof<IndentTransformer>

    let baseTransformer (resultTransformer: Assembly -> IIndentTransformer -> 'itemType -> string) (result: 'itemType) =
        detailedTestItemTransformer getTitleTimingString shortTestTitleFormatter getRelativeFilePath resultTransformer assembly indenter test None result

    match failure with
    | SetupFailureType failure ->
        baseTransformer (getSetupTeardownFailureMessage "SetupExecutionFailure") failure
    | TestRunFailureType testFailure ->
        baseTransformer getWrappedTestFailureMessage testFailure
    | TeardownFailureType failure ->
        baseTransformer (getSetupTeardownFailureMessage "TeardownExecutionFailure") failure
    | GeneralFailureType failure ->
        baseTransformer getIgnoreAssemblyGeneralTestingFailureMessage failure

/// <summary>
/// Recursively transforms a test failure container into a formatted string for logging.
/// </summary>
/// <param name="testFailureTypeTransformer">A function to transform individual test failure types.</param>
/// <param name="indenter">The indenter used for formatting.</param>
/// <param name="failures">The test failure container to transform.</param>
/// <returns>A formatted string representing all failures in the container.</returns>
let testFailContainerTransformer (testFailureTypeTransformer: IIndentTransformer -> TestFailureType * ITest -> string) (indenter: IIndentTransformer) (failures: TestFailContainer) =
    let rec testFailContainerTransformer acc (indenter: IIndentTransformer) (failures: TestFailContainer) =
        match failures with
        | EmptyFailures -> acc
        | FailedTests failures ->
            let transformer = testFailureTypeTransformer indenter

            let failureString =
                failures
                |> List.map transformer
                |> linesToString

            [
                acc |> appendNewLineIfNotEmpty
                failureString
            ]
            |> List.filter (String.IsNullOrWhiteSpace >> not)
            |> linesToString

        | FailContainer (_, []) -> acc
        | FailContainer (name, testFailContainers) ->
            let transforms =
                testFailContainers
                |> List.map (testFailContainerTransformer "" (indenter.Indent ()))
                |> List.filter (String.IsNullOrWhiteSpace >> not)

            if transforms.IsEmpty then acc
            else
                [
                    acc |> appendNewLineIfNotEmpty
                    indenter.Transform name
                    transforms |> linesToString
                ]
                |> linesToString
                |> trimEnd

    testFailContainerTransformer "" indenter failures

/// <summary>
/// The default transformer for a single test failure container.
/// </summary>
/// <param name="indenter">The indenter used for formatting the output.</param>
/// <param name="failures">The test failure container to transform.</param>
/// <returns>A formatted string representing all failures in the container.</returns>
let defaultTestFailContainerTransformer (indenter: IIndentTransformer) (failures: TestFailContainer) =
    testFailContainerTransformer transformTestFailureType indenter failures

/// <summary>
/// The default transformer for a list of test failure containers.
/// </summary>
/// <param name="indenter">The indenter used for formatting the output.</param>
/// <param name="failures">The list of test failure containers to transform.</param>
/// <returns>A formatted string representing all failures in the list of containers.</returns>
let defaultTestFailContainerAllTransformer (indenter: IIndentTransformer) (failures: TestFailContainer list) =
    failures
    |> List.map (defaultTestFailContainerTransformer indenter)
    |> linesToString