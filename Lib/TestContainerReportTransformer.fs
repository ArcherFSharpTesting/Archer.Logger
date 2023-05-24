module Archer.Logger.TestContainerReportTransformer

open System
open Archer.CoreTypes.InternalTypes
open Archer.Logger.StringHelpers
open Archer.Logger.TestResultReportTransformers
open Microsoft.FSharp.Core

let failuresTransformer (transformFailure: IIndentTransformer -> TestFailureReport -> string) (indenter: IIndentTransformer) (failures: TestFailureReport list) =
    let transformer = transformFailure indenter
    
    failures
    |> List.map (transformer >> appendNewLine)
    |> String.concat Environment.NewLine
    |> trimEnd
    
let successesTransformer (transformSuccess: IIndentTransformer -> TestSuccessReport -> string) (indenter: IIndentTransformer) (successes: TestSuccessReport list) =
    let transformer = transformSuccess indenter
    
    successes
    |> List.map (transformer >> appendNewLine)
    |> String.concat Environment.NewLine
    |> trimEnd
    
let testContainerPartialTransformer<'a> (getReportItems: TestContainerReport -> 'a list) (itemTransformer: IIndentTransformer -> 'a -> string) (itemsTransformer: (IIndentTransformer -> 'a -> string) -> IIndentTransformer -> 'a list -> string) (indenter: IIndentTransformer) (report: TestContainerReport)  =
    let items = getReportItems report
    let transformedItems = itemsTransformer itemTransformer (indenter.Indent ()) items
    
    [
        indenter.Transform report.ContainerName
        transformedItems
    ]
    |> String.concat Environment.NewLine

let defaultTestContainerReportFailurePartialTransformer (indenter: IIndentTransformer) (report: TestContainerReport) =
    testContainerPartialTransformer<TestFailureReport> (fun c -> c.Failures) getShortTitleTestFailureTransformer failuresTransformer indenter report 
    
let defaultTestContainerReportSuccessPartialTransformer (indenter: IIndentTransformer) (report: TestContainerReport) =
    testContainerPartialTransformer<TestSuccessReport> (fun c -> c.Successes) getShortTitleTestSuccessTransformer successesTransformer indenter report
    
let private getRootNamePath (name: string) (fullNamePath: string) =
    fullNamePath.Replace (name, "")
    |> removeLastChar
    |> trim
    
let testContainerReportTransformer (partialTransformer: IIndentTransformer -> TestContainerReport -> string) name (indenter: IIndentTransformer) (report: TestContainerReport) =
    let namePath =
        report.ContainerFullName
        |> getRootNamePath report.ContainerName
    
    let l1 = indenter.Indent()
    [
        indenter.Transform namePath
        l1.Transform name
        partialTransformer (l1.Indent ()) report
    ]
    |> linesToString
    
let defaultTestContainerReportFailureTransformer (indenter: IIndentTransformer) (report: TestContainerReport) =
    testContainerReportTransformer defaultTestContainerReportFailurePartialTransformer "Failures" indenter report 

let defaultTestContainerReportSuccessReporter (indenter: IIndentTransformer) (report: TestContainerReport) =
    testContainerReportTransformer defaultTestContainerReportSuccessPartialTransformer "Successes" indenter report 
    