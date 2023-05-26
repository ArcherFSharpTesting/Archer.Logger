open Archer.Bow
open Archer
open Archer.CoreTypes.InternalTypes
open Archer.CoreTypes.InternalTypes.RunnerTypes
open Archer.Logger.Summaries
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
            | result ->
                let transformedResult = defaultTestExecutionResultSummaryTransformer result test
                printfn $"%s{transformedResult}"
            
        | _ -> ()
    | RunnerEndExecution ->
        printfn "\n"
)

runner
|> addMany [
    ``TestResult Summary Transformer Should``.``Test Cases``
    ``Test Execution Result Summary Transformer Should``.``Test Cases``
    ``Indent Transformer Should``.``Test Cases``
    ``TestExecutionResult Detail Transformer Should``.``Test Cases``
    ``TestFailContainer Transformer Should``.``Test Cases``
    ``TestSuccessContainer Transformer Should``.``Test Cases``
]
|> runAndReport