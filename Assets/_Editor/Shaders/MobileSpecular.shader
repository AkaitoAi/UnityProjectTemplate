Shader "Akaito Ai/Mobile/Reflection/Specular"
{
    Properties {
        _Shininess ("Shininess", Range(0.03, 1)) = 0.078125
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Glossiness ("Glossiness", 2D) = "black" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 300

        CGPROGRAM
        #pragma surface surf MobileBlinnPhong exclude_path:prepass nolightmap noforwardadd halfasview

        inline fixed4 LightingMobileBlinnPhong(SurfaceOutput s, fixed lightDir, fixed3 halfDir, fixed atten)
        {
            fixed diff = max(0, dot(s.Normal, lightDir));
            fixed nh = max(0, dot(s.Normal, halfDir));
            fixed spec = pow(nh, s.Specular * 128) * s.Gloss;

            fixed4 c;
            c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * (atten * 2);
            c.a = 0.0;

            return c;
        }

        sampler2D _MainTex;
        sampler2D _Glossiness;
        sampler2D _BumpMap;
        half _Shininess;

        struct Input {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o) {
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = tex.rgb;
            o.Gloss = tex2D(_Glossiness, IN.uv_MainTex).rgb;
            o.Alpha = tex.a;
            o.Specular = _Shininess;
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
        }

        ENDCG
    }

    Fallback "Mobile/VertexLit"
}
