﻿[<AutoOpen>]
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
    
let getExceptionDetail (indentReporter: IndentReporter) name (ex: Exception) =
    [
        indentReporter.Report name
        getExceptionString (indentReporter.Indent ()) ex
    ]
    |> String.concat Environment.NewLine
    
let private getSetupTeardownFailureMessage (assembly: Assembly) (indentReporter: IndentReporter) name (failure: SetupTeardownFailure) =
    match failure with
    | SetupTeardownExceptionFailure ex ->
        [
            getExceptionDetail indentReporter name ex
        ]
    | SetupTeardownCanceledFailure ->
        [
            indentReporter.Report $"%s{name}: (Canceled)"
        ]
    | GeneralSetupTeardownFailure (message, location) ->
        let path = getRelativePath assembly (DirectoryInfo $"%s{location.FilePath}%c{Path.DirectorySeparatorChar}")
        let path = Path.Combine (path, location.FileName)
        [
            indentReporter.Report $"%s{name} @ \"%s{path}@%d{location.LineNumber}\""
            indentReporter.Indent().Report $"General Failure:"
            indentReporter.Indent().Indent().Report message
        ]
    |> String.concat Environment.NewLine
    |> trimEnd
    
let private getTestExpectationMessage (indentReporter: IndentReporter) (codeLocation: CodeLocation) (failure: TestExpectationFailure) =
    match failure with
    | FailureWithMessage (message, failure) -> failwith "todo"
    | ExpectationVerificationFailure failure -> failwith "todo"
    | ExpectationOtherFailure message -> failwith "todo"
    
let private getTestFailureMessage (indentReporter: IndentReporter) (failure: TestFailure) =
    
    let rec getTestFailureMessage (indentReporter: IndentReporter) (failure: TestFailure) =
        match failure with
        | TestExceptionFailure ex ->
            [
                getExceptionDetail indentReporter "Test Failure" ex
            ]
        | TestIgnored (message, codeLocation) ->
            let msg =
                match message with
                | None -> "Ignored"
                | Some value -> $"Ignored %A{value}"
            [
                indentReporter.Report $"Test Failure: (%s{msg}) @%d{codeLocation.LineNumber}"
            ]
        | TestExpectationFailure (failure, codeLocation) ->
            getTestExpectationMessage indentReporter codeLocation failure

        | CombinationFailure (failureA, failureB) -> failwith "todo"
        |> String.concat Environment.NewLine
        
    getTestFailureMessage indentReporter failure

let private getTestResultMessage (indentReporter: IndentReporter) (testResult: TestResult) =
    match testResult with
    | TestFailure failure -> getTestFailureMessage indentReporter failure
    | TestSuccess -> failwith "todo"

let detailedTestExecutionResultReporter (indentReporter: IndentReporter) (testInfo: ITestInfo) (result: TestExecutionResult) =
    let assembly = Assembly.GetCallingAssembly ()
    let msg =
        match result with
        | SetupExecutionFailure failure ->
            getSetupTeardownFailureMessage assembly (indentReporter.Indent ()) "SetupExecutionFailure" failure
        | TestExecutionResult testResult ->
            getTestResultMessage (indentReporter.Indent ()) testResult
        | TeardownExecutionFailure setupTeardownFailure -> failwith "todo"
        | GeneralExecutionFailure generalTestingFailure -> failwith "todo"
    
    let path = getRelativePath assembly (DirectoryInfo $"%s{testInfo.Location.FilePath}%c{Path.DirectorySeparatorChar}")
    let path = Path.Combine (path, testInfo.Location.FileName)
    [
        indentReporter.Report $"%s{testInfo.ContainerName}.%s{testInfo.TestName} @ %d{testInfo.Location.LineNumber}"
        indentReporter.Report $"(%s{path})"
        msg
    ]
    |> String.concat Environment.NewLine