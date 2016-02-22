// include Fake libs
#r "packages/FAKE/tools/FakeLib.dll"
#r "packages/FAKE/tools/FakeMonoGame.dll"

open System
open Fake
open Fake.MonoGameContent

// Directories
let intermediateContentDir = "./intermediateContent"
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

Target "BuildContent" (fun _ ->
    contentFiles
        |> MonoGameContent (fun p ->
            { p with
                OutputDir = contentDir;
                IntermediateDir = intermediateContentDir;
            }))

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