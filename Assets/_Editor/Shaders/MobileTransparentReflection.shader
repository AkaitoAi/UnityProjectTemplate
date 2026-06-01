Shader "Akaito Ai/Mobile/Transparent/Transparent Reflective"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1, 1, 1, 1)
        _ReflectionColor ("Reflection Color", Color) = (1, 1, 1, 1)
        _Cube ("Reflection Cubemap", Cube) = "" {}
        _Transparency ("Transparency", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
        
        CGPROGRAM
        #pragma surface surf Lambert alpha:blend
        
        samplerCUBE _Cube;
        fixed4 _MainColor;
        fixed4 _ReflectionColor;
        float _Transparency;
        
        struct Input
        {
            float3 worldRefl; // Reflection vector
            INTERNAL_DATA
        };
        
        void surf (Input IN, inout SurfaceOutput o)
        {
            // Sample the reflection from the cubemap
            fixed4 reflection = texCUBE(_Cube, IN.worldRefl);
            
            // Combine main color with reflection color and apply transparency
            fixed4 baseColor = _MainColor;
            baseColor.a *= _Transparency;
            
            o.Albedo = baseColor.rgb;
            o.Emission = reflection.rgb * _ReflectionColor.rgb;
            o.Alpha = baseColor.a;
        }
        ENDCG
    }
    FallBack "Transparent/Diffuse"
}
