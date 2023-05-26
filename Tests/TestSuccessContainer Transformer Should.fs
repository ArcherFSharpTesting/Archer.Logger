module Archer.Logger.Tests.``TestSuccessContainer Transformer Should``

open System
open System.Reflection
open Archer
open Archer.Arrows
open Archer.ApprovalsSupport
open Archer.CoreTypes.InternalTypes.RunnerTypes
open Archer.Fletching.Types.Internal
open Archer.Logger
open Archer.Logger.Detail
open Archer.Logger.LocationHelpers
open Archer.Logger.StringHelpers
open Archer.Logger.TestSuccessContainerTransformer

let private buildSuccesses n path name =
    [1..5] |> List.map (fun i -> FakeTestBuilder.BuildTest (i + n) path name
                        )
let private feature =
    loggerTestBuilder
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
        |> defaultTestSuccessContainerTransformer indenter
        |> Should.BeEqualTo ""
    )
    
let ``Transform an Succeeding Tests success container`` =
    feature.Test (fun (reporter, indenter, _) environment ->
        SucceededTests (buildSuccesses 99 "" "")
        |> defaultTestSuccessContainerTransformer indenter
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``Transform SucceededTests container`` =
    feature.Test (fun (reporter, indenter, containers) environment ->
        containers
        |> List.head
        |> defaultTestSuccessContainerTransformer indenter
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``Transform all SucceededTests container`` =
    feature.Test (fun (reporter, indenter, containers) environment ->
        containers
        |> defaultAllTestSuccessContainerTransformer indenter
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``Test Cases`` = feature.GetTests ()