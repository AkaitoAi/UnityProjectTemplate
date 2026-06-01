Shader "Akaito Ai/Mobile/Blend/TextureBlendTransparent"
{
    Properties
    {
        _MainTex ("Texture 1", 2D) = "white" {}
        _SecondTex ("Texture 2", 2D) = "white" {}
        _MainColor1 ("Main Color 1", Color) = (1,1,1,1)
        _MainColor2 ("Main Color 2", Color) = (1,1,1,1)
        _Blend ("Blend Factor", Range(0,1)) = 0.5
        _AlphaCutoff ("Alpha Cutoff", Range(0,1)) = 0.5
        _Mode ("Render Mode (0=Opaque, 1=Cutout, 2=Fade)", Range(0,2)) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue" = "AlphaTest" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Use fixed precision for mobile optimization
            fixed4 _MainColor1;
            fixed4 _MainColor2;
            sampler2D _MainTex;
            sampler2D _SecondTex;
            fixed _Blend;
            fixed _AlphaCutoff;
            fixed _Mode;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample both textures and apply their respective colors
                fixed4 col1 = tex2D(_MainTex, i.uv) * _MainColor1;
                fixed4 col2 = tex2D(_SecondTex, i.uv) * _MainColor2;

                // Blend the two textures
                fixed4 finalColor = lerp(col1, col2, _Blend);

                // Handle transparency modes
                if (_Mode == 1)  // Cutout Mode
                {
                    clip(finalColor.a - _AlphaCutoff);  // Discard fragments below cutoff
                }
                else if (_Mode == 2)  // Fade Mode
                {
                    finalColor.a = lerp(col1.a, col2.a, _Blend);  // Smooth alpha blending
                }

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
