// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/*
 * By Martin Reintges 05/2016
 */

Shader "Martin/ImageDistortionColor"
{
	Properties
	{
		_MainTex ("MainTexture", 2D) = "white" {}
		_MainColor ("MainColor", Color) = (1,1,1,1)
		_DistortionStrength ("Distortion", Range(0,1)) = 0
		_XTimer ("XTimer", Range(0,1)) = 0
		_YTimer ("YTimer", Range(0,1)) = 0
	}
	SubShader 
	{
		// Subshader Tags
		Blend SrcAlpha OneMinusSrcAlpha 
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
		ZWrite Off

		// Render Pass
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma target 2.0
			#pragma multi_compile_builtin
			#pragma fragmentoption ARB_precision_hint_fastest
			
			#pragma vertex vert
			#pragma fragment frag
	
	
		////// Uniform user variable definition
			// Auto generated reflection texture
			uniform sampler2D _MainTex;
			uniform sampler2D _MainTex_ST;
			uniform float4 _MainColor;
			uniform float _DistortionStrength;
			uniform float _XTimer;
			uniform float _YTimer;

	
		////// Input structs
			struct VertexInput 
			{
				float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
			};
			struct Vert2Frag
			{
				float4 pos : SV_POSITION;
				float4 posScreen : TEXCOORD1;
                float2 uv : TEXCOORD2;
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
				float2 normal = fragIn.uv.xy;
				
				normal.x = ((fragIn.uv.x-0.5)*2);
				normal.y = ((fragIn.uv.y-0.5)*2);
				float strength = (1- abs((fragIn.uv.x-0.5)*2)) * (1- abs((fragIn.uv.y-0.5)*2));
				normal *= strength * _DistortionStrength;
				
				normal.x *= sin(_Time.z*_XTimer);
				normal.y *= sin(_Time.z*_YTimer);

                return tex2D( _MainTex, fragIn.uv+normal.xy ) * _MainColor;
			}
			ENDCG
		}
	} 
}