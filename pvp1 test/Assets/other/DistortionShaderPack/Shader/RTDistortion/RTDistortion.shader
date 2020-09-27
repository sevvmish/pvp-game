// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/*
 * By Martin Reintges 05/2016
 */

Shader "DistortionShaderPack/RTDistortion"
{
	Properties
	{
		_RenderTexture ("RenderTexture", 2D) = "black" {}
		_DistortionStrength ("DistortionStrength", Range(-1,1)) = -1
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
				normal *= _DistortionStrength;				
				normal *= strength;

				float4 final = tex2Dproj( _RenderTexture, uvScreen+float4(normal.x,normal.y,0,0) );
                return final;
			}
			ENDCG
		}
	} 
}