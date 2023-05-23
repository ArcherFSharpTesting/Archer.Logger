module Archer.Logger.LocationHelpers

open System.IO
open System.Reflection

(*
public static class FileUtils
{
    public static string GetAssemblyFileName() => GetAssemblyPath().Split(@"\").Last();
    public static string GetAssemblyDir() => Path.GetDirectoryName(GetAssemblyPath());
    public static string GetAssemblyPath() => Assembly.GetExecutingAssembly().Location;
    public static string GetSolutionFileName() => GetSolutionPath().Split(@"\").Last();
    public static string GetSolutionDir() => Directory.GetParent(GetSolutionPath()).FullName;
    public static string GetSolutionPath()
    {
        var currentDirPath = GetAssemblyDir();
        while (currentDirPath != null)
        {
            var fileInCurrentDir = Directory.GetFiles(currentDirPath).Select(f => f.Split(@"\").Last()).ToArray();
            var solutionFileName = fileInCurrentDir.SingleOrDefault(f => f.EndsWith(".sln", StringComparison.InvariantCultureIgnoreCase));
            if (solutionFileName != null)
                return Path.Combine(currentDirPath, solutionFileName);

            currentDirPath = Directory.GetParent(currentDirPath)?.FullName;
        }

        throw new FileNotFoundException("Cannot find solution file path");
    }
}*)

let getRelativePath (assembly: Assembly) (dir: DirectoryInfo) =
    let assemblyPath = (assembly.Location |> FileInfo).Directory.FullName
    
    let rec findSolution path =
        if System.String.IsNullOrWhiteSpace path then ""
        else
            let current = DirectoryInfo path
            let files = current.EnumerateFiles "*.sln"
            
            if files |> Seq.isEmpty then findSolution current.Parent.FullName
            else
                (files |> Seq.head).Directory.FullName
            
    let rootPath = findSolution assemblyPath
    dir.FullName.Replace(rootPath, ".")