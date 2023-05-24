module Archer.Logger.Tests.``Indent Reporter Should``

open System
open Archer
open Archer.Arrows
open Archer.ApprovalsSupport
open Archer.Fletching.Types.Internal
open Archer.Logger

let private feature =
    reporterTestBuilder
    |> Sub.Feature (
        TestTags [
            Category "Test Reporter"
            Category "Approvals"
        ]
    )
    
// Zero
let ``When given 0 should return string given to it`` =
    feature.Test (fun _ ->
        let expected = "Hello World"
        
        let reporter = IndentReporter 0
        let result = reporter.Report expected
        
        result
        |> Should.BeEqualTo expected
    )
    
let ``When given 0 should return different string given to it`` =
    feature.Test (fun _ ->
        let expected = "Good Bye World"
        
        let reporter = IndentReporter 0
        let result = reporter.Report expected
        
        result
        |> Should.BeEqualTo expected
    )
    
let ``When indent is called return string given to it starting with a tab`` =
    feature.Test (fun _ ->
        let expected = "Hello World"
        
        let reporter = (IndentReporter 0).Indent ()
        let result = reporter.Report expected
        
        result
        |> Should.BeEqualTo $"\t%s{expected}"
    )
    
// One
let ``When given 1 return string given to it starting with a tab`` =
    feature.Test (fun _ ->
        let expected = "Hello World"
        
        let reporter = IndentReporter 1
        let result = reporter.Report expected
        
        result
        |> Should.BeEqualTo $"\t%s{expected}"
    )
    
let ``When given zero indent called with one return string given to it starting with a tab`` =
    feature.Test (fun _ ->
        let expected = "Hello World"
        
        let reporter = (IndentReporter 0).Indent 1
        let result = reporter.Report expected
        
        result
        |> Should.BeEqualTo $"\t%s{expected}"
    )
    
let ``When given TwoSpaces in constructor it will indent with two spaces`` =
    feature.Test (fun _ ->
        let expected = "Hello World"
        
        let reporter = IndentReporter (1, TwoSpaces)
        let result = reporter.Report expected
        
        result
        |> Should.BeEqualTo $"  %s{expected}"
    )
    
// Many
let ``When given 2 return string given to it starting two tabs`` =
    feature.Test (fun _ ->
        let expected = "Hello World"
        
        let reporter = IndentReporter 2
        let result = reporter.Report expected
        
        result
        |> Should.BeEqualTo $"\t\t%s{expected}"
    )
    
let ``When given 1 return each line of the string given to it starting with a tab`` =
    feature.Test (fun _ ->
        let expected = "Hello" + Environment.NewLine + "World"
        
        let reporter = IndentReporter 1
        let result = reporter.Report expected
        
        result
        |> Should.BeEqualTo ("\tHello" + Environment.NewLine + "\tWorld")
    )
    
let ``When given one indent called with one return string given to it starting with two tabs`` =
    feature.Test (fun _ ->
        let expected = "Hello World"
        
        let reporter = (IndentReporter 1).Indent 1
        let result = reporter.Report expected
        
        result
        |> Should.BeEqualTo $"\t\t%s{expected}"
    )
    
let ``When given zero indent called with two return string given to it starting with two tabs`` =
    feature.Test (fun _ ->
        let expected = "Hello World"
        
        let reporter = (IndentReporter 0).Indent 2
        let result = reporter.Report expected
        
        result
        |> Should.BeEqualTo $"\t\t%s{expected}"
    )
    
let ``Test Cases`` = feature.GetTests ()