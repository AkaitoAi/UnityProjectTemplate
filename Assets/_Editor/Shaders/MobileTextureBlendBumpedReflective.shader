Shader "Akaito Ai/Mobile/Blend/TextureBlendBumpedReflective"
{
    Properties
    {
        // Texture 1
        _MainTex ("Texture 1", 2D) = "white" {}
        _MainColor1 ("Main Color 1", Color) = (1,1,1,1)
        _ReflectionColor1 ("Reflection Color 1", Color) = (1,1,1,1)
        _Cubemap1 ("Cubemap 1", Cube) = "" {}
        _NormalMap1 ("Normal Map 1", 2D) = "bump" {} // Normal map for texture 1
        _MainTex_ST ("Texture 1 Tiling & Offset", Vector) = (1,1,0,0) // Tiling and Offset for Texture 1

        // Texture 2
        _SecondTex ("Texture 2", 2D) = "white" {}
        _MainColor2 ("Main Color 2", Color) = (1,1,1,1)
        _ReflectionColor2 ("Reflection Color 2", Color) = (1,1,1,1)
        _Cubemap2 ("Cubemap 2", Cube) = "" {}
        _NormalMap2 ("Normal Map 2", 2D) = "bump" {} // Normal map for texture 2
        _SecondTex_ST ("Texture 2 Tiling & Offset", Vector) = (1,1,0,0) // Tiling and Offset for Texture 2

        // Blending control
        _Blend ("Blend Factor", Range(0,1)) = 0.5
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

            sampler2D _MainTex;
            sampler2D _SecondTex;

            fixed4 _MainColor1;
            fixed4 _MainColor2;

            fixed4 _ReflectionColor1;
            fixed4 _ReflectionColor2;

            samplerCUBE _Cubemap1;
            samplerCUBE _Cubemap2;

            sampler2D _NormalMap1; // Normal map sampler for texture 1
            sampler2D _NormalMap2; // Normal map sampler for texture 2

            float _Blend;

            float4 _MainTex_ST;   // Tiling & Offset for Texture 1
            float4 _SecondTex_ST; // Tiling & Offset for Texture 2

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float4 tangent : TANGENT; // Tangent for normal mapping
            };

            struct v2f
            {
                float2 uv_MainTex : TEXCOORD0;
                float2 uv_SecondTex : TEXCOORD1;
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD2;
                float3 worldNormal : TEXCOORD3;
                float3 tangentToWorld[3] : TEXCOORD4; // To transform normals from tangent space to world space
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv_MainTex = TRANSFORM_TEX(v.uv, _MainTex);   // Apply tiling and offset for Texture 1
                o.uv_SecondTex = TRANSFORM_TEX(v.uv, _SecondTex); // Apply tiling and offset for Texture 2
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                // Calculate tangent-to-world matrix for normal mapping
                float3 worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                float3 worldTangent = normalize(mul((float3x3)unity_ObjectToWorld, v.tangent.xyz));
                float3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;
                o.tangentToWorld[0] = worldTangent;
                o.tangentToWorld[1] = worldBinormal;
                o.tangentToWorld[2] = worldNormal;

                return o;
            }

            fixed3 UnpackNormal(sampler2D normalMap, float2 uv, float3x3 tangentToWorld)
            {
                // Sample the normal map in tangent space and unpack the normal
                fixed3 tangentNormal = tex2D(normalMap, uv).xyz * 2 - 1;
                // Transform the normal to world space
                return normalize(mul(tangentToWorld, tangentNormal));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample both textures and apply their respective colors with tiling and offset
                fixed4 col1 = tex2D(_MainTex, i.uv_MainTex) * _MainColor1;
                fixed4 col2 = tex2D(_SecondTex, i.uv_SecondTex) * _MainColor2;

                // Blend between the two main colors/textures
                fixed4 baseColor = lerp(col1, col2, _Blend);

                // Sample normal maps and transform them to world space
                float3 normal1 = UnpackNormal(_NormalMap1, i.uv_MainTex, float3x3(i.tangentToWorld[0], i.tangentToWorld[1], i.tangentToWorld[2]));
                float3 normal2 = UnpackNormal(_NormalMap2, i.uv_SecondTex, float3x3(i.tangentToWorld[0], i.tangentToWorld[1], i.tangentToWorld[2]));

                // Blend the normals
                float3 blendedNormal = normalize(lerp(normal1, normal2, _Blend));

                // Sample reflection from cubemaps using the blended normal
                float3 reflectDir = reflect(i.worldPos - _WorldSpaceCameraPos, blendedNormal);
                fixed4 reflectColor1 = texCUBE(_Cubemap1, reflectDir) * _ReflectionColor1;
                fixed4 reflectColor2 = texCUBE(_Cubemap2, reflectDir) * _ReflectionColor2;

                // Blend the reflection color
                fixed4 reflectionColor = lerp(reflectColor1, reflectColor2, _Blend);

                // Final output: blended main color + blended reflection
                fixed4 finalColor = baseColor + reflectionColor;

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}