// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/*
 * By Martin Reintges 05/2016
 */

Shader "DistortionShaderPack/Legacy/RTDistortionMasterTexture"
{
	Properties
	{
		_MainColor("MainColor", Color) = (0,0,0,1)
		_RenderTexture ("RenderTexture", 2D) = "black"
		_MainTexture ("Texture", 2D) = "black" {}
		_NormalTexture ("Normal", 2D) = "blue" {}
		_TextureStrength ("TextureStrength", Range(0,1)) = 0
		_TextureDistortion ("TextureDistortion", Range(0,1)) = 0
		_NormalTexStrength ("_NormalTexStrength", Range(0,1)) = 0.5
		_NormalTexFrameless ("_NormalTexFrameless", Range(0,1)) = 0.5
		_UVOffset ("UVOffset XY, generated ZW", Vector) = (0,0,0,0)
	}
	SubShader 
	{
		// Subshader Tags
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		
		// Render Pass
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#include "DistortionCG.cginc"
			#pragma target 2.0
			
			#pragma fragmentoption ARB_precision_hint_fastest
			
			#pragma vertex vert
			#pragma fragment frag
	
	
		////// Uniform user variable definition
			uniform sampler2D _RenderTexture;
			uniform sampler2D _MainTexture;
			uniform float4 _MainTexture_ST;
			uniform sampler2D _NormalTexture;
			uniform float4 _NormalTexture_ST;
			uniform float _TextureStrength;
			uniform float _TextureDistortion;
			uniform float _NormalTexStrength;
			uniform float _NormalTexFrameless;
			uniform float4 _MainColor;
			uniform float4 _UVOffset;

	
		////// Input structs
	
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
				float2 uvTex = fragIn.uv;
				float4 uvScreen = getScreenUV(fragIn.posScreen);

				float2 normal = getNormal(_NormalTexture, fragIn.uv, _UVOffset.zw, _NormalTexFrameless, _NormalTexStrength);
				uvTex += normal * _TextureDistortion;
				uvScreen += float4(normal.x, normal.y, 0, 0);

				float4 mainTex = tex2D( _MainTexture, uvTex );
				mainTex.w *= _TextureStrength;
				float4 screen = tex2Dproj( _RenderTexture, uvScreen );
				float4 final = (1-mainTex.w)*screen + mainTex * mainTex.w;

				return final+_MainColor*mainTex.w;
			}
			ENDCG
		}
	} 
}