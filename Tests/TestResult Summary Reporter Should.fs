module Archer.Logger.Tests.``TestResult Summary Reporter Should``

open System.IO
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
        ]
    )
    
let ``format success into single line`` =
    feature.Test (fun reporter environment ->
        defaultTestResultSummaryReporter TestSuccess environment.TestInfo
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``format success from a different test`` =
    feature.Test (fun reporter environment ->
        defaultTestResultSummaryReporter TestSuccess environment.TestInfo
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
            defaultTestResultSummaryReporter failure environment.TestInfo
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
            defaultTestResultSummaryReporter failure environment.TestInfo
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
            defaultTestResultSummaryReporter failure environment.TestInfo
            |> Should.MeetStandard reporter environment.TestInfo
        )
    )
    
let ``Test Cases`` = feature.GetTests ()