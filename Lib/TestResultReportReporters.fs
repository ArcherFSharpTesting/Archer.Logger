module Archer.Logger.TestResultReportReporters

open System.Reflection
open Archer
open Archer.CoreTypes.InternalTypes
open Archer.Logger.Detail
open Archer.Logger.LocationHelpers

let getTestFailureReportReporter (timingFormatter: TestTiming option -> TimingHeader) (titleFormatter: TimingHeader -> ITestInfo -> string) (pathFormatter: Assembly -> ITestInfo -> string) (resultReporter: Assembly -> IIndentReporter -> TestExecutionResult -> string) (assembly: Assembly) (indentReporter: IIndentReporter) (result: TestFailureReport) =
    detailedTestExecutionResultReporter timingFormatter titleFormatter pathFormatter resultReporter assembly indentReporter result.Test (Some result.Time) result.Result

let getTestSuccessReportReporter (timingFormatter: TestTiming option -> TimingHeader) (titleFormatter: TimingHeader -> ITestInfo -> string) (pathFormatter: Assembly -> ITestInfo -> string) (resultReporter: Assembly -> IIndentReporter -> TestExecutionResult -> string) (assembly: Assembly) (indentReporter: IIndentReporter) (result: TestSuccessReport)=
    detailedTestExecutionResultReporter timingFormatter titleFormatter pathFormatter resultReporter assembly indentReporter result.Test (Some result.Time) (TestSuccess |> TestExecutionResult)
    
let getShortTitleTestFailureReport (indentReporter: IIndentReporter) (result: TestFailureReport) =
    let assembly = Assembly.GetCallingAssembly ()
    getTestFailureReportReporter getTitleTimingString shortTestTitleFormatter getRelativeFilePath getExecutionResultMessage assembly indentReporter result
    
let getShortTitleTestSuccessReport (indentReporter: IIndentReporter) (result: TestSuccessReport) =
    let assembly = Assembly.GetCallingAssembly ()
    getTestSuccessReportReporter getTitleTimingString shortTestTitleFormatter getRelativeFilePath getExecutionResultMessage assembly indentReporter result

let getDefaultTestFailureReportReport (indentReporter: IIndentReporter) (result: TestFailureReport) =
    defaultDetailedTestExecutionResultReporter indentReporter result.Test (Some result.Time) result.Result
    
let getDefaultTestSuccessReportReport (indentReporter: IIndentReporter) (result: TestSuccessReport) =
    defaultDetailedTestExecutionResultReporter indentReporter result.Test (Some result.Time) (TestSuccess |> TestExecutionResult)