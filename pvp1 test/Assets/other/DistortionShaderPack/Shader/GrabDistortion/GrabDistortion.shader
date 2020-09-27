// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "DistortionShaderPack/GrabDistortion" 
{

	Properties 
	{	
		_MainColor("MainColor", Color) = (0,0,0,1)
		_NormalTexture ("Normal", 2D) = "blue" {}
		_DistortionStrength ("DistortionStrength", Range(-2,2)) = 0.1
		_DistortionCircle ("DistortionCircle", Range(0,1)) = 0
		_NormalTexStrength ("_NormalTexStrength", Range(0,1)) = 0.5
		_NormalTexFrameless ("_NormalTexFrameless", Range(0,1)) = 0.5
		_StrengthAlpha ("AlphaStrength", Range(0,1)) = 0.5
		_UVOffset ("UVOffset XY, generated ZW", Vector) = (0,0,0,0)
	}

	Category 
	{

		// We must be transparent, so other objects are drawn before this one.
		Tags { "Queue"="Transparent+1" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off

		SubShader 
		{

			// This pass grabs the screen behind the object into a texture.
			// We can access the result in the next pass as _GrabTexture
			GrabPass 
			{
				Name "BASE"
				Tags { "LightMode" = "Always" }
			}
		
			// Main pass: Take the texture grabbed above and use the bumpmap to perturb it
			// on to the screen
			Pass 
			{
				Name "BASE"
				Tags { "LightMode" = "Always" }
			
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fog
				#include "UnityCG.cginc"
				#include "DistortionCG.cginc"
				
				uniform sampler2D _GrabTexture;
				uniform float4 _GrabTexture_TexelSize;
				
				uniform sampler2D _NormalTexture;
				uniform float4 _NormalTexture_ST;
				uniform float _DistortionStrength;
				uniform float _DistortionCircle;
				uniform float _NormalTexStrength;
				uniform float _NormalTexFrameless;
				uniform float4 _MainColor;
				uniform float4 _UVOffset;
				uniform float _StrengthAlpha;

				struct Vert2FragGrab
				{
					float4 position : SV_POSITION;
					float4 uv_grab : TEXCOORD0;
					float2 uv_normal : TEXCOORD1;
					float2 uv : TEXCOORD2;
					UNITY_FOG_COORDS(3)
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
					output.uv_normal = TRANSFORM_TEX( vertIn.texcoord0, _NormalTexture );
					UNITY_TRANSFER_FOG(output,output.position);
					return output;
				}


				half4 frag (Vert2FragGrab fragIn) : SV_Target
				{
#if UNITY_SINGLE_PASS_STEREO
					fragIn.uv_grab.xy = TransformStereoScreenSpaceTex(fragIn.uv_grab.xy, fragIn.uv_grab.w);
#endif
					
				float4 uvScreen = getScreenUV(fragIn.uv_grab);
				float2 direction = getVectorFromCenter(fragIn.uv);
				
				float strength = getDistortionStrength(fragIn.uv, 1);
				strength = _DistortionCircle*strength + (1-_DistortionCircle);

				direction *= strength * _DistortionStrength;
				uvScreen += float4(direction.x, direction.y, 0, 0);

				float2 normal = getNormal(_NormalTexture, fragIn.uv, _UVOffset.zw, _NormalTexFrameless, _NormalTexStrength);
				uvScreen += float4(normal.x, normal.y, 0, 0);

				float4 final = tex2Dproj( _GrabTexture, uvScreen );
				final = float4(final.xyz,(1-_StrengthAlpha)+(strength*_StrengthAlpha));
                return final+(_MainColor*strength);


				}
				ENDCG
			}
		}
	}
}
