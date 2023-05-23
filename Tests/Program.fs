open Archer.Bow
open Archer
open Archer.CoreTypes.InternalTypes
open Archer.CoreTypes.InternalTypes.RunnerTypes
open Archer.Logger.Tests
open MicroLang.Lang

let runner = bow.Runner ()

runner.RunnerLifecycleEvent
|> Event.add (fun args ->
    match args with
    | RunnerStartExecution _ ->
        printfn ""
    | RunnerTestLifeCycle (test, testEventLifecycle, _) ->
        match testEventLifecycle with
        | TestEndExecution testExecutionResult ->
            match testExecutionResult with
            | TestExecutionResult TestSuccess -> ()
            | TestExecutionResult (TestFailure (TestIgnored _)) ->
                let report = $"%A{test} : (Ignored)"
                printfn $"%s{report}"
            | _ ->
                let report = $"%A{test} : (Fail) @ %i{test.Location.LineNumber}"
                printfn $"%s{report}"
            
        | _ -> ()
    | RunnerEndExecution ->
        printfn "\n"
)

runner
|> addMany [
    ``ITestInfo Default``.``Test Cases``
]
|> runAndReport