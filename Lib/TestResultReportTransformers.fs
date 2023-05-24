module Archer.Logger.TestResultReportTransformers

open System.Reflection
open Archer
open Archer.CoreTypes.InternalTypes
open Archer.Logger.Detail
open Archer.Logger.LocationHelpers

let getTestFailureReportTransformer (timingFormatter: TestTiming option -> TimingHeader) (titleFormatter: TimingHeader -> ITestInfo -> string) (pathFormatter: Assembly -> ITestInfo -> string) (resultTransformer: Assembly -> IIndentTransformer -> TestExecutionResult -> string) (assembly: Assembly) (indenter: IIndentTransformer) (result: TestFailureReport) =
    detailedTestExecutionResultTransformer timingFormatter titleFormatter pathFormatter resultTransformer assembly indenter result.Test (Some result.Time) result.Result

let getTestSuccessReportTransformer (timingFormatter: TestTiming option -> TimingHeader) (titleFormatter: TimingHeader -> ITestInfo -> string) (pathFormatter: Assembly -> ITestInfo -> string) (resultTransformer: Assembly -> IIndentTransformer -> TestExecutionResult -> string) (assembly: Assembly) (indenter: IIndentTransformer) (result: TestSuccessReport)=
    detailedTestExecutionResultTransformer timingFormatter titleFormatter pathFormatter resultTransformer assembly indenter result.Test (Some result.Time) (TestSuccess |> TestExecutionResult)
    
let getShortTitleTestFailureTransformer (indenter: IIndentTransformer) (result: TestFailureReport) =
    let assembly = Assembly.GetCallingAssembly ()
    getTestFailureReportTransformer getTitleTimingString shortTestTitleFormatter getRelativeFilePath getExecutionResultMessage assembly indenter result
    
let getShortTitleTestSuccessTransformer (indenter: IIndentTransformer) (result: TestSuccessReport) =
    let assembly = Assembly.GetCallingAssembly ()
    getTestSuccessReportTransformer getTitleTimingString shortTestTitleFormatter getRelativeFilePath getExecutionResultMessage assembly indenter result

let getDefaultTestFailureReportTransformer (indenter: IIndentTransformer) (result: TestFailureReport) =
    defaultDetailedTestExecutionResultTransformer indenter result.Test (Some result.Time) result.Result
    
let getDefaultTestSuccessReportTransformer (indenter: IIndentTransformer) (result: TestSuccessReport) =
    defaultDetailedTestExecutionResultTransformer indenter result.Test (Some result.Time) (TestSuccess |> TestExecutionResult)