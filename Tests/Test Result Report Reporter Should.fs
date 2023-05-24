module Archer.Logger.Tests.``Test Result Report Reporter Should``

open System
open Archer
open Archer.Arrows
open Archer.ApprovalsSupport
open Archer.CoreTypes.InternalTypes
open Archer.Fletching.Types.Internal
open Archer.Logger
open Archer.Logger.TestResultReportReporters

let private feature =
    reporterTestBuilder
    |> Sub.Feature (
        TestTags [
            Category "Summary Reporters"
            Category "Approvals"
        ],
        Setup (fun reporter ->
            let fb = TestExecutionResultFailureBuilder ()
            Ok (reporter, fb)
        )
    )
    
let ``Format a test failure report`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let testFeature = Arrow.NewFeature ("Test", "Feature")
        let indentReporter = IndentReporter 0
        
        {
            Result = failureBuilder.GeneralExecutionFailure.CancelFailure ()
            Time = {
                Setup = TimeSpan (0, 0, 0, 0, 500) 
                Test =  TimeSpan (0, 0, 0, 0, 200)
                Teardown = TimeSpan (0, 0, 0, 10)
                Total = TimeSpan (0, 0, 0, 0, 711)
            }
            Test = testFeature.Test (fun _ -> TestSuccess)
        }
        |> getTestFailureReportReport indentReporter
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Format a test success report`` =
    feature.Test (fun (reporter, _) environment ->
        let testInfo = environment.TestInfo
        let testFeature = Arrow.NewFeature ("Test", "Feature")
        let indentReporter = IndentReporter 0
        
        {
            Time = {
                Setup = TimeSpan (0, 0, 0, 0, 15) 
                Test =  TimeSpan (0, 0, 0, 0, 300)
                Teardown = TimeSpan (0, 0, 0, 150)
                Total = TimeSpan (0, 0, 0, 0, 465)
            }
            Test = testFeature.Test (fun _ -> TestSuccess)
        }
        |> getTestSuccessReportReport indentReporter
        |> Should.MeetStandard reporter testInfo
    )


let ``Test Cases`` = feature.GetTests ()