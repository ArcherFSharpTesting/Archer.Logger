/// <summary>
/// Provides functions for transforming test ignore containers into formatted strings for logging.
/// </summary>
module Archer.Reporting.TestIgnoreContainerTransformer

open System
open System.Reflection
open Archer
open Archer.CoreTypes.InternalTypes.RunnerTypes
open Archer.Reporting.Detail
open Archer.Reporting.LocationHelpers
open Archer.Reporting.StringHelpers
open Microsoft.FSharp.Core

/// <summary>
/// Transforms an ignored test into a formatted string for logging.
/// </summary>
/// <param name="assembly">The assembly context.</param>
/// <param name="indenter">The indenter used for formatting.</param>
/// <param name="message">An optional ignore message.</param>
/// <param name="location">The code location of the test.</param>
/// <param name="test">The test information object.</param>
/// <returns>A formatted string representing the ignored test.</returns>
let transformIgnoredTest (assembly: Assembly) (indenter: IIndentTransformer) (message: string option, location: CodeLocation, test: ITestInfo) =
    let msg =
        match message with
        | None -> ""
        | Some value -> $" %A{value}"

    let name = shortTestTitleFormatter "" test
    let locationStr =
        if location = test.Location then ""
        else $" @ %d{location.LineNumber}"

    let relativeFilePath = (getRelativeFilePath assembly test)

    [
        indenter.Transform $"%s{name}: (Ignored%s{msg}%s{locationStr})"
        indenter.Indent().Transform $"(%s{relativeFilePath})"
    ]
    |> linesToString

/// <summary>
/// Recursively transforms a test ignore container into a formatted string for logging.
/// </summary>
/// <param name="ignoreTransformer">A function to transform individual ignored tests.</param>
/// <param name="assembly">The assembly context.</param>
/// <param name="indenter">The indenter used for formatting.</param>
/// <param name="container">The test ignore container to transform.</param>
/// <returns>A formatted string representing all ignored tests in the container.</returns>
let assemblyTestIgnoreContainerTransformer (ignoreTransformer: Assembly -> IIndentTransformer -> string option * CodeLocation * ITestInfo -> string) (assembly: Assembly) (indenter: IIndentTransformer) (container: TestIgnoreContainer) =
    let rec assemblyTestIgnoreContainerTransformer (indenter: IIndentTransformer) (container: TestIgnoreContainer) =
        match container with
        | EmptyIgnore -> ""
        | IgnoredTests tuples ->
            tuples
            |> List.map (ignoreTransformer assembly indenter)
            |> linesToString

        | IgnoreContainer (_, []) -> ""
        | IgnoreContainer (name, containers) ->
            let body = 
                containers |> List.map (assemblyTestIgnoreContainerTransformer (indenter.Indent ()) >> trimEnd >> appendNewLineIfNotEmpty)
                |> List.filter (String.IsNullOrWhiteSpace >> not)
                |> linesToString
                |> trimEnd

            [
                indenter.Transform name
                body
            ]
            |> linesToString

    assemblyTestIgnoreContainerTransformer indenter container
    |> trimEnd

/// <summary>
/// The default transformer for a single test ignore container with assembly context.
/// </summary>
/// <param name="assembly">The assembly context.</param>
/// <param name="indenter">The indenter used for formatting.</param>
/// <param name="container">The test ignore container to transform.</param>
/// <returns>A formatted string representing all ignored tests in the container.</returns>
let defaultAssemblyTestIgnoreContainerTransformer (assembly: Assembly) (indenter: IIndentTransformer) (container: TestIgnoreContainer) =
    assemblyTestIgnoreContainerTransformer transformIgnoredTest assembly indenter container 

/// <summary>
/// The default transformer for a single test ignore container using the calling assembly.
/// </summary>
/// <param name="indenter">The indenter used for formatting.</param>
/// <param name="container">The test ignore container to transform.</param>
/// <returns>A formatted string representing all ignored tests in the container.</returns>
let defaultTestIgnoreContainerTransformer (indenter: IIndentTransformer) (container: TestIgnoreContainer) =
    let assembly = Assembly.GetCallingAssembly ()
    defaultAssemblyTestIgnoreContainerTransformer assembly indenter container
    |> trimEnd

/// <summary>
/// The default transformer for a list of test ignore containers using the calling assembly.
/// </summary>
/// <param name="indenter">The indenter used for formatting.</param>
/// <param name="containers">The list of test ignore containers to transform.</param>
/// <returns>A formatted string representing all ignored tests in the list of containers.</returns>
let defaultAllTestIgnoreContainerTransformer (indenter: IIndentTransformer) (containers: TestIgnoreContainer list) =
    let assembly = Assembly.GetCallingAssembly ()

    containers
    |> List.map (defaultAssemblyTestIgnoreContainerTransformer assembly indenter >> appendNewLine)
    |> List.filter (String.IsNullOrWhiteSpace >> not)
    |> linesToString
    |> trimEnd