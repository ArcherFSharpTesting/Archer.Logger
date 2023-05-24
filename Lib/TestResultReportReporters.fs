module Archer.Logger.TestResultReportReporters

open System
open Archer.CoreTypes.InternalTypes
open Archer.Logger.Detail

let getTestFailureReport (indentReporter: IndentReporter) (result: TestFailureReport) =
    [
        indentReporter.Report $"Execution Time: %A{result.Time.Total}"
        detailedTestExecutionResultReporter indentReporter result.Test result.Result
    ]
    |> String.concat Environment.NewLine