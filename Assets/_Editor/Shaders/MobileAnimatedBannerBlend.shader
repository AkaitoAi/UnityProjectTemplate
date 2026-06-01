Shader "Akaito Ai/Mobile/Animated Banner Blend"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)

        _Cols ("Cols Count", Int) = 5
        _Rows ("Rows Count", Int) = 3
        _Frame ("Per Frame Length", Float) = 0.5
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
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            uint _Cols;
            uint _Rows;
            float _Frame;

            // Calculate the current frame's UV coordinates
            fixed4 shot (sampler2D tex, fixed2 uv, fixed dx, fixed dy, int frame) {
                return tex2D(tex, fixed2(
                    (uv.x * dx) + fmod(frame, _Cols) * dx,
                    1.0 - ((uv.y * dy) + (frame / _Cols) * dy)
                ));
            }

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Calculate total number of frames in the grid
                int frames = _Rows * _Cols;

                // Calculate the current frame based on time
                fixed frame = fmod(_Time.y / _Frame, frames);
                int current = floor(frame);
                int next = floor(fmod(frame + 1, frames));

                // Calculate the width and height of each frame in UV space
                fixed dx = 1.0 / _Cols;
                fixed dy = 1.0 / _Rows;

                // Blend between the current and the next frame
                fixed4 currentFrame = shot(_MainTex, i.uv, dx, dy, current);
                fixed4 nextFrame = shot(_MainTex, i.uv, dx, dy, next);

                // Linear interpolation between frames
                fixed4 result = lerp(currentFrame, nextFrame, frame - current);

                // Apply color tint and return the result
                return result * _Color;
            }

            ENDCG
        }
    }
}
