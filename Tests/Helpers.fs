[<AutoOpen>]
module Archer.Logger.Tests.Helpers

open Archer.Arrows
open Archer.ApprovalsSupport
open ApprovalTests

let loggerTestBuilder = Arrow.NewFeature (
    "Archer.Logger.Tests",
    "",
    
    Setup (fun _ ->
        [
            Searching
                |> findFirstReporter<Reporters.DiffReporter>
                |> findFirstReporter<Reporters.WinMergeReporter>
                |> findFirstReporter<Reporters.InlineTextReporter>
                |> findFirstReporter<Reporters.AllFailingTestsClipboardReporter>
                |> unWrapReporter
                
            Reporters.ClipboardReporter() :> Core.IApprovalFailureReporter

            Reporters.QuietReporter() :> Core.IApprovalFailureReporter
        ]
        |> buildReporter
        |> Ok
    )
)