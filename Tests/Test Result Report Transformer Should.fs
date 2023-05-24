module Archer.Logger.Tests.``Test Result Report Transformer Should``

open System
open Archer
open Archer.Arrows
open Archer.ApprovalsSupport
open Archer.CoreTypes.InternalTypes
open Archer.Fletching.Types.Internal
open Archer.Logger
open Archer.Logger.TestResultReportTransformers

let private feature =
    loggerTestBuilder
    |> Sub.Feature (
        TestTags [
            Category "Transformers"
            Category "Approvals"
        ],
        Setup (fun reporter ->
            let fb = TestExecutionResultFailureBuilder ()
            Ok (reporter, fb)
        )
    )
    
let ``Transform a test failure`` =
    feature.Test (
        TestTags [
            Category "TestFailureTransformer "
        ],
        fun (reporter, failureBuilder) environment ->
            let testInfo = environment.TestInfo
            let testFeature = Arrow.NewFeature ("Test", "Feature")
            let indenter = IndentTransformer 0
            
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
            |> getDefaultTestFailureReportTransformer indenter
            |> Should.MeetStandard reporter testInfo
    )
    
let ``Transform a test success`` =
    feature.Test (
        TestTags [
            Category "TestSuccessTransformer "
        ],
        fun (reporter, _) environment ->
            let testInfo = environment.TestInfo
            let testFeature = Arrow.NewFeature ("Test", "Feature")
            let indentReporter = IndentTransformer 0
            
            {
                Time = {
                    Setup = TimeSpan (0, 0, 0, 0, 15) 
                    Test =  TimeSpan (0, 0, 0, 0, 300)
                    Teardown = TimeSpan (0, 0, 0, 150)
                    Total = TimeSpan (0, 0, 0, 0, 465)
                }
                Test = testFeature.Test (fun _ -> TestSuccess)
            }
            |> getDefaultTestSuccessReportTransformer indentReporter
            |> Should.MeetStandard reporter testInfo
    )

let ``Test Cases`` = feature.GetTests ()