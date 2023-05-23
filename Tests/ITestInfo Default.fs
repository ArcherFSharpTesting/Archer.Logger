module Archer.Logger.Tests.``ITestInfo Default``

open System.IO
open Archer
open Archer.Arrows
open Archer.ApprovalsSupport
open Archer.Logger

let private feature =
    reporterTestBuilder
    |> Sub.Feature (
        TestTags [
            Category "Test Reporter"
            Category "Approvals"
        ]
    )
    
let ``Test Name Reporter Should return a formatted name`` =
    feature.Test (fun reporter environment ->
        let testInfo = environment.TestInfo
        
        testInfo
        |> defaultTestNameReporter
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Test Name Reporter Should be able to return a different name`` =
    feature.Test (fun reporter environment ->
        let testInfo = environment.TestInfo
        
        testInfo
        |> defaultTestNameReporter
        |> Should.MeetStandard reporter testInfo
    )
    
let private getRoot (testInfo: ITestLocationInfo) =
    Path.Join (testInfo.Location.FilePath, "..")
    |> DirectoryInfo
    
let ``Test Location Reporter should return a formatted location`` =
    feature.Test (
        fun reporter environment ->
            let testInfo = environment.TestInfo
            
            testInfo
            |> defaultTestLocationReporter getRoot
            |> Should.MeetStandard reporter testInfo
    )
    
let ``Test Location Reporter should be able to return a different location`` =
    feature.Test (fun reporter environment ->
        let testInfo = environment.TestInfo
        
        testInfo
        |> defaultTestLocationReporter getRoot
        |> Should.MeetStandard reporter testInfo
    )
    
let ``Test Reporter should use Test Name and Location Reporter to return a formatted string`` =
    feature.Test (fun reporter environment ->
        let testInfo = environment.TestInfo
        
        testInfo
        |> defaultTestReporter defaultTestNameReporter (defaultTestLocationReporter getRoot)
        |> Should.MeetStandard reporter testInfo
    )

let ``Test Cases`` = feature.GetTests ()