﻿[<AutoOpen>]
module Archer.Logger.Tests.Helpers

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Archer.Arrows
open Archer.ApprovalsSupport
open ApprovalTests
open Archer.CoreTypes.InternalTypes.RunnerTypes

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
                
            Reporters.ClipboardReporter () :> Core.IApprovalFailureReporter

            Reporters.QuietReporter () :> Core.IApprovalFailureReporter
        ]
        |> buildReporter
        |> Ok
    )
)

type NameStart =
    | StartNamingAt of int
    
type FakeTestBuilder =
    static member BuildTest (nameModifier, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let buildIt (path: string) (name: string) = 
            let f = Arrow.NewFeature (path, name)
            let t = f.Ignore ($"Test %d{nameModifier}", Setup Ok, TestBody "Ignore", Teardown (fun _ _ -> Ok ()), fileFullName, lineNumber)
            t
            
        buildIt
        
    static member BuildNTests (testCount: int, [<CallerFilePath; Optional; DefaultParameterValue("")>] fileFullName: string, [<CallerLineNumber; Optional; DefaultParameterValue(-1)>] lineNumber: int) =
        let buildIt (StartNamingAt nameStart) (path: string) (name: string) =
            [0..(testCount - 1)]
            |> List.map (fun i -> FakeTestBuilder.BuildTest ((i + nameStart), fileFullName, lineNumber) path name)
            
        buildIt