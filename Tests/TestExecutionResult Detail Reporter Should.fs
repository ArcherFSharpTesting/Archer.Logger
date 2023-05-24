module Archer.Logger.Tests.``TestExecutionResult Detail Reporter Should``

open System
open System.Reflection
open Archer
open Archer.Arrows
open Archer.ApprovalsSupport
open Archer.Fletching.Types.Internal
open Archer.Logger
open Archer.Logger.Detail
open Archer.Logger.LocationHelpers
open Archer.Logger.StringHelpers

let private feature =
    reporterTestBuilder
    |> Sub.Feature (
        TestTags [
            Category "Detail Reporters"
            Category "Reporters"
            Category "Approvals"
            Category "TestExecutionResult"
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
            |> detailedTestExecutionResultReporter indent testInfo None
            |> replace (getSolutionRoot (Assembly.GetAssembly typeof<IndentReporter>)) "."
            |> Should.MeetStandard reporter testInfo
    )
    
let ``Provide a detailed setup canceled failure report`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentReporter 0
        
        failureBuilder.SetupExecutionFailure.CancelFailure ()
        |> detailedTestExecutionResultReporter indent testInfo None
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Provide a detailed general setup failure report`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentReporter 0
        
        failureBuilder.SetupExecutionFailure.GeneralFailure "This setup is wrong\nJust Wrong\nBail out"
        |> detailedTestExecutionResultReporter indent testInfo None
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
            |> detailedTestExecutionResultReporter indent testInfo None
            |> replace (getSolutionRoot (Assembly.GetAssembly typeof<IndentReporter>)) "."
            |> Should.MeetStandard reporter testInfo
    )
    
let ``Provide a detailed test ignored failure report`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentReporter 0
        
        failureBuilder.TestExecutionResult.IgnoreFailure (Some "This is an ignored test")
        |> detailedTestExecutionResultReporter indent testInfo None
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Provide a detailed test ignored failure with no message report`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentReporter 0
        
        failureBuilder.TestExecutionResult.IgnoreFailure ()
        |> detailedTestExecutionResultReporter indent testInfo None
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Provide a detailed test expectation other failure with no message report`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentReporter 0
        
        failureBuilder.TestExecutionResult.GeneralTestExpectationFailure "Hello World\nToday is great"
        |> detailedTestExecutionResultReporter indent testInfo None
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Provide a detailed test validation failure with no message report`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentReporter 0
        
        failureBuilder.TestExecutionResult.ValidationFailure { ExpectedValue = "The World Is Great"; ActualValue = "The world is greater" }
        |> detailedTestExecutionResultReporter indent testInfo None
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Provide a detailed test validation failure with failure comment with no message report`` =
    feature.Test (
        Setup (fun (reporter, _) ->
            let fb = TestResultFailureBuilder id
            Ok (reporter, fb)
        ),
        TestBody (fun (reporter, failureBuilder: TestResultFailureBuilder<TestResult>) environment ->
            let testInfo = environment.TestInfo
            let indent = IndentReporter 0
            
            failureBuilder.ValidationFailure { ExpectedValue = "The World Is Great"; ActualValue = "The world is greater" }
            |> withFailureComment "Things are not great"
            |> TestExecutionResult
            |> detailedTestExecutionResultReporter indent testInfo None
            |> Should.MeetStandard reporter testInfo
        )
    )
    
let ``Provide a detailed test combination failure report`` =
    feature.Test (fun (reporter, _) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentReporter 0
        
        let failureA = TestExceptionFailure (Exception "Boo")
        let failureB = TestIgnored (Some "No watch me", testInfo.Location)
        
        CombinationFailure ((failureA, None), (failureB, Some { FilePath = "D:\\og"; FileName = "Bark.ruf"; LineNumber = 99 }))
        |> TestFailure
        |> TestExecutionResult
        |> detailedTestExecutionResultReporter indent testInfo None
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Provide a detailed test success report`` =
    feature.Test (fun (reporter, _) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentReporter 0
        
        TestSuccess
        |> TestExecutionResult
        |> detailedTestExecutionResultReporter indent testInfo None
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Provide a detailed test teardown cancel report`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentReporter 0
        
        failureBuilder.TeardownExecutionFailure.CancelFailure ()
        |> detailedTestExecutionResultReporter indent testInfo None
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Provide a detailed general cancel failure report`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentReporter 0
        
        failureBuilder.GeneralExecutionFailure.CancelFailure ()
        |> detailedTestExecutionResultReporter indent testInfo None
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Provide a detailed general general failure report`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentReporter 0
        
        failureBuilder.GeneralExecutionFailure.GeneralFailure "A general general failure"
        |> detailedTestExecutionResultReporter indent testInfo None
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Provide a detailed general exception failure report`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentReporter 0
        
        try
            ArgumentOutOfRangeException "gone to far this time"
            |> raise
        with ex ->
            failureBuilder.GeneralExecutionFailure.ExceptionFailure ex
            |> detailedTestExecutionResultReporter indent testInfo None
            |> replace (getSolutionRoot (Assembly.GetAssembly typeof<IndentReporter>)) "."
            |> Should.MeetStandard reporter testInfo
    )
    
let ``Test Cases`` = feature.GetTests ()