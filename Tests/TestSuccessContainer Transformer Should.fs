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
            Ok (reporter)
        )
    )
    
// let ``Transform a success container``
    
let ``Test Cases`` = feature.GetTests ()