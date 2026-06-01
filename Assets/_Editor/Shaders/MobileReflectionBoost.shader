Shader "Akaito Ai/Mobile/Reflection/Reflection Boost" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _DiffuseBoost ("Diffuse Boost", Range(0.03, 3)) = 1
        _MainTex ("Diffuse Map", 2D) = "white" {}
        _SpecColor ("Spec Lighting Color", Color) = (0.5,0.5,0.5,1)
        _Gloss ("(R)Specular Boost", Range(0.03, 10)) = 0.078125
        _Spec ("(G)Gloss Boost", Range(0.01, 0.3)) = 0.078125
        _Cube ("Cubemap", Cube) = "" {}
        _EnvBoost ("(A)Reflection Boost", Range(0.03, 1)) = 0.078125
    }

    SubShader{
        Tags { "RenderType"="Opaque" }
        LOD 200
        CGPROGRAM
        #pragma surface surf Standard
        #pragma target 3.0

        sampler2D _MainTex;
        samplerCUBE _Cube;
        fixed4 _Color;
        float _EnvBoost;

        struct Input {
            float2 uv_MainTex;
            float3 worldRefl; // Reflection vector for reflection mapping
        };

        void surf(Input IN, inout SurfaceOutputStandard o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;

            // Sample cubemap for reflection
            fixed4 reflection = texCUBE(_Cube, IN.worldRefl) * _EnvBoost;
            o.Emission = reflection.rgb;
        }
        ENDCG
    }
    Fallback "VertexLit"
}
