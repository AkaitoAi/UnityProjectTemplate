Shader "Akaito Ai/Mobile/Blend/TextureBlendColorBumped"
{
    Properties
    {
        // Texture 1
        _MainTex ("Texture 1", 2D) = "white" {}
        _MainColor1 ("Main Color 1", Color) = (1, 1, 1, 1)
        _NormalMap1 ("Normal Map 1", 2D) = "bump" {}
        _MainTex_ST ("Texture 1 Tiling & Offset", Vector) = (1, 1, 0, 0) // Tiling and Offset for Texture 1

        // Texture 2
        _SecondTex ("Texture 2", 2D) = "white" {}
        _MainColor2 ("Main Color 2", Color) = (1, 1, 1, 1)
        _NormalMap2 ("Normal Map 2", 2D) = "bump" {}
        _SecondTex_ST ("Texture 2 Tiling & Offset", Vector) = (1, 1, 0, 0) // Tiling and Offset for Texture 2

        // Blending control
        _Blend ("Blend Factor", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _SecondTex;

            fixed4 _MainColor1;
            fixed4 _MainColor2;

            sampler2D _NormalMap1; // Normal map for texture 1
            sampler2D _NormalMap2; // Normal map for texture 2

            float _Blend;

            float4 _MainTex_ST;   // Tiling & Offset for Texture 1
            float4 _SecondTex_ST; // Tiling & Offset for Texture 2

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT; // Tangent for normal mapping
            };

            struct v2f
            {
                float2 uv_MainTex : TEXCOORD0;
                float2 uv_SecondTex : TEXCOORD1;
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD2;
                float3 tangentToWorld[3] : TEXCOORD3; // Tangent space basis
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv_MainTex = TRANSFORM_TEX(v.uv, _MainTex);   // Apply tiling and offset for Texture 1
                o.uv_SecondTex = TRANSFORM_TEX(v.uv, _SecondTex); // Apply tiling and offset for Texture 2
                o.worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal)); // Transform normal to world space

                // Calculate tangent-to-world matrix for normal mapping
                float3 worldTangent = normalize(mul((float3x3)unity_ObjectToWorld, v.tangent.xyz));
                float3 worldBinormal = cross(o.worldNormal, worldTangent) * v.tangent.w;
                o.tangentToWorld[0] = worldTangent;
                o.tangentToWorld[1] = worldBinormal;
                o.tangentToWorld[2] = o.worldNormal;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Sample both textures and apply their respective colors
                fixed4 col1 = tex2D(_MainTex, i.uv_MainTex) * _MainColor1; // Multiply by color property
                fixed4 col2 = tex2D(_SecondTex, i.uv_SecondTex) * _MainColor2; // Multiply by color property

                // Blend between the two main colors/textures
                fixed4 baseColor = lerp(col1, col2, _Blend);

                // Sample normal maps and transform them to world space
                fixed3 tangentNormal1 = tex2D(_NormalMap1, i.uv_MainTex).xyz * 2.0 - 1.0; // Unpack normal map
                fixed3 tangentNormal2 = tex2D(_NormalMap2, i.uv_SecondTex).xyz * 2.0 - 1.0; // Unpack normal map

                // Transform the normals to world space
                fixed3 worldNormal1 = normalize(mul(float3x3(i.tangentToWorld[0], i.tangentToWorld[1], i.tangentToWorld[2]), tangentNormal1));
                fixed3 worldNormal2 = normalize(mul(float3x3(i.tangentToWorld[0], i.tangentToWorld[1], i.tangentToWorld[2]), tangentNormal2));

                // Blend the normals
                fixed3 blendedNormal = normalize(lerp(worldNormal1, worldNormal2, _Blend));

                // Final output: blended color
                return baseColor; // Return the final blended color
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}