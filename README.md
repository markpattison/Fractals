# Fractals

An F#/MonoGame program to display Mandelbrot and Julia sets.

## Instructions

Move - arrow keys

Zoom in/out - Page Up/Page Down

Move or zoom alowly - hold SHIFT

Change Julia set seed - A/D/S/W

Switch to showing Julia set - J (Julia set seed will be set to current position in Mandelbrot)

Switch to showing Mandelbrot set - M (position will be set to current Julia set seed)

Show current parameters - P

Save a picture to your "My Pictures" directory - Q

## Building and running

First run `dotnet tool restore` as a one-off.  Then to run the app locally use `dotnet fake build -t run`.
