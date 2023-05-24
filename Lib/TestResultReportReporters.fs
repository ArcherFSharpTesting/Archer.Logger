module Archer.Logger.TestResultReportReporters

open Archer
open Archer.CoreTypes.InternalTypes
open Archer.Logger.Detail

let getTestFailureReportReport (indentReporter: IndentReporter) (result: TestFailureReport) =
    detailedTestExecutionResultReporter indentReporter result.Test (Some result.Time) result.Result
    
let getTestSuccessReportReport (indentReporter: IndentReporter) (result: TestSuccessReport) =
    detailedTestExecutionResultReporter indentReporter result.Test (Some result.Time) (TestSuccess |> TestExecutionResult)