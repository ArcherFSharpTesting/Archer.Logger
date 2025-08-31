module Archer.Reporting.Tests.``TestSuccessContainer Transformer Should``

open Archer
open Archer.Core
open Archer.ApprovalsSupport
open Archer.Types.InternalTypes.RunnerTypes
open Archer.Reporting
open Archer.Reporting.TestSuccessContainerTransformer

let private buildSuccesses n path name =
    FakeTestBuilder.BuildNTests 5 (StartNamingAt (n + 1)) path name
    
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
            let successes = [
                SuccessContainer (
                    "My test Container",
                    [
                        EmptySuccesses
                        SucceededTests (buildSuccesses 30 "" "My test Container")
                        SuccessContainer (
                            "is awesome",
                            [
                                SucceededTests (buildSuccesses 60 "My test Container" "is awesome")
                            ]
                        )
                        EmptySuccesses
                    ]
                )
                SuccessContainer (
                    "My second test Container",
                    [
                        SucceededTests (buildSuccesses 30 "" "My second test Container")
                        SuccessContainer (
                            "is done",
                            [
                                SucceededTests (buildSuccesses 60 "My second test Container" "is done")
                            ]
                        )
                        EmptySuccesses
                    ]
                )
                EmptySuccesses
            ]
            
            Ok (reporter, IndentTransformer (), successes)
        )
    )
    
let ``Transform an empty success container`` =
    feature.Test (fun (_, indenter, _) ->
        EmptySuccesses
        |> defaultSingleTestSuccessContainerTransformer indenter
        |> Should.BeEqualTo ""
    )
    
let ``Transform an Succeeding Tests success container`` =
    feature.Test (fun (reporter, indenter, _) environment ->
        SucceededTests (buildSuccesses 99 "" "")
        |> defaultSingleTestSuccessContainerTransformer indenter
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``Transform SucceededTests container`` =
    feature.Test (fun (reporter, indenter, containers) environment ->
        containers
        |> List.head
        |> defaultSingleTestSuccessContainerTransformer indenter
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``Transform all SucceededTests container`` =
    feature.Test (fun (reporter, indenter, containers) environment ->
        containers
        |> defaultAllTestSuccessContainerTransformer indenter
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``Test Cases`` = feature.GetTests ()