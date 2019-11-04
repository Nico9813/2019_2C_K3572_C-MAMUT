/*
* Shader utilizado por el ejemplo "Lights/EjemploMultiDiffuseLights.cs"
* Permite aplicar iluminación dinámica con PhongShading a nivel de pixel.
* Soporta hasta 4 luces por objeto en la misma pasada.
* Las luces tienen atenuación por distancia.
* Solo se calcula el componente Diffuse para acelerar los cálculos. Se ignora
* el Specular.
*/

/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

//Textura para DiffuseMap
texture texDiffuseMap;
sampler2D diffuseMap = sampler_state
{
    Texture = (texDiffuseMap);
    ADDRESSU = WRAP;
    ADDRESSV = WRAP;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
    MIPFILTER = LINEAR;
};

//Textura para Lightmap
texture texLightMap;
sampler2D lightMap = sampler_state
{
    Texture = (texLightMap);
};

//Material del mesh
float3 materialEmissiveColor; //Color RGB
float3 materialDiffuseColor; //Color RGB

float3 materialAmbientColor; //Color RGB

float3 materialSpecularColor; //Color RGB
float materialSpecularExp; //Exponente de specular

//Variables de Fogatas
float3 lightColorFog; //Color RGB de las 4 luces
float4 lightPositionFog[2]; //Posicion de las 4 luces
float lightIntensityFog; //Intensidad de las 4 luces
float lightAttenuationFog; //Factor de atenuacion de las 4 luces

//Luz del personaje
float3 lightColorPj; //Color de la linterna o vela
float4 lightPositionPj; //Posicion de la luz
float4 eyePositionPj; //Posicion de la camara
float lightIntensityPj; //Intensidad de la luz
float lightAttenuationPj; //Factor de atenuacion de la luz

//Parametros de Spot
float3 spotLightDir; //Direccion del cono de luz
float spotLightAngleCos; //Angulo de apertura del cono de luz (en radianes)
float spotLightExponent; //Exponente de atenuacion dentro del cono de luz

float time = 0;

/**************************************************************************************/
/* MultiDiffuseLightsTechnique  PARA LAS FOGATAS*/
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT_MULTIPLE_LIGHTS
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float4 Color : COLOR;
    float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT_MULTIPLE_LIGHTS
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
    float3 WorldPosition : TEXCOORD1;
    float3 WorldNormal : TEXCOORD2;
};

//Vertex Shader
VS_OUTPUT_MULTIPLE_LIGHTS vs_general(VS_INPUT_MULTIPLE_LIGHTS input)
{
    VS_OUTPUT_MULTIPLE_LIGHTS output;

	//Proyectar posicion
    output.Position = mul(input.Position, matWorldViewProj);

	//Enviar Texcoord directamente
    output.Texcoord = input.Texcoord;

	//Posicion pasada a World-Space (necesaria para atenuación por distancia)
    output.WorldPosition = mul(input.Position, matWorld);

	/* Pasar normal a World-Space
	Solo queremos rotarla, no trasladarla ni escalarla.
	Por eso usamos matInverseTransposeWorld en vez de matWorld */
    output.WorldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;

    return output;
}

//Input del Pixel Shader
struct PS_INPUT
{
    float2 Texcoord : TEXCOORD0;
    float3 WorldPosition : TEXCOORD1;
    float3 WorldNormal : TEXCOORD2;
};

//Funcion para calcular color RGB de Diffuse
float3 computeDiffuseComponent(float3 surfacePosition, float3 N, int i)
{
	//Calcular intensidad de luz, con atenuacion por distancia
    float distAtten = length(lightPositionFog[i].xyz - surfacePosition);
    float3 Ln = (lightPositionFog[i].xyz - surfacePosition) / distAtten;
    distAtten = distAtten * lightAttenuationFog;
    float intensity = lightIntensityFog / distAtten; //Dividimos intensidad sobre distancia

	//Calcular Diffuse (N dot L)
	return abs(intensity * lightColorFog.rgb * materialDiffuseColor);
}

