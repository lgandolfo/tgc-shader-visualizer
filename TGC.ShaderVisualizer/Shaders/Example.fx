
/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

float time = 0;

//Textura
texture textureExample;
sampler2D textureSampler = sampler_state
{
    Texture = (textureExample);
    ADDRESSU = MIRROR;
    ADDRESSV = MIRROR;
	MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};


float distanceToShape(int sides, float2 position)
{
    float angle = atan2(position.y, position.x) + 3.14;
    float radius = 3.14 * 2.0 / float(sides);

    return cos(floor(.5 + angle / radius) * radius - angle) * length(position);
}

float2 random2(float2 input)
{
    return frac(sin(float2(dot(input, float2(127.1, 311.7)), dot(input, float2(269.5, 183.3)))) * 43758.5453);
}

float2 rotate2D(float2 position, float angle)
{
    position -= 0.5;
    position = mul(float2x2(cos(angle), -sin(angle),
						    sin(angle), cos(angle)), position);
    position += 0.5;
    return position;
}


//Input del Vertex Shader
struct VS_INPUT
{
	float4 Position : POSITION0;
	float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
	float4 Position : POSITION0;
	float2 Texcoord : TEXCOORD0;
	float3 MeshPosition : TEXCOORD1;
};

//Vertex Shader
VS_OUTPUT vsDefault(VS_INPUT input)
{
	VS_OUTPUT output;

    output.Position = input.Position;
	output.Texcoord = input.Texcoord;
	output.MeshPosition = input.Position;

	return output;
}

float dibujarSeno(float2 position)
{
	
	float grosor = 0.01;
	float altura = 3;
	float amplitud = 4;
	float desplazamiento = -0.5;

	float s = desplazamiento + sin(position.x * amplitud) / altura;

	return abs(step(s,position.y - grosor/2) - step(s,position.y + grosor/2));

}

float dibujarCuadricula(float2 position)
{
	
	float vertical = 0.5;
	float horizontal = -0.5;

	return abs(step(horizontal,position.y) - step(vertical,position.x));
}


//Pixel Shader
float4 psDefault(VS_OUTPUT input) : COLOR0
{
	float2 position = input.MeshPosition;

	float color;

	color = abs(dibujarSeno(position) - dibujarCuadricula(position));
	
    return float4(color, color, color, color);
}



technique Default 
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vsDefault();
		PixelShader = compile ps_3_0 psDefault();
	}
}