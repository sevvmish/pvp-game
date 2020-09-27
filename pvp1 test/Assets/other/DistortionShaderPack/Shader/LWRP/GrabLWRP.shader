// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
// https://forum.unity.com/threads/the-scriptable-render-pipeline-how-to-support-grabpass.521473/

Shader "DistortionShaderPack/GrabLWRP" 
{

	Properties 
	{	
		_MainColor("MainColor", Color) = (0,0,0,1)
	}

	Category 
	{
	
        Tags{"RenderType" = "Opaque" "RenderPipeline" = "LightweightPipeline" "IgnoreProjector" = "True"}
		ZWrite Off

		SubShader 
		{
			Pass 
			{
				Name "BASE"
				Tags{"LightMode" = "LightweightForward"}
			
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				//#pragma multi_compile_fog
				#include "DistortionCG.cginc"
				
				uniform sampler2D _CameraOpaqueTexture;
				uniform float4 _CameraOpaqueTexture_TexelSize;
				
				uniform float4 _MainColor;

				struct Vert2FragGrab
				{
					float4 position : SV_POSITION;
					float2 uv : TEXCOORD0;
					float4 uv_grab : TEXCOORD1;
				};


				Vert2FragGrab vert (VertexInput vertIn)
				{
					Vert2FragGrab output;
					output.position = UnityObjectToClipPos(vertIn.vertex);
#if UNITY_UV_STARTS_AT_TOP
					float scale = -1.0;
#else
					float scale = 1.0;
#endif
					output.uv = vertIn.texcoord0;
					output.uv_grab.xy = (float2(output.position.x, output.position.y*scale) + output.position.w) * 0.5;
					output.uv_grab.zw = output.position.zw;
					return output;
				}


				half4 frag (Vert2FragGrab fragIn) : SV_Target
				{
				float4 uvScreen = getScreenUV(fragIn.uv_grab);

				float4 final = tex2Dproj( _CameraOpaqueTexture, uvScreen );
                return final*(_MainColor);


				}
				ENDCG
			}
		}
	}
}
