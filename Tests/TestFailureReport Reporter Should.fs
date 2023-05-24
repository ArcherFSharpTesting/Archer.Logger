module Archer.Logger.Tests.``TestFailureReport Reporter Should``

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


let ``Test Cases`` = feature.GetTests ()