module Archer.Logger.Tests.``TestResult Summary Transformer Should``

open Archer
open Archer.Arrows
open Archer.ApprovalsSupport
open Archer.Fletching.Types.Internal
open Archer.Logger.Summaries

let private feature =
    loggerTestBuilder
    |> Sub.Feature (
        TestTags [
            Category "Transformers"
            Category "Approvals"
            Category "TestResult"
        ]
    )
    
let ``format success into single line`` =
    feature.Test (fun reporter environment ->
        defaultTestResultSummaryTransformer TestSuccess environment.TestInfo
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``format success from a different test`` =
    feature.Test (fun reporter environment ->
        defaultTestResultSummaryTransformer TestSuccess environment.TestInfo
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``format failure into single line`` =
    feature.Test (
        Setup (fun reporter ->
            let fb = TestResultFailureBuilder id
            Ok (reporter, fb)
        ),
        TestBody (fun (reporter, failureBuilder: TestResultFailureBuilder<TestResult>) environment ->
            let failure = failureBuilder.GeneralTestExpectationFailure "BadNews"
            defaultTestResultSummaryTransformer failure environment.TestInfo
            |> Should.MeetStandard reporter environment.TestInfo
        )
    )
    
let ``format different failure into single line`` =
    feature.Test (
        Setup (fun reporter ->
            let fb = TestResultFailureBuilder id
            Ok (reporter, fb)
        ),
        TestBody (fun (reporter, failureBuilder: TestResultFailureBuilder<TestResult>) environment ->
            let failure = failureBuilder.ValidationFailure {ActualValue = 33; ExpectedValue = "Hello" }
            defaultTestResultSummaryTransformer failure environment.TestInfo
            |> Should.MeetStandard reporter environment.TestInfo
        )
    )
    
let ``format Ignore into single line`` =
    feature.Test (
        Setup (fun reporter ->
            let fb = TestResultFailureBuilder id
            Ok (reporter, fb)
        ),
        TestBody (fun (reporter, failureBuilder: TestResultFailureBuilder<TestResult>) environment ->
            let failure = failureBuilder.IgnoreFailure (Some "Don't do this")
            defaultTestResultSummaryTransformer failure environment.TestInfo
            |> Should.MeetStandard reporter environment.TestInfo
        )
    )
    
let ``Test Cases`` = feature.GetTests ()