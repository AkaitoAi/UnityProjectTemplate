Shader "Akaito Ai/Mobile/Blend/TextureBlendReflective"
{
    Properties
    {
        // Texture 1
        _MainTex ("Texture 1", 2D) = "white" {}
        _MainColor1 ("Main Color 1", Color) = (1,1,1,1)
        _ReflectionColor1 ("Reflection Color 1", Color) = (1,1,1,1)
        _Cubemap1 ("Cubemap 1", Cube) = "" {}

        // Texture 2
        _SecondTex ("Texture 2", 2D) = "white" {}
        _MainColor2 ("Main Color 2", Color) = (1,1,1,1)
        _ReflectionColor2 ("Reflection Color 2", Color) = (1,1,1,1)
        _Cubemap2 ("Cubemap 2", Cube) = "" {}

        // Blending control
        _Blend ("Blend Factor", Range(0,1)) = 0.5

        // Tiling and Offset for Textures
        _MainTex_ST ("Main Texture Tiling & Offset", Vector) = (1, 1, 0, 0)
        _SecondTex_ST ("Second Texture Tiling & Offset", Vector) = (1, 1, 0, 0)
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

            float _Blend;

            // Tiling and Offset properties
            float4 _MainTex_ST;
            float4 _SecondTex_ST;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv_MainTex : TEXCOORD0;
                float2 uv_SecondTex : TEXCOORD1;
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD2;
                float3 worldNormal : TEXCOORD3;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                // Apply tiling and offset to UVs for both textures
                o.uv_MainTex = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv_SecondTex = TRANSFORM_TEX(v.uv, _SecondTex);

                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample both textures with the transformed UVs
                fixed4 col1 = tex2D(_MainTex, i.uv_MainTex) * _MainColor1;
                fixed4 col2 = tex2D(_SecondTex, i.uv_SecondTex) * _MainColor2;

                // Blend between the two main colors/textures
                fixed4 baseColor = lerp(col1, col2, _Blend);

                // Sample reflection from cubemaps
                float3 reflectDir = reflect(i.worldPos - _WorldSpaceCameraPos, i.worldNormal);
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
