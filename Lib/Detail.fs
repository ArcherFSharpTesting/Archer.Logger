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
    
let rec private getTestExpectationMessage (indentReporter: IndentReporter) (codeLocation: CodeLocation) (failure: TestExpectationFailure) =
    match failure with
    | ExpectationOtherFailure message ->
        [
            indentReporter.Report "Expectation Failure (Other)"
            indentReporter.Indent().Report message
        ]
    | ExpectationVerificationFailure failure ->
        [
            indentReporter.Report "Validation Failure"
            indentReporter.Indent().Report $"Expected: %A{failure.Expected}" |> replace "\"\"" "\""
            indentReporter.Indent().Report $"Actual:   %A{failure.Actual}" |> replace "\"\"" "\""
        ]
    | FailureWithMessage (message, failure) ->
        [
            getTestExpectationMessage indentReporter codeLocation failure
            indentReporter.Report "Failure Comment:"
            indentReporter.Indent().Report message
        ]
    |> String.concat Environment.NewLine

let private getTestFailureMessage assembly (indentReporter: IndentReporter) (failure: TestFailure) =
    
    let rec getTestFailureMessage (indentReporter: IndentReporter) (failure: TestFailure) =
        let message =
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
                [
                    getTestExpectationMessage indentReporter codeLocation failure
                ]
            | CombinationFailure ((failureA, maybeLocationA), (failureB, maybeLocationB)) ->
                let getInfo (maybeLocation: CodeLocation option) =
                    match maybeLocation with
                    | None -> ""
                    | Some value ->
                        let path = getRelativePath assembly (DirectoryInfo value.FilePath)
                        let fullPath = Path.Combine (path, value.FileName)
                        $"%s{fullPath}@%d{value.LineNumber}"
            
                let idA = maybeLocationA |> getInfo
                let idB = maybeLocationB |> getInfo
                
                let lengthMessageA =
                    if 0 < idA.Length
                    then indentReporter.Indent().Indent().Report idA
                    else ""
                    
                let lengthMessageB =
                    if 0 < idB.Length
                    then indentReporter.Indent().Indent().Report idB
                    else ""
                
                [
                    [
                        indentReporter.Report "Combination Failure"
                    ]
                    [
                        getTestFailureMessage (indentReporter.Indent ()) failureA
                        lengthMessageA
                    ] |> List.filter (fun (v: string) -> 0 < v.Length)
                    [
                        getTestFailureMessage (indentReporter.Indent ()) failureB
                        lengthMessageB
                    ] |> List.filter (fun v -> 0 < v.Length)
                ]
                |> List.concat
        message                
        |> String.concat Environment.NewLine
        
    getTestFailureMessage indentReporter failure

let private getTestResultMessage assembly (indentReporter: IndentReporter) (testResult: TestResult) =
    match testResult with
    | TestFailure failure -> getTestFailureMessage assembly indentReporter failure
    | TestSuccess -> indentReporter.Report "Test Result: Success"
    
let private getGeneralTestingFailureMessage assembly (indentReporter: IndentReporter) (testResult: GeneralTestingFailure) =
    match testResult with
    | GeneralFailure message -> failwith "todo"
    | GeneralExceptionFailure e -> failwith "todo"
    | GeneralCancelFailure -> failwith "todo"

let detailedTestExecutionResultReporter (indentReporter: IndentReporter) (testInfo: ITestInfo) (result: TestExecutionResult) =
    let assembly = Assembly.GetCallingAssembly ()
    let msg =
        match result with
        | SetupExecutionFailure failure ->
            getSetupTeardownFailureMessage assembly (indentReporter.Indent ()) "SetupExecutionFailure" failure
        | TestExecutionResult testResult ->
            getTestResultMessage assembly (indentReporter.Indent ()) testResult
        | TeardownExecutionFailure failure ->
            getSetupTeardownFailureMessage assembly (indentReporter.Indent ()) "TeardownExecutionFailure" failure
        | GeneralExecutionFailure failure -> failwith "todo"
    
    let path = getRelativePath assembly (DirectoryInfo $"%s{testInfo.Location.FilePath}%c{Path.DirectorySeparatorChar}")
    let path = Path.Combine (path, testInfo.Location.FileName)
    [
        indentReporter.Report $"%s{testInfo.ContainerName}.%s{testInfo.TestName} @ %d{testInfo.Location.LineNumber}"
        indentReporter.Report $"(%s{path})"
        msg
    ]
    |> String.concat Environment.NewLine