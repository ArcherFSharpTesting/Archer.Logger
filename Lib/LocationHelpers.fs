module Archer.Logger.LocationHelpers

open System.IO
open System.Reflection

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