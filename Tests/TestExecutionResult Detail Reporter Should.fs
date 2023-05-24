module Archer.Logger.Tests.``TestExecutionResult Detail Reporter Should``

open System
open System.Reflection
open Archer
open Archer.Arrows
open Archer.ApprovalsSupport
open Archer.Fletching.Types.Internal
open Archer.Logger
open Archer.Logger.LocationHelpers
open Archer.Logger.StringHelpers

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
    
let ``Provide a detailed setup exception failure report`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentReporter 0
        
        try
            Exception "Boom Boom Bang" |> raise
        with ex ->
            failureBuilder.SetupExecutionFailure.ExceptionFailure ex
            |> detailedTestExecutionResultReporter indent testInfo
            |> replace (getSolutionRoot (Assembly.GetAssembly (typeof<IndentReporter>))) "."
            |> Should.MeetStandard reporter testInfo
    )
    
let ``Provide a detailed setup canceled failure report`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentReporter 0
        
        failureBuilder.SetupExecutionFailure.CancelFailure ()
        |> detailedTestExecutionResultReporter indent testInfo
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Provide a detailed general setup failure report`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentReporter 0
        
        failureBuilder.SetupExecutionFailure.GeneralFailure "This setup is wrong\nJust Wrong\nBail out"
        |> detailedTestExecutionResultReporter indent testInfo
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Provide a detailed test exception failure report`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentReporter 0
        
        try
            ArgumentException "A bad argument" |> raise
        with ex ->
            failureBuilder.TestExecutionResult.ExceptionFailure ex
            |> detailedTestExecutionResultReporter indent testInfo
            |> replace (getSolutionRoot (Assembly.GetAssembly typeof<IndentReporter>)) "."
            |> Should.MeetStandard reporter testInfo
    )
    
let ``Provide a detailed test ignored failure report`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentReporter 0
        
        failureBuilder.TestExecutionResult.IgnoreFailure (Some "This is an ignored test")
        |> detailedTestExecutionResultReporter indent testInfo
        |> replace (getSolutionRoot (Assembly.GetAssembly typeof<IndentReporter>)) "."
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Provide a detailed test ignored failure with no message report`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentReporter 0
        
        failureBuilder.TestExecutionResult.IgnoreFailure ()
        |> detailedTestExecutionResultReporter indent testInfo
        |> replace (getSolutionRoot (Assembly.GetAssembly typeof<IndentReporter>)) "."
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Test Cases`` = feature.GetTests ()