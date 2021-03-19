Shader "Custom/FlattenTerrain" {
	Properties{
		_Tess("Tessellation", Range(1,32)) = 4 
		_EdgeLength("Edge length", Range(1,50)) = 15
		_MainTex("Base (RGB)", 2D) = "white" {}
		_DispTex("Disp Texture", 2D) = "gray" {}
		_Displacement("Displacement", Range(0, 10.0)) = 0.3
		_Color("Color", color) = (1,1,1,0)
		_SpecColor("Spec color", color) = (0.5,0.5,0.5,0.5)
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 300

		CGPROGRAM
#pragma surface surf BlinnPhong addshadow fullforwardshadows vertex:disp tessellate:tessFixed nolightmap
#pragma target 5.0
#include "Tessellation.cginc"

	struct appdata {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float2 texcoord : TEXCOORD0;
	};

	float _EdgeLength;

	float4 tessEdge(appdata v0, appdata v1, appdata v2)
	{
		return UnityEdgeLengthBasedTess(v0.vertex, v1.vertex, v2.vertex, _EdgeLength);
	}

	float _Tess;

	float4 tessFixed()
	{
		return _Tess;
	}

	sampler2D _DispTex;
	float _Displacement;

	void disp(inout appdata v)
	{
		float d = (1 - tex2Dlod(_DispTex, float4(v.texcoord.xy,0,0)).r) * _Displacement;
		v.vertex.y += _Displacement - d;
		// v.normal *= -1;
	}



	struct Input {
		float2 uv_MainTex;
	};

	sampler2D _MainTex;
	fixed4 _Color; 

	void surf(Input IN, inout SurfaceOutput o) {
		half4 c = tex2D(_DispTex, IN.uv_MainTex) * _Color;
		if (c.r == 0) clip(-1);
		c.a = 0;
		o.Albedo = c;
		o.Specular = 0.2;
		o.Gloss = 1.0;
	}
	ENDCG
	}
		FallBack "Diffuse"
}