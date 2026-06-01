Shader "Akaito Ai/Mobile/Blend/TextureBlend"
{
    Properties
    {
        _MainTex ("Texture 1", 2D) = "white" {}
        _SecondTex ("Texture 2", 2D) = "white" {}
        _Blend ("Blend Factor", Range(0,1)) = 0.5

        // Tiling and Offset Properties
        _MainTex_ST ("Texture 1 Tiling & Offset", Vector) = (1, 1, 0, 0)
        _SecondTex_ST ("Texture 2 Tiling & Offset", Vector) = (1, 1, 0, 0)
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
            #include "UnityCG.cginc" // Needed for TRANSFORM_TEX

            sampler2D _MainTex;
            sampler2D _SecondTex;
            float _Blend;

            // Tiling and Offset Variables
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
                fixed4 col1 = tex2D(_MainTex, i.uv_MainTex);
                fixed4 col2 = tex2D(_SecondTex, i.uv_SecondTex);
                
                // Linearly interpolate between the two textures
                fixed4 finalColor = lerp(col1, col2, _Blend);
                
                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
