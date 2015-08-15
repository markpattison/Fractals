//
// INSTRUCTIONS
//
// Move - arrow keys
// Zoom in/out - Page Up/Page Down
// Change Julia set seed - A/D/S/W
//
// Switch to showing Julia set - J (Julia set seed will be set to current position in Mandelbrot)
// Switch to showing Mandelbrot set - M (position will be set to current Julia set seed)

module Program

open Fractals

[<EntryPoint>]
let Main args =
    let game = new FractalsGame()
    do game.Run()
    0