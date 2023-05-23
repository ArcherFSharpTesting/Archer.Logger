module Archer.Logger.Tests.``TestExecutionResult Detail Reporter Should``

open System
open Archer
open Archer.Arrows
open Archer.ApprovalsSupport
open Archer.Fletching.Types.Internal
open Archer.Logger

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
    
let ``Provide a detailed result for setup exception failure`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentReporter 0
        
        try
            Exception "Boom Boom Bang" |> raise
        with ex ->
            failureBuilder.SetupExecutionFailure.ExceptionFailure ex
            |> detailedTestExecutionResultReporter indent testInfo
            |> Should.MeetStandard reporter testInfo
    )

let ``Test Cases`` = feature.GetTests ()