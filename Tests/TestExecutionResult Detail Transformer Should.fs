module Archer.Reporting.Tests.``TestExecutionResult Detail Transformer Should``

open System
open System.Reflection
open Archer
open Archer.Core
open Archer.ApprovalsSupport
open Archer.Validations.Types.Internal
open Archer.Reporting
open Archer.Reporting.Detail
open Archer.Reporting.LocationHelpers
open Archer.Reporting.StringHelpers

let private feature =
    ReportingTestBuilder
    |> Sub.Feature (
        TestTags [
            Category "Detail Transformers"
            Category "Transformers"
            Category "Approvals"
            Category "TestExecutionResult"
        ],
        Setup (fun reporter ->
            let fb = TestExecutionResultFailureBuilder ()
            Ok (reporter, fb)
        )
    )
    
let ``Transform a detailed setup exception failure`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentTransformer 0
        
        try
            Exception "Boom Boom Bang" |> raise
        with ex ->
            failureBuilder.SetupExecutionFailure.ExceptionFailure ex
            |> defaultDetailedTestExecutionResultTransformer indent testInfo None
            |> replace (getSolutionRoot (Assembly.GetAssembly typeof<IndentTransformer>)) "."
            |> Should.MeetStandard reporter testInfo
    )
    
let ``Transform a detailed setup canceled failure`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentTransformer 0
        
        failureBuilder.SetupExecutionFailure.CancelFailure ()
        |> defaultDetailedTestExecutionResultTransformer indent testInfo None
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Transform a detailed general setup failure`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentTransformer 0
        
        failureBuilder.SetupExecutionFailure.GeneralFailure "This setup is wrong\nJust Wrong\nBail out"
        |> defaultDetailedTestExecutionResultTransformer indent testInfo None
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Transform a detailed test exception failure`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentTransformer 0
        
        try
            ArgumentException "A bad argument" |> raise
        with ex ->
            failureBuilder.TestExecutionResult.ExceptionFailure ex
            |> defaultDetailedTestExecutionResultTransformer indent testInfo None
            |> replace (getSolutionRoot (Assembly.GetAssembly typeof<IndentTransformer>)) "."
            |> Should.MeetStandard reporter testInfo
    )
    
let ``Transform a detailed test ignored failure`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentTransformer 0
        
        failureBuilder.TestExecutionResult.IgnoreFailure (Some "This is an ignored test")
        |> defaultDetailedTestExecutionResultTransformer indent testInfo None
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Transform a detailed test ignored failure with no message`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentTransformer 0
        
        failureBuilder.TestExecutionResult.IgnoreFailure ()
        |> defaultDetailedTestExecutionResultTransformer indent testInfo None
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Transform a detailed test expectation other failure with no message`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentTransformer 0
        
        failureBuilder.TestExecutionResult.GeneralTestExpectationFailure "Hello World\nToday is great"
        |> defaultDetailedTestExecutionResultTransformer indent testInfo None
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Transform a detailed test validation failure with no message`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentTransformer 0
        
        failureBuilder.TestExecutionResult.ValidationFailure { ExpectedValue = "The World Is Great"; ActualValue = "The world is greater" }
        |> defaultDetailedTestExecutionResultTransformer indent testInfo None
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Transform a detailed test validation failure with failure comment with no message`` =
    feature.Test (
        Setup (fun (reporter, _) ->
            let fb = TestResultFailureBuilder id
            Ok (reporter, fb)
        ),
        TestBody (fun (reporter, failureBuilder: TestResultFailureBuilder<TestResult>) environment ->
            let testInfo = environment.TestInfo
            let indent = IndentTransformer 0
            
            failureBuilder.ValidationFailure { ExpectedValue = "The World Is Great"; ActualValue = "The world is greater" }
            |> withFailureComment "Things are not great"
            |> TestExecutionResult
            |> defaultDetailedTestExecutionResultTransformer indent testInfo None
            |> Should.MeetStandard reporter testInfo
        )
    )
    
let ``Transform a detailed test combination failure`` =
    feature.Test (fun (reporter, _) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentTransformer 0
        
        let failureA = TestExceptionFailure (Exception "Boo")
        let failureB = TestIgnored (Some "No watch me", testInfo.Location)
        
        CombinationFailure ((failureA, None), (failureB, Some { FilePath = "D:\\og"; FileName = "Bark.ruf"; LineNumber = 99 }))
        |> TestFailure
        |> TestExecutionResult
        |> defaultDetailedTestExecutionResultTransformer indent testInfo None
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Transform a detailed test success`` =
    feature.Test (fun (reporter, _) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentTransformer 0
        
        TestSuccess
        |> TestExecutionResult
        |> defaultDetailedTestExecutionResultTransformer indent testInfo None
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Transform a detailed test teardown cancel`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentTransformer 0
        
        failureBuilder.TeardownExecutionFailure.CancelFailure ()
        |> defaultDetailedTestExecutionResultTransformer indent testInfo None
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Transformer a detailed general cancel failure`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentTransformer 0
        
        failureBuilder.GeneralExecutionFailure.CancelFailure ()
        |> defaultDetailedTestExecutionResultTransformer indent testInfo None
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Transformer a detailed general general failure`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentTransformer 0
        
        failureBuilder.GeneralExecutionFailure.GeneralFailure "A general general failure"
        |> defaultDetailedTestExecutionResultTransformer indent testInfo None
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Transform a detailed general exception failure`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentTransformer 0
        
        try
            ArgumentOutOfRangeException "gone to far this time"
            |> raise
        with ex ->
            failureBuilder.GeneralExecutionFailure.ExceptionFailure ex
            |> defaultDetailedTestExecutionResultTransformer indent testInfo None
            |> replace (getSolutionRoot (Assembly.GetAssembly typeof<IndentTransformer>)) "."
            |> Should.MeetStandard reporter testInfo
    )
    
let ``Transform a detailed test validation failure of integers with no message`` =
    feature.Test (fun (reporter, failureBuilder) environment ->
        let testInfo = environment.TestInfo
        let indent = IndentTransformer 0
        
        failureBuilder.TestExecutionResult.ValidationFailure { ExpectedValue = 100; ActualValue = 200 }
        |> defaultDetailedTestExecutionResultTransformer indent testInfo None
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Test Cases`` = feature.GetTests ()