//Pixel Shader para Point Light
float4 point_light_ps(PS_INPUT input) : COLOR0
{
    float3 Nn = normalize(input.WorldNormal);

	//Emissive + Diffuse de 4 luces PointLight
    float3 diffuseLighting = materialEmissiveColor;

	//Diffuse 0
    diffuseLighting += computeDiffuseComponent(input.WorldPosition, Nn, 0);

	//Diffuse 1
    diffuseLighting += computeDiffuseComponent(input.WorldPosition, Nn, 1);

	//Diffuse 2
    //diffuseLighting += computeDiffuseComponent(input.WorldPosition, Nn, 2);

	//Diffuse 3
    //diffuseLighting += computeDiffuseComponent(input.WorldPosition, Nn, 3);

	//Obtener texel de la textura
    float4 texelColor = tex2D(diffuseMap, input.Texcoord);
    texelColor.rgb *= diffuseLighting;

    return texelColor;
}


/*
* Technique Solo fogatas
*/
technique Fogatas
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_general();
        PixelShader = compile ps_3_0 point_light_ps();
    }
}

/**************************************************************************************/
/* DIFFUSE_MAP */ /////////PARA LA LINTERNA Y VELA////////////
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT_DIFFUSE_MAP
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float4 Color : COLOR0;
    float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT_DIFFUSE_MAP
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
    float3 WorldPosition : TEXCOORD1;
    float3 WorldNormal : TEXCOORD2;
    float3 LightVec : TEXCOORD3;
    float3 HalfAngleVec : TEXCOORD4;
    float4 Color : COLOR0;
};

//Vertex Shader
VS_OUTPUT_DIFFUSE_MAP vs_DiffuseMap(VS_INPUT_DIFFUSE_MAP input)
{
    VS_OUTPUT_DIFFUSE_MAP output;

	//Proyectar posicion
    output.Position = mul(input.Position, matWorldViewProj);

	//Enviar Texcoord directamente
    output.Texcoord = input.Texcoord;

	//Posicion pasada a World-Space (necesaria para atenuación por distancia)
    output.WorldPosition = mul(input.Position, matWorld);

	/* Pasar normal a World-Space
	Solo queremos rotarla, no trasladarla ni escalarla.
	Por eso usamos matInverseTransposeWorld en vez de matWorld */
    output.WorldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;

	//LightVec (L): vector que va desde el vertice hacia la luz. Usado en Diffuse y Specular
    output.LightVec = lightPositionPj.xyz - output.WorldPosition;

	//ViewVec (V): vector que va desde el vertice hacia la camara.
    float3 viewVector = eyePositionPj.xyz - output.WorldPosition;

	//HalfAngleVec (H): vector de reflexion simplificado de Phong-Blinn (H = |V + L|). Usado en Specular
    output.HalfAngleVec = viewVector + output.LightVec;
    //Propago el color x vertice
    output.Color = input.Color;

    return output;
}

//Input del Pixel Shader
struct PS_DIFFUSE_MAP
{
    float2 Texcoord : TEXCOORD0;
    float3 WorldPosition : TEXCOORD1;
    float3 WorldNormal : TEXCOORD2;
    float3 LightVec : TEXCOORD3;
    float3 HalfAngleVec : TEXCOORD4;
};

