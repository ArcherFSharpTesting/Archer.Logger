module Archer.Logger.TestContainerReportReporter

open System
open Archer.CoreTypes.InternalTypes
open Archer.Logger.StringHelpers
open Archer.Logger.TestResultReportReporters

let testContainerReportFailurePartialReporter (indentReporter: IndentReporter) (report: TestContainerReport) =
    let failures =
        report.Failures
        |> List.map ((getShortTitleTestFailureReport (indentReporter.Indent ())) >> fun value -> $"%s{value}%s{Environment.NewLine}")
        |> String.concat Environment.NewLine
        |> trimEnd
        
    [
        indentReporter.Report report.ContainerName
        failures
    ]
    |> String.concat Environment.NewLine
    
let testContainerReportSuccessPartialReporter (indentReporter: IndentReporter) (report: TestContainerReport) =
    let successes =
        report.Successes
        |> List.map ((getShortTitleTestSuccessReport (indentReporter.Indent ())) >> fun value -> $"%s{value}%s{Environment.NewLine}")
        |> String.concat Environment.NewLine
        |> trimEnd
        
    [
        indentReporter.Report report.ContainerName
        successes
    ]
    |> String.concat Environment.NewLine
