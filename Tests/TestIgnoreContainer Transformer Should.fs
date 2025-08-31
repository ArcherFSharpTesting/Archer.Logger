module Archer.Reporting.Tests.``TestIgnoreContainer Transformer Should``

open Archer
open Archer.Core
open Archer.ApprovalsSupport
open Archer.Types.InternalTypes.RunnerTypes
open Archer.Reporting
open Archer.Reporting.TestIgnoreContainerTransformer

let private buildIgnored n path name =
    [
        None
        Some "This test is not complete"
        Some "Trouble shooting related test"
        None
    ]
    |> List.map (fun v ->
        FakeTestBuilder.BuildNTests 2 (StartNamingAt n) path name
        |> List.mapi (fun i t ->
            let location =
                if i % 2 = 0 then { t.Location with LineNumber = t.Location.LineNumber + 1 }
                else t.Location
                
            v, location, t
        )
    )
    |> List.concat
        
    
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
            let ignored = [
                EmptyIgnore
                IgnoreContainer (
                    "My ignored container",
                    [
                        IgnoredTests (buildIgnored 0 "" "My ignored container")
                        EmptyIgnore
                        IgnoreContainer (
                            "contains things",
                            [
                                IgnoredTests (buildIgnored 10 "My ignored container" "contains things")
                            ]
                        )
                        IgnoreContainer (
                            "contains no things",
                            []
                        )
                    ]
                )
                IgnoreContainer (
                    "My second ignored container",
                    [
                        IgnoreContainer (
                            "is also ignored",
                            [
                                IgnoredTests (buildIgnored 20 "My second ignored container" "is also ignored")
                            ]
                        )
                    ]
                )
            ]
            
            Ok (reporter, IndentTransformer (), ignored)
        )
    )
    
let ``Transform an empty ignored container`` =
    feature.Test (fun (_, indenter, _) ->
        EmptyIgnore
        |> defaultTestIgnoreContainerTransformer indenter
        |> Should.BeEqualTo ""
    )
    
let ``Transform IgnoredTests container`` =
    feature.Test (fun (reporter, indenter, _) environment ->
        IgnoredTests (buildIgnored 45 "My ignored container" "contains things")
        |> defaultTestIgnoreContainerTransformer indenter
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``Transform simple IgnoreContainer container`` =
    feature.Test (fun (reporter, indenter, _) environment ->
        IgnoreContainer ("something ignored", [IgnoredTests (buildIgnored 73 "My ignored container" "contains things")])
        |> defaultTestIgnoreContainerTransformer indenter
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``Transform all IgnoreContainers`` =
    feature.Test (fun (reporter, indenter, containers) environment ->
        containers
        |> defaultAllTestIgnoreContainerTransformer indenter
        |> Should.MeetStandard reporter environment.TestInfo
    )
    
let ``Test Cases`` = feature.GetTests ()