//Pixel Shader
float4 ps_DiffuseMap(PS_DIFFUSE_MAP input) : COLOR0
{
	//Normalizar vectores
    float3 Nn = normalize(input.WorldNormal);
    float3 Ln = normalize(input.LightVec);
    float3 Hn = normalize(input.HalfAngleVec);

	//Calcular atenuacion por distancia
    float distAtten = length(lightPositionPj.xyz - input.WorldPosition) * lightAttenuationPj;

	//Calcular atenuacion por Spot Light. Si esta fuera del angulo del cono tiene 0 intensidad.
    float spotAtten = dot(-spotLightDir, Ln);
    spotAtten = (spotAtten > spotLightAngleCos)
					? pow(spotAtten, spotLightExponent)
					: 0.0;

	//Calcular intensidad de la luz segun la atenuacion por distancia y si esta adentro o fuera del cono de luz
    float intensity = lightIntensityPj * spotAtten / distAtten;

	//Obtener texel de la textura
    float4 texelColor = tex2D(diffuseMap, input.Texcoord);

	//Componente Ambient
    float3 ambientLight = intensity * lightColorPj * materialAmbientColor;

	//Componente Diffuse: N dot L
    float3 n_dot_l = dot(Nn, Ln);
    float3 diffuseLight = intensity * lightColorPj * materialDiffuseColor.rgb * max(0.0, n_dot_l); //Controlamos que no de negativo
    //Diffuse 0
    diffuseLight += computeDiffuseComponent(input.WorldPosition, Nn, 0);

	//Diffuse 1
    diffuseLight += computeDiffuseComponent(input.WorldPosition, Nn, 1);


	//Componente Specular: (N dot H)^exp
    float3 n_dot_h = dot(Nn, Hn);
    float3 specularLight = n_dot_l <= 0.0
			? float3(0.0, 0.0, 0.0)
			: (intensity * lightColorPj * materialSpecularColor * pow(max(0.0, n_dot_h), materialSpecularExp));

	/* Color final: modular (Emissive + Ambient + Diffuse) por el color de la textura, y luego sumar Specular.
	   El color Alpha sale del diffuse material */
    float4 finalColor = float4(saturate(materialEmissiveColor + ambientLight + diffuseLight) * texelColor + specularLight, 1);

    return finalColor;
}

/*
* Technique linterna o vela + fogatas
*/
technique Spotlight
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_DiffuseMap();
        PixelShader = compile ps_3_0 ps_DiffuseMap();
    }
}


VS_OUTPUT_DIFFUSE_MAP vs_Sepia(VS_INPUT_DIFFUSE_MAP input)
{
    VS_OUTPUT_DIFFUSE_MAP output;

	//Proyectar posicion
    output.Position = mul(input.Position, matWorldViewProj);
    output.Position.y -=    5 * sin(10 * time) - 5 * cos(5*time);
	//Enviar Texcoord directamente
    output.Texcoord = input.Texcoord;

	//Posicion pasada a World-Space (necesaria para atenuación por distancia)
    output.WorldPosition = mul(input.Position, matWorld);

	/* Pasar normal a World-Space
	Solo queremos rotarla, no trasladarla ni escalarla.
	Por eso usamos matInverseTransposeWorld en vez de matWorld */
    output.WorldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;

	//LightVec (L): vector que va desde el vertice hacia la luz. Usado en Diffuse y Specular
    output.LightVec = lightPositionPj.xyz - output.WorldPosition;

	//ViewVec (V): vector que va desde el vertice hacia la camara.
    float3 viewVector = eyePositionPj.xyz - output.WorldPosition;

	//HalfAngleVec (H): vector de reflexion simplificado de Phong-Blinn (H = |V + L|). Usado en Specular
    output.HalfAngleVec = viewVector + output.LightVec;


	//Propago el color x vertice
    output.Color = input.Color;

    return output;
}


float4 ps_Sepia(PS_DIFFUSE_MAP input) : COLOR0
{

    float3 Nn = normalize(input.WorldNormal);
    float3 diffuseLighting = (0.5,0.5,0.5);

	//Diffuse 0
    diffuseLighting += computeDiffuseComponent(input.WorldPosition, Nn, 0);

	//Diffuse 1
    diffuseLighting += computeDiffuseComponent(input.WorldPosition, Nn, 1);
    float4 outputColor = tex2D(diffuseMap, input.Texcoord);
    outputColor.r = outputColor.r * abs(sin(1.5*time));
    outputColor.g *= 0.1;
    outputColor.b *= 0.1;
    outputColor.rgb *= diffuseLighting;
    return outputColor;
}

technique Sepia
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_Sepia();
        PixelShader = compile ps_3_0 ps_Sepia();
    }
}