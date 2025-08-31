module Archer.Reporting.LocationHelpers

open System.IO
open System.Reflection
open Archer

let getSolutionRoot (assembly: Assembly) =
    let assemblyPath = (assembly.Location |> FileInfo).Directory.FullName
    
    let rec findSolution path =
        if System.String.IsNullOrWhiteSpace path then ""
        else
            let current = DirectoryInfo path
            let files = current.EnumerateFiles "*.sln"
            
            if files |> Seq.isEmpty then findSolution current.Parent.FullName
            else
                (files |> Seq.head).Directory.FullName

    findSolution assemblyPath
    
let getRelativePath (assembly: Assembly) (dir: DirectoryInfo) =
    let rootPath = getSolutionRoot assembly
    dir.FullName.Replace(rootPath, ".")
    
let getRelativePathFromPath assembly path =
    path
    |> DirectoryInfo
    |> getRelativePath assembly
    
let getRelativePathFromTest assembly (test: ITestLocationInfo) =
    test.Location.FilePath
    |> getRelativePathFromPath assembly
    
let getRelativeFilePath assembly (test: ITestLocationInfo) =
    let path =
        test
        |> getRelativePathFromTest assembly
    
    Path.Combine (path, test.Location.FileName)