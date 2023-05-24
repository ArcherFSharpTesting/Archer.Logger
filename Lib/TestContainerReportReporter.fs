module Archer.Logger.TestContainerReportReporter

open System
open Archer
open Archer.CoreTypes.InternalTypes
open Archer.Logger.StringHelpers
open Archer.Logger.TestResultReportReporters
open Microsoft.FSharp.Core

let reportFailures (reportFailure: IIndentReporter -> TestFailureReport -> string) (indentReporter: IIndentReporter) (failures: TestFailureReport list) =
    let reporter = reportFailure indentReporter
    
    failures
    |> List.map (reporter >> appendNewLine)
    |> String.concat Environment.NewLine
    |> trimEnd
    
let reportSuccesses (reportSuccess: IIndentReporter -> TestSuccessReport -> string) (indentReporter: IIndentReporter) (successes: TestSuccessReport list) =
    let reporter = reportSuccess indentReporter
    
    successes
    |> List.map (reporter >> appendNewLine)
    |> String.concat Environment.NewLine
    |> trimEnd
    
let testContainerPartialReporter<'a> (getReportItems: TestContainerReport -> 'a list) (itemReporter: IIndentReporter -> 'a -> string) (itemsReporter: (IIndentReporter -> 'a -> string) -> IIndentReporter -> 'a list -> string) (indentReporter: IIndentReporter) (report: TestContainerReport)  =
    let items = getReportItems report
    let itemsReport = itemsReporter itemReporter (indentReporter.Indent ()) items
    
    [
        indentReporter.Report report.ContainerName
        itemsReport
    ]
    |> String.concat Environment.NewLine

let defaultTestContainerReportFailurePartialReporter (indentReporter: IIndentReporter) (report: TestContainerReport) =
    testContainerPartialReporter<TestFailureReport> (fun c -> c.Failures) getShortTitleTestFailureReport reportFailures indentReporter report 
    
let defaultTestContainerReportSuccessPartialReporter (indentReporter: IIndentReporter) (report: TestContainerReport) =
    testContainerPartialReporter<TestSuccessReport> (fun c -> c.Successes) getShortTitleTestSuccessReport reportSuccesses indentReporter report
    
let private getRootNamePath (name: string) (fullNamePath: string) =
    fullNamePath.Replace (name, "")
    |> removeLastChar
    |> trim
    
let defaultTestContainerReportFailureReporter (indenter: IIndentReporter) (report: TestContainerReport) =
    let namePath =
        report.ContainerFullName
        |> getRootNamePath report.ContainerName
    
    let l1 = indenter.Indent()
    [
        indenter.Report namePath
        l1.Report "Failures"
        defaultTestContainerReportFailurePartialReporter (l1.Indent ()) report
    ]
    |> linesToString

let defaultTestContainerReportSuccessReporter (indenter: IIndentReporter) (report: TestContainerReport) =
    let namePath =
        report.ContainerFullName
        |> getRootNamePath report.ContainerName
    
    let l1 = indenter.Indent()
    [
        indenter.Report namePath
        l1.Report "Successes"
        defaultTestContainerReportSuccessPartialReporter (l1.Indent ()) report
    ]
    |> linesToString
    