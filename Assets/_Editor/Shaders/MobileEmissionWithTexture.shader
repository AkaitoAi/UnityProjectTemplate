Shader "Akaito Ai/Mobile/ Texture Emission"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _EmissionTex ("Emission Texture", 2D) = "black" {}
        _EmissionStrength ("Emission Strength", Range(0, 2)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        CGPROGRAM
        #pragma surface surf Lambert
        
        sampler2D _MainTex;
        sampler2D _EmissionTex;
        float _EmissionStrength;
        
        struct Input
        {
            float2 uv_MainTex;
            float2 uv_EmissionTex;
        };
        
        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 mainColor = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 emissionColor = tex2D(_EmissionTex, IN.uv_EmissionTex);
            
            o.Albedo = mainColor.rgb;
            o.Emission = emissionColor.rgb * _EmissionStrength;
        }
        ENDCG
    }
    FallBack "Diffuse"
}