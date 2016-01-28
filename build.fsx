// include Fake libs
#r "packages/FAKE/tools/FakeLib.dll"

open System
open Fake

// Directories
let contentDir = "./Fractals"
let buildDir  = "./build/"
let deployDir = "./deploy/"

// Filesets
let appReferences = 
    !! "**/*.fsproj"

let contentFiles =
    !! "**/*.fx"
        ++ "**/*.spritefont"

// Targets
Target "Clean" (fun _ -> 
    CleanDirs [buildDir; deployDir]
)

let mgcbArgs =
    @"/outputDir:""./Fractals"" /intermediateDir:""./intermediateContent"" /platform:Windows"

Target "BuildContent" (fun _ ->
    let contentFileList = contentFiles |> Seq.map (fun cf -> @" /build:" + "\"" + cf + "\"") |> String.concat ""
    ExecProcess (fun info ->
        info.FileName <- @"C:\Program Files (x86)\MSBuild\MonoGame\v3.0\Tools\MGCB.exe"
        info.WorkingDirectory <- "."
        info.Arguments <- mgcbArgs + contentFileList)
        (TimeSpan.FromMinutes 5.0)
    |> ignore)

Target "BuildApp" (fun _ ->
    appReferences
        |> MSBuildRelease buildDir "Build"
        |> Log "AppBuild-Output: "
)

// Build order
"Clean"
    ==> "BuildContent"
    ==> "BuildApp"

// start build
RunTargetOrDefault "BuildApp"