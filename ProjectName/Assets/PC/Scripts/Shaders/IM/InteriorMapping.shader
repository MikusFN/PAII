
Shader "PAII/InteriorMapping"
{
	Properties
	{
		// Texturas
		//Textura das janelas
		_TexturaJanela("Textura Janela", 2D) = "white" { }
	//Material de fora
	[NoScaleOffset]_MetalicMAP("Metallic", 2D) = "white" { }
	//Normal map das janelas
	[NoScaleOffset]_NormalMap("Normal", 2D) = "white" { }
	//Interiores
	[NoScaleOffset]_TexturaInterior("Textura Interior", 2D) = "white" { }
	//Decoracoes interiores
	[NoScaleOffset]_TexturaDecoracoes("Textura Decoracoes", 2D) = "white" { }

	// Costumizaçoes
	[Header(Costumizacao dos rooms)]
	//Profundidade dos rooms
	_RoomZvalue("Profundidade", Range(0.25 , 5)) = 2
		//Offset e largura dos rooms
		[IntRange]_RoomXvalue("Largura", Range(1 , 50)) = 4
		[IntRange]_RoomOffset("Offset", Range(0 , 50)) = 0

		[Header(Variacoes)]
	//Randomness das decoraçoes dos rooms
	_RandomSeed("Random seed", Range(0.0 , 1000.0)) = 0.0
		//Cor das janelas
		_CorJanelas("Cor das Janelas", Color) = (0.9, 0.9, 0.9)
		//Variaçoes das precianas de acordo com a etxtura
		[IntRange]_Percianas("Percianas", Range(1 , 4)) = 3
	}

		SubShader
	{
		Tags{ "RenderType" = "Opaque"   "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM

		#include "UnityCG.cginc"

		#pragma surface surf Standard 

		#pragma target 5.0

		//Uniforms
		sampler2D _TexturaJanela;
		sampler2D _TexturaInterior;
		sampler2D _TexturaDecoracoes;
		sampler2D _MetalicMAP;
		sampler2D _NormalMap;

		float _RoomXvalue;
		float _RoomZvalue;
		float _RoomOffset;
		float _Corridor;
		float _Percianas;
		float _RandomSeed;
		float3 _CorJanelas;

		//Estrutura de entrada de dados
		struct Input
		{
			float3 worldNormal;
			float3 worldPos;
			float2 uv_TexturaJanela;
			float3 viewDir;
			float4 screenPos;
		};
		//Funcao usada para um random float
		float random(float2 p)
		{
			p += _RandomSeed.xx;
			p = frac(p*0.3183099 + .1);
			p *= 17.0;
			return frac(p.x*p.y*(p.x + p.y));
		}
		//surface shader
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			//Processo automatico de vinda do vertex shader e inicializacao
			o.Normal = float3(0,0,1);
			//Vai buscar as normais do normal map de acordo com a coorddenada de tectura em que estamos
			float3 normal = UnpackNormal(tex2D(_NormalMap, IN.uv_TexturaJanela));
			//Obtem a view direction pro fragmento
			float3 viewDir = normalize(IN.viewDir.xyz + float3(normal.x, normal.y, 0.0f));
			//Inicializa a cor
			float3 color = float3(0.0f, 0.0f, 0.0f);

			//posicao do frgamento na textura
			float3 positionCellUV = float3(frac(IN.uv_TexturaJanela), 0.0f);
			//Indexacao com offset do fragmento
			float2 indexCell = floor(IN.uv_TexturaJanela) + float2(floor(_RoomOffset), 0.0f);

			//Numero de celulas que formam um quarto mais o corredor
			float cellCount = floor(_RoomXvalue);
			//transacoes esquerda direita no comprimento
			float leftShift = indexCell.x % cellCount;
			float rightShift = cellCount - leftShift - 1;

			//Primeira celula do quarto e 0 ou 1
			float firstCell = step(leftShift, 0.5f);
			//Percentagens de avanço
			rightShift *= (1 - firstCell);
			leftShift = max(0, leftShift - 1);

			//Random values para as celulas com percianas
			float randomFromCell = random(float2(floor(indexCell.x / cellCount),indexCell.y));
			//Textura a usar para as precianas (1-3)
			float percRand = floor(randomFromCell * 100) % _Percianas;

			//Direcao da luz pra celula
			float3 lightDirCell = normalize(UnityWorldSpaceLightDir(IN.worldPos));

			//Distancia maxima da uv (colocar no maximo possivel)
			float4 uvMaxDist = float4(1.0f, 1.0f, 1.0f, 100.0f);

			//Painters algoritm

			// Ground
			//Verifica se a view è maior que zero no eixo dos y ( se for verifica se 
			float isGroundVisible = step(0.0f, viewDir.y);
			//Plano do chao é a percentagem da posicao das UV relativamente à direcao
			float distToGround = (positionCellUV.y / viewDir.y);
			//A intersecçao com esse plano é a percentagem anteerior no vector de direcao menos a posicao da celula 
			//(magnitude do vector direcao na intercpcao menos pos de celula da um ponto de intercepcao)
			float3 intersectPos = positionCellUV - distToGround * viewDir;
			//O fragmento a renderizar é aquele que esta mais proximo à camera 
			//( em que se esta secao nao for vista(groundis visible) é sempre 0, mas caso seja a percentagem é de 100% e a uv mais proxima é esta em que estamos) 
			uvMaxDist = lerp(uvMaxDist, float4(frac(intersectPos.x), frac(-intersectPos.z), 0.0f, distToGround), isGroundVisible);

			// Roof
			// o roof so é visivel se o chao nao for
			float isRoofVisible = 1.0f - isGroundVisible;
			//Plano do teto é a percentagem da posicao das UV relativamente à direcao
			float distToRoof = (-(1.0f - positionCellUV.y) / viewDir.y);
			//Repete-se o processo anterior
			intersectPos = positionCellUV - distToRoof * viewDir;
			uvMaxDist = lerp(uvMaxDist, float4(1.0f + frac(intersectPos.x), frac(-intersectPos.z), 0.0f, distToRoof), isRoofVisible);

			// Left wall
			//O mesmo processo é feito para a esquerda e direita mas com a direcao horizontal (eixo x)
			float isLeftVisible = step(0.0f, viewDir.x);
			//Plano da esquerda é a percentagem da posicao das UV relativamente à direcao
			float distToLeft = ((positionCellUV.x + leftShift) / viewDir.x);
			intersectPos = positionCellUV - distToLeft * viewDir;
			uvMaxDist = lerp(uvMaxDist, float4(2.0f + frac(intersectPos.z*0.5f), intersectPos.y, 0.0f, distToLeft), step(distToLeft, uvMaxDist.w) * isLeftVisible);

			// Right wall
			float isRightVisible = 1.0f - isLeftVisible;
			//Plano da direita é a percentagem da posicao das UV relativamente à direcao
			float distToRight = (-(1.0f - positionCellUV.x + rightShift) / viewDir.x);
			intersectPos = positionCellUV - distToRight * viewDir;
			uvMaxDist = lerp(uvMaxDist, float4(2.0f + frac(intersectPos.z*0.5f), intersectPos.y, 0.0f, distToRight), step(distToRight, uvMaxDist.w) * isRightVisible);

			//A distancia à parede de traz é a percentagem da profundidade relativaemnte a view 
			float distToBack = _RoomZvalue / viewDir.z;
			intersectPos = positionCellUV - distToBack * viewDir;

			float4 decorations = float4(0.0f, 0.0f, 0.0f, 0.0f);
			//Indice da celula em que a parede vai tter decoraçoes
			float2 indexBackWall = indexCell + float2(floor(intersectPos.x), 0.0f);
			float decorationRandom = random(indexBackWall);
			float decorationModel;
			//Aplicar alguma randomness
			float decorationChance = modf(decorationRandom*4.0f, decorationModel);
			//Valor da textura de decoraçoes de acordo com o modelo a usar da textura
			decorations = tex2D(_TexturaDecoracoes, float2(frac(intersectPos.x)*0.25f + decorationModel * 0.25f, intersectPos.y))
			* step(distToBack, uvMaxDist.w) * (1.0f - firstCell) * decorationChance;
			//E caso seja a parede de traz sera esta a uv a ser renderizada
			uvMaxDist = lerp(uvMaxDist, float4(3.0f + frac(intersectPos.x*0.5f), intersectPos.y, 0.0f, distToBack), step(distToBack, uvMaxDist.w));

			//Modelo de quarto a usar na textura
			float roomModel = lerp(-floor(random(float2(1000.0f,indexCell.y))*3.0f), 1.0f, firstCell);
			//Vallor da luz directional ( dot entre direcao da luz na celula e o valor de maxima intensidade)
			float externalLightIntensity = dot(lightDirCell, float3(0.0f, 0.0f, 1.0f));
			//Avancar do modelo
			uvMaxDist.y += roomModel;
			color = tex2Dgrad(_TexturaInterior, float2(0.0f, 0.25f) + uvMaxDist.xy*0.25f, ddx(IN.uv_TexturaJanela), ddy(IN.uv_TexturaJanela));
			//Juntar as texturas
			color = lerp(color, decorations.rgb, decorations.a);

			//Valor do metallic texture
			float3 MetallicCoord = tex2D(_MetalicMAP, IN.uv_TexturaJanela);
			o.Smoothness = 1.0f - MetallicCoord.r;
			//Valor do modelo de janela a usar ( precisanas) mais os valores das uv pros planos
			float4 window = tex2Dgrad(_TexturaJanela, float2(0.0f, 0.75f - 0.25f*percRand) +
			float2(positionCellUV.x, positionCellUV.y*0.25f), ddx(IN.uv_TexturaJanela*float2(1.0f, .25f)), ddy(IN.uv_TexturaJanela*float2(1.0f, .25f)));
			//Cor da janela
			o.Emission = _CorJanelas * color * (1.0f - window.a);
			//Cor da janela
			o.Albedo = window.rgb * window.a * (lerp(_CorJanelas, float4(1.0f, 1.0f, 1.0f, 1.0f),MetallicCoord.b));
			//Valor do metallic
			o.Metallic = MetallicCoord.r;
			//Valor das normais
			o.Normal = lerp(float3(0.0f, 0.0f, 1.0f), normal,MetallicCoord.b);
		}
		ENDCG
	}
}