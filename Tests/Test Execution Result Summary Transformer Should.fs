module Archer.Reporting.Tests.``Test Execution Result Summary Transformer Should``

open Archer
open Archer.Core
open Archer.ApprovalsSupport
open Archer.Validations.Types.Internal
open Archer.Reporting.Summaries

let private feature =
    ReportingTestBuilder
    |> Sub.Feature (
        TestTags [
            Category "Transformers"
            Category "Summary Transformers"
            Category "Approvals"
            Category "TestExecutionResult"
        ],
        Setup (fun reporter ->
            let fb = TestExecutionResultFailureBuilder ()
            Ok (reporter, fb)
        )
    )
    
let ``Transform a test success result into a single line`` =
    feature.Test (fun (reporter, _) environment ->
        defaultTestExecutionResultSummaryTransformer (TestExecutionResult TestSuccess) environment.TestInfo
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``Transform a test success result for a different test`` =
    feature.Test (fun (reporter, _) environment ->
        defaultTestExecutionResultSummaryTransformer (TestExecutionResult TestSuccess) environment.TestInfo
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``Transform a general test execution failure result into a single line`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let failure = failureBuilder.GeneralExecutionFailure.GeneralFailure "Something happened"
        defaultTestExecutionResultSummaryTransformer failure environment.TestInfo
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``Transform a test execution cancel failure result into a single line`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let failure = failureBuilder.GeneralExecutionFailure.CancelFailure ()
        defaultTestExecutionResultSummaryTransformer failure environment.TestInfo
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``Transform a general test failure result into a single line`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let failure = failureBuilder.TestExecutionResult.GeneralTestExpectationFailure "Bad test"
        defaultTestExecutionResultSummaryTransformer failure environment.TestInfo
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``Transform a setup failure result into a single line`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let failure = failureBuilder.SetupExecutionFailure.ExceptionFailure (System.Exception "Something boomed")
        defaultTestExecutionResultSummaryTransformer failure environment.TestInfo
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``Transform a setup canceled failure result into a single line`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let failure = failureBuilder.SetupExecutionFailure.CancelFailure ()
        defaultTestExecutionResultSummaryTransformer failure environment.TestInfo
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``Transform a general teardown failure result into a single line`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let failure = failureBuilder.TeardownExecutionFailure.GeneralFailure "Something not right"
        defaultTestExecutionResultSummaryTransformer failure environment.TestInfo
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``Transform a teardown cancel failure result into a single line`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let failure = failureBuilder.TeardownExecutionFailure.CancelFailure ()
        defaultTestExecutionResultSummaryTransformer failure environment.TestInfo
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``Test Cases`` = feature.GetTests ()