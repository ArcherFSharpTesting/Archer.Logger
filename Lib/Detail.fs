[<AutoOpen>]
module Archer.Logger.Detail

open System
open System.IO
open System.Reflection
open Archer
open Archer.Logger.StringHelpers
open Archer.Logger.LocationHelpers

let rec private getExceptionString (indentReporter: IndentReporter) (ex : exn) =
    let inner =
        if (ex.InnerException = null) then ""
        else getExceptionString (indentReporter.Indent ()) ex.InnerException
        
    let exType = ex.GetType()
    [
        indentReporter.Report $"%s{exType.Namespace}.%s{exType.Name}: %s{ex.Message}"
        indentReporter.Indent().Report (ex.StackTrace |> trim)
        inner
    ]
    |> String.concat Environment.NewLine
    |> trimEnd

let private getSetupTeardownFailureMessage (assembly: Assembly) (indentReporter: IndentReporter) name (failure: SetupTeardownFailure) =
    match failure with
    | SetupTeardownExceptionFailure ex ->
        [
            indentReporter.Report name
            getExceptionString (indentReporter.Indent ()) ex
        ]
    | SetupTeardownCanceledFailure ->
        [
            indentReporter.Report $"%s{name}: (Canceled)"
        ]
    | GeneralSetupTeardownFailure (message, location) ->
        let path = getRelativePath assembly (DirectoryInfo $"%s{location.FilePath}%c{Path.DirectorySeparatorChar}%s{location.FileName}")
        [
            indentReporter.Report $"%s{name} @ \"%s{path}@%d{location.LineNumber}\""
            indentReporter.Indent().Report $"General Failure:"
            indentReporter.Indent().Indent().Report message
        ]
    |> String.concat Environment.NewLine
    |> trimEnd

let detailedTestExecutionResultReporter (indentReporter: IndentReporter) (testInfo: ITestInfo) (result: TestExecutionResult) =
    let msg =
        match result with
        | SetupExecutionFailure failure ->
            let assembly = Assembly.GetCallingAssembly ()
            getSetupTeardownFailureMessage assembly (indentReporter.Indent ()) "SetupExecutionFailure" failure
        | TestExecutionResult testResult -> failwith "todo"
        | TeardownExecutionFailure setupTeardownFailure -> failwith "todo"
        | GeneralExecutionFailure generalTestingFailure -> failwith "todo"
    
    [
        indentReporter.Report $"%s{testInfo.ContainerName}.%s{testInfo.TestName} @ %d{testInfo.Location.LineNumber}"
        indentReporter.Report $"(%s{testInfo.Location.FilePath}%c{Path.DirectorySeparatorChar}%s{testInfo.Location.FileName})"
        msg
    ]
    |> String.concat Environment.NewLine