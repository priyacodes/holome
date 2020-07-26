Shader "Unlit/HoloMe"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", COLOR) = (1, 1, 1, 1)
		_Offset("Offset", FLOAT) = 0.5
		_t("_t", Range(0,1)) = 1
		[Toggle(USE_AMBIENT_LIGHTING)] _UseAmbientLighting("Use ambient lighting", Float) = 0
	}
		SubShader
		{
			Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "LightMode" = "ForwardBase" }
			LOD 100
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma shader_feature USE_AMBIENT_LIGHTING
				#include "UnityCG.cginc"
				#include "UnityLightingCommon.cginc"
				struct v2f { float2 uv:TEXCOORD0; float4 vertex:SV_POSITION; fixed4 diff : COLOR0; };
				sampler2D _MainTex;
				float4 _MainTex_ST,_Color;
				float _ThresholdMask,_Offset,_AlphaFactor;
				float _t;

				 v2f vert(appdata_base v)
				 {
				   v2f f;
				   f.vertex = UnityObjectToClipPos(v.vertex);
				   f.uv = v.texcoord;
				   half3 z = UnityObjectToWorldNormal(v.normal);
				   half w = max(0,dot(z,_WorldSpaceLightPos0.xyz));
				   f.diff = _LightColor0;
				   return f;
				 }
				 fixed4 frag(v2f v) :SV_Target
				 {
				   fixed4 f = tex2D(_MainTex,float2(v.uv.x * .5,v.uv.y)),z = tex2D(_MainTex,float2(v.uv.x * .5 + _Offset,v.uv.y));
				   float w = f.z,x = f.y,y = f.x;
				   f.w = z.z;
				   f.xyz = float3(w,x,y);
				   f.w = z.y;
				   if (f.w < _ThresholdMask)
					 {
				   discard;
				   }
#ifdef USE_AMBIENT_LIGHTING
				   v.diff.xyz = clamp(v.diff.xyz, float3(.25, .25, .25), float3(1.75, 1.75, 1.75));
				   f.xyz *= v.diff;
#endif
				   f.a *= _t;
				   return f * _Color;
				 }
				 ENDCG
			}
		}
}