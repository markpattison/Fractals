module Program

open Fractals

[<EntryPoint>]
let Main args =
    let game = new FractalsGame()
    do game.Run()
    0
