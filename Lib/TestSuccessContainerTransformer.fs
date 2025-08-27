/// <summary>
/// Provides functions for transforming test success containers into formatted strings for logging.
/// </summary>
module Archer.Logger.TestSuccessContainerTransformer

open System
open System.Reflection
open Archer
open Archer.CoreTypes.InternalTypes.RunnerTypes
open Archer.Logger.Detail
open Archer.Logger.StringHelpers
open Archer.Logger.LocationHelpers

/// <summary>
/// Transforms a test success container into a formatted string for logging, using the provided title and path formatters and assembly context.
/// </summary>
/// <param name="titleFormatter">A function to format the test title.</param>
/// <param name="pathFormatter">A function to format the test path.</param>
/// <param name="assembly">The assembly context.</param>
/// <param name="indenter">The indenter used for formatting.</param>
/// <param name="successContainer">The test success container to transform.</param>
/// <returns>A formatted string representing all successes in the container.</returns>
let testAssemblySuccessContainerTransformer (titleFormatter: string -> ITestInfo -> string) (pathFormatter: Assembly -> ITestLocationInfo -> string) (assembly: Assembly) (indenter: IIndentTransformer) (successContainer: TestSuccessContainer) =
    let rec testSuccessContainerTransformer (indenter: IIndentTransformer) (successContainer: TestSuccessContainer) =
        match successContainer with
        | EmptySuccesses -> ""
        | SucceededTests tests ->
            tests
            |> List.map (fun testInfo ->
                let title = titleFormatter "" testInfo
                let path = pathFormatter assembly testInfo

                [
                    indenter.Transform $"%s{title}: Success"
                    indenter.Indent().Transform $"(%s{path})"
                ]
                |> linesToString
            )
            |> linesToString
        | SuccessContainer (name, testSuccessContainers) ->
            let details =
                testSuccessContainers
                |> List.map (testSuccessContainerTransformer (indenter.Indent ()) >> appendNewLine)
                |> List.filter (String.IsNullOrWhiteSpace >> not)
                |> linesToString

            [
                indenter.Transform name
                details
            ]
            |> linesToString

    testSuccessContainerTransformer indenter successContainer
    |> trimEnd

/// <summary>
/// Transforms a test success container into a formatted string for logging, using the calling assembly.
/// </summary>
/// <param name="titleFormatter">A function to format the test title.</param>
/// <param name="pathFormatter">A function to format the test path.</param>
/// <param name="indenter">The indenter used for formatting.</param>
/// <param name="successContainer">The test success container to transform.</param>
/// <returns>A formatted string representing all successes in the container.</returns>
let singleTestAssemblySuccessContainerTransformer (titleFormatter: string -> ITestInfo -> string) (pathFormatter: Assembly -> ITestLocationInfo -> string) (indenter: IIndentTransformer) (successContainer: TestSuccessContainer) =
    let assembly = Assembly.GetCallingAssembly ()

    testAssemblySuccessContainerTransformer titleFormatter pathFormatter assembly indenter successContainer

/// <summary>
/// The default transformer for a single test success container using the calling assembly.
/// </summary>
/// <param name="indenter">The indenter used for formatting.</param>
/// <param name="successContainer">The test success container to transform.</param>
/// <returns>A formatted string representing all successes in the container.</returns>
let defaultSingleTestSuccessContainerTransformer (indenter: IIndentTransformer) (successContainer: TestSuccessContainer) =
    singleTestAssemblySuccessContainerTransformer shortTestTitleFormatter getRelativeFilePath indenter successContainer

/// <summary>
/// The default transformer for a single test success container with explicit assembly context.
/// </summary>
/// <param name="assembly">The assembly context.</param>
/// <param name="indenter">The indenter used for formatting.</param>
/// <param name="successContainer">The test success container to transform.</param>
/// <returns>A formatted string representing all successes in the container.</returns>
let defaultTestSuccessContainer (assembly: Assembly) (indenter: IIndentTransformer) (successContainer: TestSuccessContainer) =
    testAssemblySuccessContainerTransformer shortTestTitleFormatter getRelativeFilePath assembly indenter successContainer

/// <summary>
/// The default transformer for a list of test success containers using the calling assembly.
/// </summary>
/// <param name="indenter">The indenter used for formatting.</param>
/// <param name="successContainers">The list of test success containers to transform.</param>
/// <returns>A formatted string representing all successes in the list of containers.</returns>
let defaultAllTestSuccessContainerTransformer (indenter: IIndentTransformer) (successContainers: TestSuccessContainer list) =
    successContainers
    |> List.map (defaultSingleTestSuccessContainerTransformer indenter >> appendNewLine)
    |> linesToString
    |> trimEnd