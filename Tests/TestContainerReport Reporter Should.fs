module Archer.Logger.Tests.``TestContainerReport Reporter Should``

open System
open Archer
open Archer.Arrows
open Archer.ApprovalsSupport
open Archer.Arrows.Internal.Types
open Archer.CoreTypes.InternalTypes
open Archer.Fletching.Types.Internal
open Archer.Logger
open Archer.Logger.TestContainerReportReporter

let private getTime add =
    let add = add * 50
    let sTime = 10 + add
    let tTime = 100 + add
    let tdTime = 12 + add
    let total = sTime + tTime + tdTime
    {
        Setup = TimeSpan (0, 0, 0, sTime)
        Test = TimeSpan (0, 0, 0, 0, tTime)
        Teardown = TimeSpan (0, 0, 0, 0, tdTime)
        Total = TimeSpan (0, 0, 0, 0, 0, total)
    }
    
let getTest (testFeature: IFeature<unit>) index =
    testFeature.Test ($"Test %d{index}", fun _ -> TestSuccess)
    
let private getReport () =
    let fb = TestExecutionResultFailureBuilder ()
    let containerPath = "Name Space Containing"
    let containerName = "A test module name"
    let testFeature = Arrow.NewFeature (containerPath, containerName)
    
    let failures =
        [0..2]
        |> List.map (fun index ->
            {
                Result = fb.TestExecutionResult.ValidationFailure {ExpectedValue = index * 100; ActualValue = (index * 100) - 10 }
                Time = getTime index
                Test =  getTest testFeature index
            }
        )
        
    let successes =
        [3..5]
        |> List.map (fun index ->
            {
                Time = getTime index
                Test = getTest testFeature index
            }
        )
    
    {
        ContainerFullName = $"%s{containerPath}.%s{containerName}"
        ContainerName = containerName
        Failures = failures
        Successes = successes
    }
    
let private feature =
    reporterTestBuilder
    |> Sub.Feature (
        TestTags [
            Category "Reporters"
            Category "Approvals"
        ],
        Setup (fun reporter ->
            let indentReporter = IndentReporter 0
            Ok (reporter, getReport (), indentReporter)
        )
    )
    
let ``Format test container failures`` =
    feature.Test (fun (reporter, report, indenter) environment ->
        let testInfo = environment.TestInfo
        
        report
        |> defaultTestContainerReportFailureReporter indenter
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Test Cases`` = feature.GetTests ()