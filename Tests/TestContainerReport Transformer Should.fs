module Archer.Logger.Tests.``TestContainerReport Transformer Should``

open System
open Archer
open Archer.Arrows
open Archer.ApprovalsSupport
open Archer.Arrows.Internal.Types
open Archer.CoreTypes.InternalTypes
open Archer.Fletching.Types.Internal
open Archer.Logger
open Archer.Logger.TestContainerReportTransformer

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
    
let private getReport pathN nameN =
    let fb = TestExecutionResultFailureBuilder ()
    let containerPath = $"%d{pathN} Name Space Containing"
    let containerName = $"A test module%d{nameN} name"
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
    loggerTestBuilder
    |> Sub.Feature (
        TestTags [
            Category "Transformers"
            Category "Approvals"
        ],
        Setup (fun reporter ->
            let indentReporter = IndentTransformer 0
            
            let reports =
                [
                    for path in 0..2 do
                        for name in 3..5 do
                            yield path, name
                ]
                |> List.map (fun (a, b) -> getReport a b)
            
            Ok (reporter, reports, indentReporter)
        )
    )
    
let ``Transform test container failures`` =
    feature.Test (fun (reporter, reports, indenter) environment ->
        let testInfo = environment.TestInfo
        let report = reports |> List.head
        
        report
        |> defaultTestContainerReportFailureTransformer indenter
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Transform test container successes`` =
    feature.Test (fun (reporter, reports, indenter) environment ->
        let testInfo = environment.TestInfo
        let report = reports |> List.head
        
        report
        |> defaultTestContainerReportSuccessTransformer indenter
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Transform multiple test containers failures`` =
    feature.Test (fun (reporter, reports, indenter) environment ->
        let testInfo = environment.TestInfo
        
        reports
        |> defaultTestContainerReportFailuresTransformer indenter
        |> Should.MeetStandard reporter testInfo
    )
    
    
let ``Transform multiple test containers successes`` =
    feature.Test (fun (reporter, reports, indenter) environment ->
        let testInfo = environment.TestInfo
        
        reports
        |> defaultTestContainerReportSuccessesTransformer indenter
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Test Cases`` = feature.GetTests ()