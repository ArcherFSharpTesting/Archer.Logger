module Archer.Logger.TestContainerReportReporter

open System
open Archer.CoreTypes.InternalTypes
open Archer.Logger.StringHelpers
open Archer.Logger.TestResultReportReporters
open Microsoft.FSharp.Core

let reportFailures (reportFailure: IndentReporter -> TestFailureReport -> string) (indentReporter: IndentReporter) (failures: TestFailureReport list) =
    let reporter = reportFailure indentReporter
    
    failures
    |> List.map (reporter >> appendNewLine)
    |> String.concat Environment.NewLine
    |> trimEnd
    
let reportSuccesses (reportSuccess: IndentReporter -> TestSuccessReport -> string) (indentReporter: IndentReporter) (successes: TestSuccessReport list) =
    let reporter = reportSuccess indentReporter
    
    successes
    |> List.map (reporter >> appendNewLine)
    |> String.concat Environment.NewLine
    |> trimEnd
    
let testContainerPartialReporter<'a> (getReportItems: TestContainerReport -> 'a list) (itemReporter: IndentReporter -> 'a -> string) (itemsReporter: (IndentReporter -> 'a -> string) -> IndentReporter -> 'a list -> string) (indentReporter: IndentReporter) (report: TestContainerReport)  =
    let items = getReportItems report
    let itemsReport = itemsReporter itemReporter (indentReporter.Indent ()) items
    
    [
        indentReporter.Report report.ContainerName
        itemsReport
    ]
    |> String.concat Environment.NewLine

let defaultTestContainerReportFailurePartialReporter (indentReporter: IndentReporter) (report: TestContainerReport) =
    testContainerPartialReporter<TestFailureReport> (fun c -> c.Failures) getShortTitleTestFailureReport reportFailures indentReporter report 
    
let defaultTestContainerReportSuccessPartialReporter (indentReporter: IndentReporter) (report: TestContainerReport) =
    testContainerPartialReporter<TestSuccessReport> (fun c -> c.Successes) getShortTitleTestSuccessReport reportSuccesses indentReporter report
