// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/*
 * By Martin Reintges 05/2016
 */

Shader "DistortionShaderPack/Legacy/RTDistortionMasterHDR"
{
	Properties
	{
		_MainColor("MainColor", Color) = (0,0,0,1)
		_RenderTexture ("RenderTexture", 2D) = "black"
		_NormalTexture ("Normal", 2D) = "blue" {}
		_DistortionStrength ("DistortionStrength", Range(-2,2)) = 0.1
		_DistortionCircle ("DistortionCircle", Range(0,1)) = 0
		_NormalTexStrength ("_NormalTexStrength", Range(0,1)) = 0.5
		_NormalTexFrameless ("_NormalTexFrameless", Range(0,1)) = 0.5
		_StrengthAlpha ("AlphaStrength", Range(0,1)) = 0.5
		_UVOffset ("UVOffset XY, generated ZW", Vector) = (0,0,0,0)
	}
	SubShader 
	{
		// Subshader Tags
		Tags { "Queue"="Transparent+1" "RenderType"="Transparent" }
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
			uniform sampler2D _NormalTexture;
			uniform float4 _NormalTexture_ST;
			uniform float _DistortionStrength;
			uniform float _DistortionCircle;
			uniform float _NormalTexStrength;
			uniform float _NormalTexFrameless;
			uniform float4 _MainColor;
			uniform float4 _UVOffset;
			uniform float _StrengthAlpha;

	
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
				float4 uvScreen = getScreenUV(fragIn.posScreen);
				float2 direction = getVectorFromCenter(fragIn.uv);
				float strength = getDistortionStrength(fragIn.uv, 1);
				strength = _DistortionCircle*strength + (1-_DistortionCircle);

				direction *= strength * _DistortionStrength;
				uvScreen += float4(direction.x, direction.y, 0, 0);

				float2 normal = getNormal(_NormalTexture, fragIn.uv, _UVOffset.zw, _NormalTexFrameless, _NormalTexStrength);
				uvScreen += float4(0, 0, normal.x, normal.y);

				float4 final = tex2Dproj( _RenderTexture, uvScreen );
				final = float4(final.xyz,(1-_StrengthAlpha)+(strength*_StrengthAlpha));
                final += (_MainColor*strength);
				final.w = saturate( strength * 1000);
				return final;
			}
			ENDCG
		}
	} 
}