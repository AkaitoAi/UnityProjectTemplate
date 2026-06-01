Shader "Akaito Ai/Mobile/ Bumped Emission" {
Properties{
    _MainTex("Base (RGB)", 2D) = "white" {}
    [NoScaleOffset] _BumpMap("Normalmap", 2D) = "bump" {}
    _Color("Color", Color) = (1, 1, 1, 1)
        _Emission("Emission", float) = 0
    }

    SubShader {
        Tags{ "Queue" = "Transparent" "RenderType" = "Transparency" }
        LOD 250
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

    CGPROGRAM
    #pragma surface surf Lambert noforwardadd alpha:fade

    sampler2D _MainTex;
    sampler2D _BumpMap;
    fixed4 _Color;
    float _Emission;

    struct Input {
        float2 uv_MainTex;
    };

    void surf(Input IN, inout SurfaceOutput o) {
        fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
        o.Albedo = c.rgb;
        o.Alpha = c.a;
        o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
        o.Emission = c.rgb * tex2D(_MainTex, IN.uv_MainTex).a * _Emission;
    }
    ENDCG
    }

        FallBack "Mobile/Diffuse"
}