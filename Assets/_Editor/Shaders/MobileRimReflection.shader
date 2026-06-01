Shader "Akaito Ai/Mobile/Reflection/Rim Reflection"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Cube ("Reflection", CUBE) = "" { }
		_Power ("Rim Power", Float) = 3
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed4 rim : COLOR;
                float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
				UNITY_FOG_COORDS(1)
				float4 pos : SV_POSITION;
				float4 posWorld : TEXCOORD1;
				float3 I : TEXCOORD2;
				float4 rim : COLOR;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			uniform samplerCUBE _Cube;
			float _Power;

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.normal = normalize( mul ( float4(v.normal, 0.0), unity_WorldToObject).xyz);
				o.rim = v.rim;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				float3 viewDir = WorldSpaceViewDir( v.vertex );
				float3 worldN = UnityObjectToWorldNormal( v.normal );
				o.I = reflect( -viewDir, worldN );

				UNITY_TRANSFER_FOG(o,o.vertex);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//rim lighting
				float3 normalDir = i.normal;
				float3 viewDir = normalize( _WorldSpaceCameraPos.xyz - i.posWorld.xyz);
				float rim = 1 - saturate ( dot(viewDir, normalDir) );
				fixed rimLight = pow(rim, _Power);
				// sample the textures
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 reflcol = texCUBE( _Cube, i.I );

				col = lerp(col, reflcol, (rimLight * reflcol + reflcol*0.5f)/1.5f);

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}