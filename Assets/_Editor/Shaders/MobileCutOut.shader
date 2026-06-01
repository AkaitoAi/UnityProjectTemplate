Shader "Akaito Ai/Mobile/Transparent/CutOut" {
 
    Properties
    {
 
        _Cutoff("Alpha Cutoff" , Range(0, 1)) = .5
        _MainTex("Texture", 2D) = ""
    }
 
        SubShader
    {
        Alphatest Greater[_Cutoff]
        Cull Off
 
 
        BindChannels
    {
        Bind "vertex", vertex
        Bind "color", color
        Bind "texcoord1", texcoord
    }
 
        Pass
    {
        SetTexture[_MainTex]{ Combine texture, texture * primary }
    }
    }
}