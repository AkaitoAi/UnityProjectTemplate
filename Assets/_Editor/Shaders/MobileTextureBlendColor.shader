Shader "Akaito Ai/Mobile/Blend/TextureBlendColor"
{
    Properties
    {
        // Texture 1 properties
        _MainTex ("Texture 1", 2D) = "white" {}
        _MainColor1 ("Main Color 1", Color) = (1,1,1,1)

        // Texture 2 properties
        _SecondTex ("Texture 2", 2D) = "white" {}
        _MainColor2 ("Main Color 2", Color) = (1,1,1,1)

        // Blending control
        _Blend ("Blend Factor", Range(0,1)) = 0.5

        // Tiling and Offset
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

            // Texture samplers
            sampler2D _MainTex;
            sampler2D _SecondTex;

            // Main color properties
            fixed4 _MainColor1;
            fixed4 _MainColor2;

            // Blend factor
            float _Blend;

            // Tiling and Offset properties
            float4 _MainTex_ST;
            float4 _SecondTex_ST;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv_MainTex : TEXCOORD0;
                float2 uv_SecondTex : TEXCOORD1;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                // Apply tiling and offset to UVs
                o.uv_MainTex = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv_SecondTex = TRANSFORM_TEX(v.uv, _SecondTex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample both textures with their respective UVs
                fixed4 texColor1 = tex2D(_MainTex, i.uv_MainTex) * _MainColor1;
                fixed4 texColor2 = tex2D(_SecondTex, i.uv_SecondTex) * _MainColor2;

                // Linearly interpolate between the two textures and colors
                fixed4 finalColor = lerp(texColor1, texColor2, _Blend);

                return finalColor;
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
