// Upgrade NOTE: commented out 'float4x4 _CameraToWorld', a built-in variable
// Upgrade NOTE: replaced '_CameraToWorld' with 'unity_CameraToWorld'
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// http://www.popekim.com/2012/10/siggraph-2012-screen-space-decals-in.html

Shader "Decal/DecalShader"
{
	Properties
	{
		_MainTex ("Diffuse", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			Fog { Mode Off } // no fog in g-buffers pass
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma exclude_renderers nomrt
			
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
				float4 screenUV : TEXCOORD1;
				float3 ray : TEXCOORD2;
				half3 orientation : TEXCOORD3;
			};

			struct COL_OUTPUT {
				fixed4 Col0 : SV_TARGET0;
				fixed4 Mask : SV_TARGET1;
			};

			v2f vert (float4 v : POSITION)
			{
				v2f o;

				o.pos = UnityObjectToClipPos (v);
				// 使ってない
				o.uv = v.xz+0.5;
				o.screenUV = ComputeScreenPos (o.pos);
				// View空間で(x,y)を反転
				o.ray = UnityObjectToViewPos(v) * float3(-1,-1,1);
				//o.ray = UnityObjectToViewPos(v);
				// Objectにとっての上(法線の向き)がどっちかを計算
				o.orientation = mul ((float3x3)unity_ObjectToWorld, float3(0,1,0));
				return o;
			}

			sampler2D _MainTex;
			sampler2D_float _CameraDepthTexture;
			sampler2D _NormalsCopy;



			//void frag(
			//	v2f i,
			//	out half4 outDiffuse : COLOR0,			// RT0: diffuse color (rgb), --unused-- (a)
			//	out half4 outSpecRoughness : COLOR1,	// RT1: spec color (rgb), roughness (a)
			//	out half4 outNormal : COLOR2,			// RT2: normal (rgb), --unused-- (a)
			//	out half4 outEmission : COLOR3			// RT3: emission (rgb), --unused-- (a)
			//)
			COL_OUTPUT frag(v2f i) : SV_Target
			{
				// (カメラのFar Plane / テクセルのビュー空間での場所)
				// Far Plane上での場所に変換？
				i.ray = i.ray * (_ProjectionParams.z / i.ray.z);
				// 純粋なスクリーン座標に変換
				float2 uv = i.screenUV.xy / i.screenUV.w;
				// read depth and reconstruct world position
				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
				// 高精度の距離に変換
				depth = Linear01Depth (depth);
				// depthはFar Planeを1としているため、これでビュー空間上の座標が得られる
				float4 vpos = float4(i.ray * depth,1);
				// ビューからワールドへ
				float3 wpos = mul (unity_CameraToWorld, vpos).xyz;
				float3 opos = mul (unity_WorldToObject, float4(wpos,1)).xyz;

				clip (float3(0.5,0.5,0.5) - abs(opos.xyz));
				//clip (float2(0.5,0.5) - abs(opos.xy));

				// clipと組み合わせると 0<=xy<=1の範囲のみ描画する
				i.uv = opos.xy + 0.5;

				//_NormalsCopyはDeferredDecalRenderer.csから来ている
				//スクリーン上でうつっている位置からそこの法線を取り出す
				half3 normal = tex2D(_NormalsCopy, uv).rgb;
				//値域を[0,1]に変換
				fixed3 wnormal = normal.rgb * 2.0 - 1.0;
				//法線の向きが cos(theta) = 0.3より小さければカット				
				// TODO: xzからxyに切り替えた点を反映する
				//clip (dot(wnormal, i.orientation) - 0.3);

				COL_OUTPUT col_out;
				col_out.Col0 = tex2D(_MainTex, i.uv);
				col_out.Mask = fixed4(0.0, 0.0, 1.0, 1.0);

				return col_out;
			}
			ENDCG
		}		

	}

	Fallback Off
}
