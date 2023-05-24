module Archer.Logger.TestResultReportReporters

open System
open Archer
open Archer.CoreTypes.InternalTypes
open Archer.Logger.Detail

let getTestFailureReportReport (indentReporter: IndentReporter) (result: TestFailureReport) =
    [
        indentReporter.Report $"Execution Time: %A{result.Time.Total}"
        detailedTestExecutionResultReporter indentReporter result.Test None result.Result
    ]
    |> String.concat Environment.NewLine
    
let getTestSuccessReportReport (indentReporter: IndentReporter) (result: TestSuccessReport) =
    detailedTestExecutionResultReporter indentReporter result.Test (Some result.Time) (TestSuccess |> TestExecutionResult)