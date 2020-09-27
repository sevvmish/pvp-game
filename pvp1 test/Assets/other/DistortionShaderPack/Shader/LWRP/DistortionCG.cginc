
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

			struct VertexInputLit
			{
				float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct Vert2FragLit
			{
				float4 pos : SV_POSITION;
				float4 posScreen : TEXCOORD0;
                float2 uv : TEXCOORD1;
				float3 normal : TEXCOORD2;
				float3 lightDirection : TEXCOORD3;
			};

////// Distortion helper functions
			float getDistortionStrength()
			{
				return 1;
			}

			float4 getScreenUV(float4 posScreen)
			{
				return UNITY_PROJ_COORD(posScreen);
			}

			float2 getSinglePassVR(float2 uvScreen, float4 leftOffset, float4 rightOffset)
			{
				float2 uv;
				if(uvScreen.x < 0.5)
				{
					uv = float2(uvScreen.x*2, uvScreen.y);
					uv.x = leftOffset.x + uv.x * leftOffset.z;
					uv.y = leftOffset.y + uv.y * leftOffset.w;
				}
				else
				{
					uv = float2((uvScreen.x-0.5)*2, uvScreen.y);
					uv.x = rightOffset.x + uv.x * rightOffset.z;
					uv.y = rightOffset.y + uv.y * rightOffset.w;
				}

				return uv;
				/*
				float screenCenter = 1 / uvScreen.w;
				float x = uvScreen.x;
				if( x < screenCenter )
				{
					float frac = (screenCenter / (x)) ;
					x = frac;
				}
				else
				{
					float frac = screenCenter / (x - screenCenter);
					x = frac;
				}

				uvScreen.x = x;
				return uvScreen;*/
			}

			float4 getDistortedScreenUV(float4 screenUV, float2 distortion)
			{
				return screenUV + float4(0, 0, distortion.x, distortion.y);
			}

			float2 getVectorFromCenter(float2 uv)
			{
				float2 direction = uv - float2(0.5, 0.5);
				return (direction);
			}

			float getDistortionStrength(float2 uv, float distortionSize)
			{
				float2 diff = float2(distance(0.5, uv.x), distance(0.5, uv.y)) * 2.0;
				float dist = saturate(length(diff));
				return 1.0-dist;
			}

			float2 getNormal(sampler2D _NormalTexture, float2 uv, float2 uvOffset, float frameless, float strength)
			{
				float2 normal = tex2D( _NormalTexture, uv+uvOffset ).xy;
				float length = getDistortionStrength(uv, 1);

				float normalTexStrength = ((1-frameless) + frameless*length) * strength;
				normal.x = ((normal.x-.5)*2) * normalTexStrength;
				normal.y = ((normal.y-.5)*2) * normalTexStrength;

				return normal;
			}

			float getBlend(float4 posScreen, sampler2D depthTexture)
			{
				float depth = tex2Dproj(depthTexture, posScreen);
				depth = 1.0 / (_ZBufferParams.z * depth + _ZBufferParams.w);
				return (depth - posScreen.w);
			}