// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/*
 * By Martin Reintges 05/2016
 */

Shader "DistortionShaderPack/RTDistortionCircleHard"
{
	Properties
	{
		_RenderTexture ("RenderTexture", 2D) = "black" {}
		_DistortionStrength ("DistortionStrength", Range(-1,1)) = -1
		_DistortionSize ("DistortionSize", Range(0,0.8)) = 0.8
	}
	SubShader 
	{
		// Subshader Tags
		Tags { "RenderType"="Opaque" }
		
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
			uniform float _DistortionStrength;
			uniform float _DistortionSize;

	
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
				normal.x = sin((fragIn.uv.x-0.5)*2);
				normal.y = sin((fragIn.uv.y-0.5)*2);
				
				float dist = min(length(normal), _DistortionSize);
				if(dist < _DistortionSize)
				{
					dist = 1;
				}
				else
				 {
				 dist = 0;
				 }
				
				normal *= _DistortionStrength * dist;

				float4 final = tex2Dproj( _RenderTexture, uvScreen+float4(normal.x*0.5,normal.y,0,0) );
                return final;
			}
			ENDCG
		}
	} 
}