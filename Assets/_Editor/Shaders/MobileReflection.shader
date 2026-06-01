Shader "Akaito Ai/Mobile/Reflection/Reflection"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1, 1, 1, 1)
        _ReflectionColor ("Reflection Color", Color) = (1, 1, 1, 1)
        _Cube ("Reflection Cubemap", Cube) = "" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        CGPROGRAM
        #pragma surface surf Lambert
        
        samplerCUBE _Cube;
        fixed4 _MainColor;
        fixed4 _ReflectionColor;
        
        struct Input
        {
            float3 worldRefl; // Reflection vector
            INTERNAL_DATA
        };
        
        void surf (Input IN, inout SurfaceOutput o)
        {
            // Sample the reflection from the cubemap
            fixed4 reflection = texCUBE(_Cube, IN.worldRefl);
            
            // Combine main color with reflection color
            o.Albedo = _MainColor.rgb;
            o.Emission = reflection.rgb * _ReflectionColor.rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
