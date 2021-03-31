Shader "Custom/Mobile/Bloom"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "" {}
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	struct v2f {
		half4 pos : POSITION;
		half2 uv  : TEXCOORD0;
	};

	struct v2fb
	{
		half4 pos  : SV_POSITION;
		half2  uv  : TEXCOORD0;
		half4  uv1 : TEXCOORD1;
		half4  uv2 : TEXCOORD2;
	};

	sampler2D _MainTex;
	sampler2D _BloomTex;
	uniform half4 _MainTex_TexelSize;
	uniform half _BloomAmount;
	uniform half _BlurAmount;
	uniform half _FadeAmount;
	v2fb vertb(appdata_img v)
	{
		v2fb o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		o.uv1.xy = v.texcoord.xy + _MainTex_TexelSize.xy * _BlurAmount;
		o.uv1.zw = v.texcoord.xy + half2(-1.0h, 1.0h) * _MainTex_TexelSize.xy * _BlurAmount;
		o.uv2.xy = v.texcoord.xy - _MainTex_TexelSize.xy * _BlurAmount;
		o.uv2.zw = v.texcoord.xy + half2(1.0h, -1.0h) * _MainTex_TexelSize.xy * _BlurAmount;
		return o;
	}
	v2f vert(appdata_img v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	}
	fixed4 fragMain(v2fb i) : COLOR
	{
		fixed4 result = tex2D(_MainTex, i.uv);
		result += tex2D(_MainTex, i.uv1.xy);
		result += tex2D(_MainTex, i.uv1.zw);
		result += tex2D(_MainTex, i.uv2.xy);
		result += tex2D(_MainTex, i.uv2.zw);
		return max(result*0.2h-_FadeAmount,0.0h);
	}
	fixed4 fragBloom(v2fb i) : COLOR
	{
		fixed4 result = tex2D(_MainTex, i.uv);
		result += tex2D(_MainTex, i.uv1.xy);
		result += tex2D(_MainTex, i.uv1.zw);
		result += tex2D(_MainTex, i.uv2.xy);
		result += tex2D(_MainTex, i.uv2.zw);
		return result*0.2h;
	}

	fixed4 frag(v2f i) : COLOR
	{
		fixed4 c = tex2D(_MainTex, i.uv);
		fixed4 b = tex2D(_BloomTex, i.uv);
		return c+b*_BloomAmount;
	}
	ENDCG 
		
	Subshader 
	{
		Pass
		{
		  CGPROGRAM
		  #pragma vertex vertb
		  #pragma fragment fragMain
		  #pragma fragmentoption ARB_precision_hint_fastest
		  ENDCG
		}
		Pass
		{
		  CGPROGRAM
		  #pragma vertex vertb
		  #pragma fragment fragBloom
		  #pragma fragmentoption ARB_precision_hint_fastest
		  ENDCG
		}
		Pass
		{
		  CGPROGRAM
		  #pragma vertex vert
		  #pragma fragment frag
		  #pragma fragmentoption ARB_precision_hint_fastest
		  ENDCG
		}
	}
	Fallback off
	}