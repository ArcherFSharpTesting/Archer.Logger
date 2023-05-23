[<AutoOpen>]
module Archer.Logger.Detail

open System
open Archer
open Archer.Logger.StringHelpers

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

let private getSetupTeardownFailureMessage (indentReporter: IndentReporter) name (failure: SetupTeardownFailure) =
    match failure with
    | SetupTeardownExceptionFailure ex ->
        [
            indentReporter.Report name
            getExceptionString (indentReporter.Indent ()) ex
        ]
    | SetupTeardownCanceledFailure -> failwith "todo"
    | GeneralSetupTeardownFailure(message, location) -> failwith "todo"
    |> String.concat Environment.NewLine
    |> trimEnd

let detailedTestExecutionResultReporter (indentReporter: IndentReporter) (testInfo: ITestInfo) (result: TestExecutionResult) =
    let msg =
        match result with
        | SetupExecutionFailure failure -> getSetupTeardownFailureMessage (indentReporter.Indent ()) "SetupExecutionFailure" failure
        | TestExecutionResult testResult -> failwith "todo"
        | TeardownExecutionFailure setupTeardownFailure -> failwith "todo"
        | GeneralExecutionFailure generalTestingFailure -> failwith "todo"
    
    [
        indentReporter.Report $"%s{testInfo.ContainerName}.%s{testInfo.TestName} @ %d{testInfo.Location.LineNumber}"
        indentReporter.Report $"(%s{testInfo.Location.FilePath}%c{System.IO.Path.PathSeparator}%s{testInfo.Location.FileName})"
        msg
    ]
    |> String.concat Environment.NewLine