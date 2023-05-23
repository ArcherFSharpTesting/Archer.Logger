[<AutoOpen>]
module Archer.Logger.Defaults

open System.IO
open Archer

let defaultTestNameReporter (testInfo: ITestNameInfo) = $"%s{testInfo.ContainerPath}.%s{testInfo.ContainerName}.%s{testInfo.TestName}"

let defaultTestLocationReporter (getRoot: ITestLocationInfo -> DirectoryInfo) (testInfo: ITestLocationInfo) =
    let dir = testInfo.Location.FilePath |> DirectoryInfo
    let root = testInfo |> getRoot
    
    let rec buildPath (dir: DirectoryInfo) (acc: string list) =
        if dir.Name = root.Name then
            acc |> List.rev |> Seq.ofList |> String.concat "\\"
        elif dir.Root.Name = dir.Name then
            dir.Name::acc |> List.rev |> Seq.ofList |> String.concat "\\"
        else
            dir.Name::acc |> buildPath dir.Parent
            
    
    let path = buildPath dir []
    $".\\%s{path}\\%s{testInfo.Location.FileName} @ %d{testInfo.Location.LineNumber}"
    
let defaultTestReporter _ _ _ =
    [
        "Archer.Logger.Tests.ITestInfo Default.Test Reporter should use Test Name and Location Reporter to return a formatted string"
        ".\\Tests\\ITestInfo Default.fs @ 60"
    ]
    |> String.concat "\r\n"