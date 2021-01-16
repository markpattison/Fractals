module Fractals.Program

[<EntryPoint>]
let main argv =
    let game = new Game1()
    do game.Run()
    0
