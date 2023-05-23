module Archer.Logger.Tests.``Test Execution Result Summary Reporter Should``

open Archer
open Archer.Arrows
open Archer.ApprovalsSupport
open Archer.Fletching.Types.Internal
open Archer.Logger

let private feature =
    reporterTestBuilder
    |> Sub.Feature (
        TestTags [
            Category "Test Reporter"
            Category "Approvals"
        ],
        Setup (fun reporter ->
            let fb = TestExecutionResultFailureBuilder ()
            Ok (reporter, fb)
        )
    )
    
let ``Format a test success result into a single line`` =
    feature.Test (fun (reporter, _) environment ->
        testExecutionResultSummaryReporter (TestExecutionResult TestSuccess) environment.TestInfo
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``Format a test success result for a different test`` =
    feature.Test (fun (reporter, _) environment ->
        testExecutionResultSummaryReporter (TestExecutionResult TestSuccess) environment.TestInfo
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``Format a general test execution failure result into a single line`` =
    feature.Test (fun (reporter, failureBuilder: TestExecutionResultFailureBuilder) environment ->
        let failure = failureBuilder.GeneralExecutionFailure.GeneralFailure "Something happened"
        testExecutionResultSummaryReporter failure environment.TestInfo
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``Format a test execution cancel failure result into a single line`` =
    feature.Test (fun (reporter, failureBuilder: TestExecutionResultFailureBuilder) environment ->
        let failure = failureBuilder.GeneralExecutionFailure.CancelFailure ()
        testExecutionResultSummaryReporter failure environment.TestInfo
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``Format a general test failure result into a single line`` =
    feature.Test (fun (reporter, failureBuilder: TestExecutionResultFailureBuilder) environment ->
        let failure = failureBuilder.TestExecutionResult.GeneralTestExpectationFailure "Bad test"
        testExecutionResultSummaryReporter failure environment.TestInfo
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``Format a setup failure result into a single line`` =
    feature.Test (fun (reporter, failureBuilder: TestExecutionResultFailureBuilder) environment ->
        let failure = failureBuilder.SetupExecutionFailure.ExceptionFailure (System.Exception "Something boomed")
        testExecutionResultSummaryReporter failure environment.TestInfo
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``Format a setup canceled failure result into a single line`` =
    feature.Test (fun (reporter, failureBuilder: TestExecutionResultFailureBuilder) environment ->
        let failure = failureBuilder.SetupExecutionFailure.CancelFailure ()
        testExecutionResultSummaryReporter failure environment.TestInfo
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``Format a general teardown failure result into a single line`` =
    feature.Test (fun (reporter, failureBuilder: TestExecutionResultFailureBuilder) environment ->
        let failure = failureBuilder.TeardownExecutionFailure.GeneralFailure "Something not right"
        testExecutionResultSummaryReporter failure environment.TestInfo
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``Format a teardown cancel failure result into a single line`` =
    feature.Test (fun (reporter, failureBuilder: TestExecutionResultFailureBuilder) environment ->
        let failure = failureBuilder.TeardownExecutionFailure.CancelFailure ()
        testExecutionResultSummaryReporter failure environment.TestInfo
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``Test Cases`` = feature.GetTests ()