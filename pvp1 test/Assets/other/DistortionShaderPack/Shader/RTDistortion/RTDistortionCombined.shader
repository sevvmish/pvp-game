// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/*
 * By Martin Reintges 05/2016
 */

Shader "DistortionShaderPack/RTDistortionCombined"
{
	Properties
	{
		_MainColor("MainColor", Color) = (0,0,0,1)
		_RenderTexture ("RenderTexture", 2D) = "black"
		_NormalTexture ("Normal", 2D) = "blue" {}
		_DistortionStrength ("DistortionStrength", Range(-1,1)) = -1
		_NormalTexStrength ("_NormalTexStrength", Range(0,1)) = 0.5
		_NormalTexFrameless ("_NormalTexFrameless", Range(0,1)) = 0.5
		_StrengthAlpha ("AlphaStrength", Range(0,1)) = 0.5
		_UVOffset ("UVOffset XY, generated ZW", Vector) = (0,0,0,0)
	}
	SubShader 
	{
		// Subshader Tags
		Tags { "Queue"="Transparent-1" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		
		// Render Pass
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma target 2.0
			
			#pragma fragmentoption ARB_precision_hint_fastest
			
			#pragma vertex vert
			#pragma fragment frag
	
	
		////// Uniform user variable definition
			uniform sampler2D _RenderTexture;
			uniform sampler2D _NormalTexture;
			uniform float4 _NormalTexture_ST;
			uniform float _DistortionStrength;
			uniform float _NormalTexStrength;
			uniform float _NormalTexFrameless;
			uniform float4 _MainColor;
			uniform float4 _UVOffset;
			uniform float _StrengthAlpha;
			uniform float _ImplodeExplode;

	
		////// Input structs
			struct VertexInput 
			{
				float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
			};
			struct Vert2Frag
			{
				float4 pos : SV_POSITION;
				float4 posScreen : TEXCOORD0;
                float2 uv : TEXCOORD1;
			};
	
		////// Shader functions
			// Vertex shader
			Vert2Frag vert(VertexInput vertIn)
			{
				Vert2Frag output;
				
				output.pos = UnityObjectToClipPos(vertIn.vertex);
				output.posScreen = ComputeScreenPos(output.pos);
                output.uv = vertIn.texcoord0;
				
				return(output);
			}
			
			// Fragent shader
			float4 frag(Vert2Frag fragIn) : SV_Target
			{
				float4 uvScreen = UNITY_PROJ_COORD(fragIn.posScreen);
				float2 normal = float2(0,0);
				normal.x = ((fragIn.uv.x-0.5)*2);
				normal.y = ((fragIn.uv.y-0.5)*2);
				
				float strength = ( (1-abs(normal.x)) * (1-abs(normal.y)) );

				normal *= strength;
				float2 normalTex = tex2D( _NormalTexture, fragIn.uv+_UVOffset.zw ).xy;
				float normalTexStrength = ((1-_NormalTexFrameless) + _NormalTexFrameless*strength) * _NormalTexStrength;
				normal *= _DistortionStrength;
				normal.x += ((normalTex.x-.5)*2) * normalTexStrength * strength;
				normal.y += ((normalTex.y-.5)*2) * normalTexStrength * strength;

				float4 final = tex2Dproj( _RenderTexture, uvScreen+float4(normal.x,normal.y,0,0) );
				final = float4(final.xyz,(1-_StrengthAlpha)+(strength*_StrengthAlpha));
                return final+(_MainColor*strength);
			}
			ENDCG
		}
	} 
}