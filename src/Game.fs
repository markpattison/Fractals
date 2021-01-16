namespace Fractals

open System.IO
open System
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

open Input

type Fractal = Mandelbrot | Julia

type Game1() as _this =
    inherit Game()
    let graphics = new GraphicsDeviceManager(_this)

    do graphics.GraphicsProfile <- GraphicsProfile.HiDef

    do graphics.PreferredBackBufferWidth <- 800
    do graphics.PreferredBackBufferHeight <- 600
    do graphics.IsFullScreen <- false

    //do graphics.PreferredBackBufferWidth <- 2560
    //do graphics.PreferredBackBufferHeight <- 1440
    //do graphics.IsFullScreen <- true

    do graphics.ApplyChanges()
    do base.Content.RootDirectory <- "content"
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let mutable spriteFont = Unchecked.defaultof<SpriteFont>

    let mutable input = Unchecked.defaultof<Input>
    let mutable originalMouseState = Unchecked.defaultof<MouseState>

    let mutable (vertices: VertexPositionTexture[]) = [| |]
    let indices = [| 0; 2; 1; 1; 2; 3 |]
    let mutable effect = Unchecked.defaultof<Effect>
    let mutable widthOverHeight = 0.0f
    let mutable zoom = 0.314f
    let mutable offset = Vector2(0.0f, 0.0f)
    let mutable juliaSeed = Vector2(0.0f, 0.0f)
    let mutable fractal = Mandelbrot
    let mutable showParameters = false

    let getDirectionalInput keyPos keyNeg (input: Input) =
        (if input.IsPressed keyPos then 1.0f else 0.0f) - (if input.IsPressed keyNeg then 1.0f else 0.0f)

    let getLeftRight = getDirectionalInput Keys.Left Keys.Right
    let getDownUp = getDirectionalInput Keys.Down Keys.Up
    let getPageDownPageUp = getDirectionalInput Keys.PageDown Keys.PageUp
    let getDA = getDirectionalInput Keys.D Keys.A
    let getWS = getDirectionalInput Keys.W Keys.S

    let getInput (input: Input) =
        let scaled = if (input.IsPressed Keys.LeftShift || input.IsPressed Keys.RightShift) then 0.1f else 1.0f

        let move = scaled * 0.005f * Vector2(getLeftRight input, getDownUp input) / zoom
        let zoomIn = 1.02f ** (scaled * getPageDownPageUp input)
        let changeSeed = scaled * 0.005f * Vector2(getDA input, getWS input)

        (-move, zoomIn, changeSeed)
    
    let ShowParameters() =
        spriteBatch.Begin()

        let textHeight = spriteFont.MeasureString("Hello").Y
        let colour = Color(128, 128, 128, 128)

        match fractal with
        | Mandelbrot ->
            spriteBatch.DrawString(spriteFont, "Mandelbrot", Vector2(0.0f, 0.0f), colour)
            spriteBatch.DrawString(spriteFont, sprintf "X = %.6f" offset.X, Vector2(0.0f, textHeight), colour)
            spriteBatch.DrawString(spriteFont, sprintf "Y = %.6f" offset.Y, Vector2(0.0f, 2.0f * textHeight), colour)
            spriteBatch.DrawString(spriteFont, sprintf "Zoom = %.6f" zoom, Vector2(0.0f, 3.0f * textHeight), colour)

        | Julia ->
            spriteBatch.DrawString(spriteFont, "Julia", Vector2(0.0f, 0.0f), colour)
            spriteBatch.DrawString(spriteFont, sprintf "Seed X = %.6f" juliaSeed.X, Vector2(0.0f, textHeight), colour)
            spriteBatch.DrawString(spriteFont, sprintf "Seed Y = %.6f" juliaSeed.Y, Vector2(0.0f, 2.0f * textHeight), colour)
            spriteBatch.DrawString(spriteFont, sprintf "X = %.6f" offset.X, Vector2(0.0f, 3.0f * textHeight), colour)
            spriteBatch.DrawString(spriteFont, sprintf "Y = %.6f" offset.Y, Vector2(0.0f, 4.0f * textHeight), colour)
            spriteBatch.DrawString(spriteFont, sprintf "Zoom = %.6f" zoom, Vector2(0.0f, 5.0f * textHeight), colour)

        spriteBatch.End()

    member _this.TakeScreenShot(gameTime) =
        use screenShot = new RenderTarget2D(_this.GraphicsDevice, _this.GraphicsDevice.PresentationParameters.BackBufferWidth, _this.GraphicsDevice.PresentationParameters.BackBufferHeight)
        _this.GraphicsDevice.SetRenderTarget(screenShot)
        _this.Draw gameTime
        _this.GraphicsDevice.SetRenderTarget(null)
        let getFileName i = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), sprintf "Fractal%05i.png" i)
        let fileIndex = { 1 .. 99999 } |> Seq.find (fun i -> not (File.Exists(getFileName i)))
        let fileName = getFileName fileIndex
        use stream = new FileStream(fileName, FileMode.Create)
        screenShot.SaveAsPng(stream, screenShot.Width, screenShot.Height)

    override _this.Initialize() =
        base.Initialize()  

    override _this.LoadContent() =
        vertices <-
            [|
                VertexPositionTexture(Vector3(-1.0f, -1.0f, 0.0f), Vector2(0.0f, 0.0f))
                VertexPositionTexture(Vector3( 1.0f, -1.0f, 0.0f), Vector2(1.0f, 0.0f))
                VertexPositionTexture(Vector3(-1.0f,  1.0f, 0.0f), Vector2(0.0f, 1.0f))
                VertexPositionTexture(Vector3( 1.0f,  1.0f, 0.0f), Vector2(1.0f, 1.0f))
            |]

        effect <- _this.Content.Load<Effect>("effects/effects")
        spriteFont <- _this.Content.Load<SpriteFont>("Fonts/Miramo")

        spriteBatch <- new SpriteBatch(_this.GraphicsDevice)

        widthOverHeight <- (single graphics.PreferredBackBufferWidth) / (single graphics.PreferredBackBufferHeight)

        //Mouse.SetPosition(_this.Window.ClientBounds.Width / 2, _this.Window.ClientBounds.Height / 2)
        originalMouseState <- Mouse.GetState()
        input <- Input(Keyboard.GetState(), Keyboard.GetState(), Mouse.GetState(), Mouse.GetState(), _this.Window, originalMouseState, 0, 0)



    override _this.Update(gameTime) =
        let time = float32 gameTime.TotalGameTime.TotalSeconds

        input <- input.Updated(Keyboard.GetState(), Mouse.GetState(), _this.Window)

        if input.Quit then _this.Exit()
        if input.JustPressed(Keys.P) then showParameters <- not showParameters

        let (move, zoomIn, changeSeed) = getInput input

        offset <- offset + move
        zoom <- zoom / zoomIn
        if fractal = Julia then juliaSeed <- juliaSeed + changeSeed

        if (fractal = Julia && input.JustPressed Keys.M) then
            fractal <- Mandelbrot
            offset <- juliaSeed

        if (fractal = Mandelbrot && input.JustPressed Keys.J) then
            fractal <- Julia
            juliaSeed <- offset

        if input.JustPressed(Keys.Q) then _this.TakeScreenShot gameTime

        base.Update(gameTime)

    override _this.Draw(gameTime) =
        let time = (single gameTime.TotalGameTime.TotalMilliseconds) / 100.0f

        let effectName =
            match fractal with
            | Julia -> "Julia"
            | Mandelbrot -> "Mandelbrot"

        _this.GraphicsDevice.Clear(Color.Red)
        effect.CurrentTechnique <- effect.Techniques.[effectName]
        effect.Parameters.["Zoom"].SetValue(zoom)
        effect.Parameters.["WidthOverHeight"].SetValue(widthOverHeight)
        effect.Parameters.["Offset"].SetValue(offset)
        effect.Parameters.["JuliaSeed"].SetValue(juliaSeed)

        effect.CurrentTechnique.Passes |> Seq.iter
            (fun pass ->
                pass.Apply()
                _this.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3)
            )

        if showParameters then ShowParameters()

        base.Draw(gameTime)
