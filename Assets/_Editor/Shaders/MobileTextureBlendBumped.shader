Shader "Akaito Ai/Mobile/Blend/TextureBlendBumped"
{
    Properties
    {
        // Texture 1 properties
        _MainTex ("Texture 1", 2D) = "white" {}
        _NormalMap1 ("Normal Map 1", 2D) = "bump" {} 

        // Texture 2 properties
        _SecondTex ("Texture 2", 2D) = "white" {}
        _NormalMap2 ("Normal Map 2", 2D) = "bump" {}

        // Blending control
        _Blend ("Blend Factor", Range(0,1)) = 0.5

        // Tiling and Offset
        _MainTex_ST ("Main Texture Tiling & Offset", Vector) = (1, 1, 0, 0)
        _SecondTex_ST ("Second Texture Tiling & Offset", Vector) = (1, 1, 0, 0)
        _NormalMap1_ST ("Normal Map 1 Tiling & Offset", Vector) = (1, 1, 0, 0)
        _NormalMap2_ST ("Normal Map 2 Tiling & Offset", Vector) = (1, 1, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Texture samplers
            sampler2D _MainTex;
            sampler2D _SecondTex;
            sampler2D _NormalMap1;
            sampler2D _NormalMap2;

            // Tiling and Offset properties
            float4 _MainTex_ST;
            float4 _SecondTex_ST;
            float4 _NormalMap1_ST;
            float4 _NormalMap2_ST;

            // Blend factor
            float _Blend;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 tangent : TANGENT; // Tangent for normal mapping
            };

            struct v2f
            {
                float2 uv_MainTex : TEXCOORD0;
                float2 uv_SecondTex : TEXCOORD1;
                float2 uv_NormalMap1 : TEXCOORD2;
                float2 uv_NormalMap2 : TEXCOORD3;
                float4 pos : SV_POSITION;
                float3 tangentToWorld[3] : TEXCOORD4; // Matrix to transform normals from tangent to world space
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                // Apply tiling and offset to UVs
                o.uv_MainTex = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv_SecondTex = TRANSFORM_TEX(v.uv, _SecondTex);
                o.uv_NormalMap1 = TRANSFORM_TEX(v.uv, _NormalMap1);
                o.uv_NormalMap2 = TRANSFORM_TEX(v.uv, _NormalMap2);

                // Calculate tangent-to-world matrix for normal mapping
                float3 worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.vertex.xyz));
                float3 worldTangent = normalize(mul((float3x3)unity_ObjectToWorld, v.tangent.xyz));
                float3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;

                o.tangentToWorld[0] = worldTangent;
                o.tangentToWorld[1] = worldBinormal;
                o.tangentToWorld[2] = worldNormal;

                return o;
            }

            fixed3 UnpackNormal(sampler2D normalMap, float2 uv, float3x3 tangentToWorld)
            {
                // Sample and unpack normal map
                fixed3 tangentNormal = tex2D(normalMap, uv).xyz * 2 - 1;
                return normalize(mul(tangentToWorld, tangentNormal)); // Transform normal to world space
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample both base textures with their respective UVs
                fixed4 col1 = tex2D(_MainTex, i.uv_MainTex);
                fixed4 col2 = tex2D(_SecondTex, i.uv_SecondTex);

                // Blend the two textures
                fixed4 finalColor = lerp(col1, col2, _Blend);

                // Sample and blend normal maps
                float3 normal1 = UnpackNormal(_NormalMap1, i.uv_NormalMap1, float3x3(i.tangentToWorld[0], i.tangentToWorld[1], i.tangentToWorld[2]));
                float3 normal2 = UnpackNormal(_NormalMap2, i.uv_NormalMap2, float3x3(i.tangentToWorld[0], i.tangentToWorld[1], i.tangentToWorld[2]));

                // Blend normals
                float3 blendedNormal = normalize(lerp(normal1, normal2, _Blend));

                // Apply lighting or other effects here using blendedNormal if necessary

                return finalColor; // Return the final blended color
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
