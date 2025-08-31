module Archer.Reporting.Tests.``TestFailContainer Transformer Should``

open System
open System.Reflection
open Archer
open Archer.Core
open Archer.ApprovalsSupport
open Archer.Core.Internal.Types
open Archer.Types.InternalTypes.RunnerTypes
open Archer.Validations.Types.Internal
open Archer.Reporting
open Archer.Reporting.Detail
open Archer.Reporting.LocationHelpers
open Archer.Reporting.StringHelpers
open Archer.Reporting.TestFailContainerTransformer

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
            let indenter = IndentTransformer ()
            let fbSetup = SetupTeardownResultFailureBuilder SetupFailureType
            let fbTest = TestFailureBuilder TestRunFailureType
            let fbTear = SetupTeardownResultFailureBuilder TeardownFailureType
            let fbGen = GeneralFailureBuilder GeneralFailureType
                
            let buildTestFailures n path name =
                [
                    (fbSetup.CancelFailure ())
                    (fbSetup.ExceptionFailure (Exception "Setup went boom"))
                    (fbSetup.GeneralFailure "Setup generally did not work")
                    
                    (fbTest.ExceptionFailure (ArgumentException "This test won the argument"))
                    (fbTest.IgnoreFailure ())
                    (fbTest.ValidationFailure ("good things", 666))
                    (fbTest.GeneralTestExpectationFailure "Generally tests should work\nbut this one did not")
                    
                    (fbTear.CancelFailure ())
                    (fbTear.ExceptionFailure (OutOfMemoryException "I cannot remember what I was doing"))
                    (fbTear.GeneralFailure "Nope!")
                    
                    (fbGen.CancelFailure ())
                    (fbGen.ExceptionFailure (IndexOutOfRangeException "Don't point that thing at me"))
                    (fbGen.GeneralFailure "In general nothing will work\Not by the hair on my\nchinny chin chin")
                    
                ]
                |> List.mapi (fun i value -> value, FakeTestBuilder.BuildTest (n + i) path name)
                
            let failContainers =
                [
                    FailContainer ("My First", [
                        EmptyFailures
                        FailedTests (buildTestFailures 0 "" "My First")
                        FailContainer ("Failing containers", [
                            FailedTests (buildTestFailures 100 "My First" "Failing containers")
                        ])
                    ])
                    FailContainer ("Your First Container", [FailedTests (buildTestFailures 33 "" "Your First Container")])
                ]
                
            Ok (reporter, indenter, failContainers)
        )
    )
    
let ``Transform an empty Failure`` =
    feature.Test (fun (_, indenter, _) ->
        EmptyFailures
        |> defaultTestFailContainerTransformer indenter
        |> Should.BeEqualTo ""
    )
    
let ``Transform a failure container`` =
    feature.Test (fun (reporter, indenter, failContainers) environment ->
        failContainers
        |> List.head
        |> defaultTestFailContainerTransformer indenter
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``Transform all failure containers`` =
    feature.Test (fun (reporter, indenter, failContainers) environment ->
        failContainers
        |> defaultTestFailContainerAllTransformer indenter
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
    
let ``Test Cases`` = feature.GetTests ()