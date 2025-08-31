open Archer.Runner
open Archer
open Archer.Types.InternalTypes
open Archer.Types.InternalTypes.RunnerTypes
open Archer.Reporting.Summaries
open Archer.Reporting.Tests
open MicroLang.Lang

let runner = runnerFactory.Runner ()

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
    ``TestIgnoreContainer Transformer Should``.``Test Cases``
]
|> runAndReport