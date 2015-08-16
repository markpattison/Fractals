float Zoom;
float WidthOverHeight;
float2 Offset;

float2 JuliaSeed;

struct VertexShaderInput
{
    float4 Position : SV_POSITION;
    float2 TextureCoords: TEXCOORD0;
};

struct VertexToPixel
{
    float4 Position : SV_POSITION;
    float2 TextureCoords: TEXCOORD0;
};

struct PixelToFrame
{
    float4 Color : COLOR0;
};

VertexToPixel VertexShader1(VertexShaderInput input)
{
    VertexToPixel output;

    output.Position = input.Position;
    output.TextureCoords = input.TextureCoords;

    return output;
}

float2 CalculatePosition(float2 inputCoords, float zoom, float widthOverHeight, float2 offset)
{
	return (inputCoords - 0.5) * zoom * float2(widthOverHeight, 1.0) + offset;
}

float4 ApplyColourMap(float x)
{
	return float4(sin(x * 4.0), sin(x * 5.0), sin(x * 6.0), 1.0);
}

float2 Function(float2 z, float2 offset)
{
	return float2(z.x * z.x - z.y * z.y, 2.0 * z.x * z.y) + offset;
}

float CalculatePixelResult(float2 z, float2 offset)
{
	float result = 0.0;

	float2 zsq = z * z;

	int iteration = 0;
	int maxIteration = 128;

	while (zsq.x + zsq.y <= 49.0 && iteration < maxIteration)
	{
		z = Function(z, offset);
		zsq = z * z;

		iteration = iteration + 1;
	}

	if (iteration == maxIteration)
	{
		result = 0.0;
	}
	else
	{
		result = iteration + (log(2.0 * log(7.0)) - log(log(zsq.x + zsq.y))) / log(2.0);

		result = log(result * 0.4) / log(maxIteration);
	}

	return result;
}

PixelToFrame MandelbrotPS(VertexToPixel input)
{
    PixelToFrame output;
    
    float2 z = CalculatePosition(input.TextureCoords, Zoom, WidthOverHeight, Offset);
    
    float result = CalculatePixelResult(z, z);
    
	output.Color = ApplyColourMap(result);
    
    return output;
}

PixelToFrame JuliaPS(VertexToPixel input)
{
    PixelToFrame output;
    
    float2 z = CalculatePosition(input.TextureCoords, Zoom, WidthOverHeight, Offset);
    
    float result = CalculatePixelResult(z, JuliaSeed);
    
	output.Color = ApplyColourMap(result);
    
    return output;
}

technique Mandelbrot
{
    pass Pass0
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_4_0 VertexShader1();
        PixelShader = compile ps_4_0 MandelbrotPS();
    }
}

technique Julia
{
    pass Pass0
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_4_0 VertexShader1();
        PixelShader = compile ps_4_0 JuliaPS();
    }
}
