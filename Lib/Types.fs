[<AutoOpen>]
module Archer.Logger.Types

open Archer
open Archer.CoreTypes.InternalTypes.RunnerTypes

type InformationDensity =
    | SummaryDensity
    | DetailedDensity
    
type LogScope =
    | ErrorScope
    | TestFailureScope
    | TestIgnoreScope
    | TestSuccessScope
    
type ITestLogger =
    abstract member Log: density: InformationDensity -> scope: LogScope -> string -> unit
    
type ITestResultLogger =
    abstract member LogTestResult: density: InformationDensity -> result: TestResult -> unit
    
type ITestExecutionResultLogger =
    abstract member LogExecutionResult: density: InformationDensity -> result: TestExecutionResult -> unit
    
type ITestContainerLogger =
    abstract member LogFailures: density: InformationDensity -> failures: TestFailContainer list -> unit
    abstract member LogSuccesses: density: InformationDensity -> successes: TestSuccessContainer list -> unit
    abstract member LogIgnored: density: InformationDensity -> ignored: TestIgnoreContainer list -> unit