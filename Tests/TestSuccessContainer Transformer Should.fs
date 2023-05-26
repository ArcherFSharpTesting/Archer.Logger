module Archer.Logger.Tests.``TestSuccessContainer Transformer Should``

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
    loggerTestBuilder
    |> Sub.Ignore (
        TestTags [
            Category "Detail Transformers"
            Category "Transformers"
            Category "Approvals"
            Category "TestExecutionResult"
        ],
        Setup (fun reporter ->
            Ok (reporter)
        )
    )
    
let ``Test Cases`` = feature.GetTests ()